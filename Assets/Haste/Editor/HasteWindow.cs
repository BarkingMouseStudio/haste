using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Haste {

  public class HasteWindow : EditorWindow {

    public static HasteWindow Instance { get; protected set; }

    const int itemHeight = 46;
    const int itemStepHeightOffset = 6;
    const int prefixWidth = 96;
    const int groupSpacing = 10;
    const int width = 500;
    const int height = 300;

    const int resultCount = 25;

    int initialActiveInstanceId;

    IHasteResult[] results = new IHasteResult[0];
    IHasteResult selectedResult;
    IHasteResult highlightedResult;

    Vector2 scrollPosition = Vector2.zero;

    string query = "";
    int highlightedIndex = 0;

    HasteBlur blur;
    Texture backgroundTexture;

    string currentTip;

    public void RestoreInitialSelection() {
      // Restore initial selection
      Selection.activeInstanceID = initialActiveInstanceId;
    }

    public static bool IsOpen {
      get { return Instance == EditorWindow.focusedWindow; }
    }

    [MenuItem("Window/Haste %k", true)]
    public static bool IsHasteEnabled() {
      return Haste.Enabled;
    }

    [MenuItem("Window/Haste %k")]
    public static void Open() {
      if (IsOpen) {
        // Window is already open
        return;
      }

      if (Instance == null) {
        // Window
        Instance = EditorWindow.CreateInstance<HasteWindow>();
        Instance.title = "Haste";
      }

      Haste.UsageCount++;

      Instance.initialActiveInstanceId = Selection.activeInstanceID;

      Instance.currentTip = HasteTips.Random;

      Instance.position = new Rect(
        (Screen.currentResolution.width - width) / 2,
        (Screen.currentResolution.height - height) / 2,
        width, height
      );

      // Disable the resize handle on the window
      Instance.minSize = Instance.maxSize = new Vector2(width, height);

      Instance.UpdateBlur();
      Instance.ShowPopup();
      Instance.Focus();
    }

    void DestroyBlur() {
      if (blur != null) {
        blur.Dispose();
        blur = null;
      }

      if (backgroundTexture != null) {
        Texture.DestroyImmediate(backgroundTexture);
        backgroundTexture = null;
      }
    }

    void UpdateBlur() {
      if (Application.HasProLicense()) {
        blur = new HasteBlur(width, height, EditorGUIUtility.isProSkin ? Color.black : Color.white);

        // Must grab texture before Haste is visible
        backgroundTexture = blur.BlurTexture(
          HasteUtils.GrabScreenSwatch(position)
        );
      }
    }

    void OnDestroy() {
      DestroyBlur();
    }

    new void Close() {
      RestoreInitialSelection();
      base.Close();
    }

    void OnBackspace() {
      if (query != "") {
        ClearQuery();
      }
    }

    void OnEscape() {
      Close();
    }

    void OnReturn() {
      Close();

      if (results.Length > 0 && highlightedIndex >= 0) {
        selectedResult = results[highlightedIndex];
        selectedResult.Action();
      }
    }

    void UpdateHighlightedIndex(int index) {
      highlightedIndex = index;

      if (highlightedIndex < 0 || highlightedIndex > results.Length - 1) {
        scrollPosition = Vector2.zero;
        return;
      }

      highlightedResult = results[highlightedIndex];
      highlightedResult.Select();

      UpdateScroll();
    }

    void OnUpArrow() {
      int index = Math.Max(highlightedIndex - 1, 0);
      UpdateHighlightedIndex(index);
    }

    void OnDownArrow() {
      int index = Math.Min(highlightedIndex + 1, results.Length - 1);
      UpdateHighlightedIndex(index);
    }

    void UpdateScroll() {
      int previousGroups = 0;
      for (int i = 0; i <= highlightedIndex; i++) {
        if (i > 0 && results[i].Item.Source != results[i - 1].Item.Source) {
          previousGroups++;
        }
      }

      // Account for leading and between group spacing
      int highlightOffset = highlightedIndex * (itemHeight + itemStepHeightOffset) +
        (groupSpacing * highlightedIndex > 0 ? 1 : 0) +
        (groupSpacing * previousGroups);
      scrollPosition = new Vector2(scrollPosition.x, highlightOffset);
    }

    void OnKeyDown(Event e) {
      switch (e.keyCode) {
        case KeyCode.Backspace:
          e.Use();
          OnBackspace();
          break;
        case KeyCode.Escape:
          e.Use();
          OnEscape();
          break;
        case KeyCode.Return:
          e.Use();
          OnReturn();
          break;
        case KeyCode.UpArrow:
          e.Use();
          OnUpArrow();
          break;
        case KeyCode.DownArrow:
          e.Use();
          OnDownArrow();
          break;
      }
    }

    void OnEvent(Event e) {
      switch (e.type) {
        case EventType.KeyDown:
          OnKeyDown(e);
          break;
      }
    }

    void DrawResult(IHasteResult result, int index) {
      var resultStyle = index == highlightedIndex ? HasteStyles.HighlightStyle : HasteStyles.NonHighlightStyle;

      using (var horizontal = new HasteHorizontal(resultStyle, GUILayout.Height(itemHeight))) {
        if (GUI.Button(horizontal.Rect, "", GUIStyle.none)) {
          Close();
          result.Action();
          return;
        }

        result.Draw();
      }
    }

    void DrawResults() {
      using (var scrollView = new HasteScrollView(scrollPosition,
        GUILayout.ExpandWidth(true),
        GUILayout.ExpandHeight(true))) {

        scrollPosition = scrollView.ScrollPosition;

        for (int i = 0; i < results.Length; i++) {
          IHasteResult result = results[i];

          bool isBeginGroup = false;
          if (i == 0) {
            isBeginGroup = true;
          } else {
            IHasteResult prevResult = results[i - 1];
            if (result.Item.Source != prevResult.Item.Source) {
              isBeginGroup = true;
            }
          }

          if (isBeginGroup) {
            var style = HasteStyles.PrefixStyle;

            #if !IS_HASTE_PRO
            if (result.Item.Source == HasteMenuItemSource.NAME) {
              style = HasteStyles.DisabledPrefixStyle;
            }
            #endif

            // Begin group
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
              result.Item.Source,
              style,
              GUILayout.Width(prefixWidth));
            EditorGUILayout.BeginVertical();
          }

          DrawResult(result, i);
          EditorGUILayout.Space();

          bool isEndGroup = false;
          if (i == results.Length - 1) {
            isEndGroup = true;
          } else {
            IHasteResult nextResult = results[i + 1];
            if (result.Item.Source != nextResult.Item.Source) {
              isEndGroup = true;
            }
          }

          if (isEndGroup) {
            // End group
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
          }
        }
      }
    }

    void DrawEmptyResults() {
      using (new HasteSpace()) {
        EditorGUILayout.LabelField("No results found.", HasteStyles.EmptyStyle);

        HasteGUILayout.Expander();

        EditorGUILayout.LabelField(currentTip, HasteStyles.TipStyle);
        DrawUpsell();
      }
    }

    void DrawUpsell() {
      #if !IS_HASTE_PRO
      if (GUILayout.Button("Click here to upgrade to Haste Pro", HasteStyles.UpgradeStyle)) {
        UnityEditorInternal.AssetStore.Open(Haste.ASSET_STORE_PRO_URL);
      }
      #endif
    }

    void DrawPlaying() {
      using (new HasteSpace()) {
        EditorGUILayout.LabelField("Haste currently only works when not in play mode.", HasteStyles.EmptyStyle,
          GUILayout.Height(HasteStyles.EmptyStyle.fixedHeight));
      }
    }

    void DrawIntro() {
      using (new HasteSpace()) {
        EditorGUILayout.LabelField("Just type.", HasteStyles.IntroStyle,
          GUILayout.Height(HasteStyles.IntroStyle.fixedHeight));

        HasteGUILayout.Expander();

        if (Haste.IsIndexing) {
          EditorGUILayout.LabelField(String.Format("(Indexing {0}...)", Haste.IndexingCount), HasteStyles.IndexingStyle);
        } else {
          EditorGUILayout.LabelField(currentTip, HasteStyles.TipStyle);
        }

        DrawUpsell();
      }
    }

    void DrawQuery() {
      using (new HasteFocus("query")) {
        query = EditorGUILayout.TextField(query, HasteStyles.QueryStyle,
          GUILayout.Height(HasteStyles.QueryStyle.fixedHeight));
        query = query.Trim();
      }
    }

    void Update() {
      if (query == "") {
        Repaint();
      }

      if (this != EditorWindow.focusedWindow) {
        // Check if we lost focus and close:
        // Cannot use OnLostFocus due to render bug in Unity
        Close();
      }
    }

    void UpdateResults(IHasteResult[] updatedResults) {
      results = updatedResults;
      UpdateHighlightedIndex(0);
    }

    void ClearQuery() {
      UpdateResults(new IHasteResult[0]);
      query = "";
    }

    void UpdateQuery() {
      if (query != "") {
        UpdateResults(Haste.Index.Filter(query, resultCount));
      } else {
        ClearQuery();
      }
    }

    void OnGUIChanged() {
      UpdateQuery();
    }

    void OnGUI() {
      if (backgroundTexture != null) {
        UnityEngine.GUI.DrawTexture(new Rect(0, 0, width, height), backgroundTexture);
      }

      OnEvent(Event.current);

      DrawQuery();

      if (GUI.changed) {
        OnGUIChanged();
      }

      if (query == "") {
        DrawIntro();
      } else if (results.Length == 0) {
        DrawEmptyResults();
      } else {
        DrawResults();
      }
    }
  }
}
