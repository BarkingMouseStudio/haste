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

    public static GUIStyle QueryStyle;
    public static GUIStyle NameStyle;
    public static GUIStyle DescriptionStyle;
    public static GUIStyle PrefixStyle;
    public static GUIStyle DisabledPrefixStyle;
    public static GUIStyle IntroStyle;
    public static GUIStyle IndexingStyle;
    public static GUIStyle HighlightStyle;
    public static GUIStyle NonHighlightStyle;
    public static GUIStyle EmptyStyle;
    public static GUIStyle UpgradeStyle;
    public static GUIStyle DisabledNameStyle;
    public static GUIStyle DisabledDescriptionStyle;

    public static string BoldStart;
    public static string BoldEnd;

    static HasteWindow instance;
    static int width = 500;
    static int height = 300;

    IHasteResult[] results = new IHasteResult[0];
    IHasteResult selectedResult;
    // IHasteResult highlightedResult;

    Vector2 scrollPosition = Vector2.zero;

    string query = "";

    int highlightedIndex = 0;

    #if IS_HASTE_PRO
    HasteBlur blur;
    Texture backgroundTexture;
    #endif

    const int itemHeight = 46;
    const int itemStepHeightOffset = 6;
    const int prefixWidth = 96;
    const int groupSpacing = 10;

    const int resultCount = 25;

    [PreferenceItem("Haste")]
    public static void PreferencesGUI() {
      EditorGUILayout.Space();

      #if !IS_HASTE_PRO
      EditorGUILayout.HelpBox("Upgrade to Haste Pro to enable more features (like actions).", MessageType.Warning);
      EditorGUILayout.Space();
      if (GUILayout.Button("Upgrade to Haste Pro", GUILayout.Width(128))) {
        UnityEditorInternal.AssetStore.Open(Haste.ASSET_STORE_PRO_URL);
      }

      EditorGUILayout.Space();
      EditorGUILayout.Space();
      #endif

      if (GUILayout.Button("Rebuild Index", GUILayout.Width(128))) {
        Haste.Rebuild();
      }
      EditorGUILayout.Space();
      EditorGUILayout.HelpBox("Rebuilds the internal index used for fast searching in Haste. Use this if Haste starts providing weird results.", MessageType.Info);

      EditorGUILayout.Space();
      EditorGUILayout.Space();

      EditorGUILayout.LabelField("Times Opened", Haste.UsageCount.ToString());

      EditorGUILayout.Space();
      EditorGUILayout.Space();

      EditorGUILayout.LabelField("Available Sources", EditorStyles.whiteLargeLabel);
      EditorGUILayout.Space();

      using (var toggleGroup = new HasteToggleGroup("Haste Enabled", Haste.Enabled)) {
        Haste.Enabled = toggleGroup.Enabled;
        EditorGUILayout.Space();

        foreach (var watcher in Haste.Watchers) {
          string label = String.Format("{0} ({1})", watcher.Key, watcher.Value.IndexedCount);
          bool enabled = EditorGUILayout.Toggle(label, watcher.Value.Enabled);
          EditorPrefs.SetBool(Haste.GetPrefKey("Source", watcher.Key), enabled);
          Haste.Watchers.ToggleSource(watcher.Key, enabled);
        }
      }

      EditorGUILayout.Space();

      if (Haste.IsIndexing) {
        EditorGUILayout.LabelField("Indexing...", Haste.IndexingCount.ToString());
      } else {
        EditorGUILayout.LabelField("Index Size", Haste.IndexSize.ToString());
      }
    }

    static void Init() {
      // Styles
      HasteWindow.QueryStyle = new GUIStyle(EditorStyles.textField);
      HasteWindow.QueryStyle.fixedHeight = 64;
      HasteWindow.QueryStyle.alignment = TextAnchor.MiddleLeft;
      HasteWindow.QueryStyle.fontSize = 32;

      HasteWindow.IntroStyle = new GUIStyle(EditorStyles.largeLabel);
      HasteWindow.IntroStyle.fixedHeight = 64;
      HasteWindow.IntroStyle.alignment = TextAnchor.MiddleCenter;
      HasteWindow.IntroStyle.fontSize = 32;

      HasteWindow.IndexingStyle = new GUIStyle(EditorStyles.largeLabel);
      HasteWindow.IndexingStyle.fixedHeight = 24;
      HasteWindow.IndexingStyle.alignment = TextAnchor.MiddleCenter;
      HasteWindow.IndexingStyle.fontSize = 16;
      HasteWindow.IndexingStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f);

      HasteWindow.NameStyle = new GUIStyle(EditorStyles.largeLabel);
      HasteWindow.NameStyle.alignment = TextAnchor.MiddleLeft;
      HasteWindow.NameStyle.fixedHeight = 24;
      HasteWindow.NameStyle.fontSize = 16;

      HasteWindow.EmptyStyle = new GUIStyle(EditorStyles.largeLabel);
      HasteWindow.EmptyStyle.alignment = TextAnchor.MiddleCenter;
      HasteWindow.EmptyStyle.fixedHeight = 24;
      HasteWindow.EmptyStyle.fontSize = 16;

      HasteWindow.UpgradeStyle = new GUIStyle(EditorStyles.largeLabel);
      HasteWindow.UpgradeStyle.alignment = TextAnchor.MiddleCenter;
      HasteWindow.UpgradeStyle.fixedHeight = 24;
      HasteWindow.UpgradeStyle.fontSize = 16;
      HasteWindow.UpgradeStyle.normal.textColor = new Color(0.2f, 0.30f, 0.82f);

      HasteWindow.DisabledNameStyle = new GUIStyle(EditorStyles.largeLabel);
      HasteWindow.DisabledNameStyle.alignment = TextAnchor.MiddleLeft;
      HasteWindow.DisabledNameStyle.fixedHeight = 24;
      HasteWindow.DisabledNameStyle.fontSize = 16;
      HasteWindow.DisabledNameStyle.normal.textColor = new Color(0.4f, 0.4f, 0.4f);

      HasteWindow.DisabledDescriptionStyle = new GUIStyle(EditorStyles.largeLabel);
      HasteWindow.DisabledDescriptionStyle.alignment = TextAnchor.MiddleLeft;
      HasteWindow.DisabledDescriptionStyle.fixedHeight = 24;
      HasteWindow.DisabledDescriptionStyle.fontSize = 12;
      HasteWindow.DisabledDescriptionStyle.normal.textColor = new Color(0.4f, 0.4f, 0.4f);
      HasteWindow.DisabledDescriptionStyle.richText = true;

      HasteWindow.DescriptionStyle = new GUIStyle(EditorStyles.largeLabel);
      HasteWindow.DescriptionStyle.alignment = TextAnchor.MiddleLeft;
      HasteWindow.DescriptionStyle.fixedHeight = 24;
      HasteWindow.DescriptionStyle.fontSize = 12;
      HasteWindow.DescriptionStyle.richText = true;

      HasteWindow.PrefixStyle = new GUIStyle(EditorStyles.largeLabel);
      HasteWindow.PrefixStyle.alignment = TextAnchor.MiddleRight;
      HasteWindow.PrefixStyle.fixedHeight = 18;
      HasteWindow.PrefixStyle.fontSize = 12;

      HasteWindow.DisabledPrefixStyle = new GUIStyle(EditorStyles.largeLabel);
      HasteWindow.DisabledPrefixStyle.alignment = TextAnchor.MiddleRight;
      HasteWindow.DisabledPrefixStyle.fixedHeight = 18;
      HasteWindow.DisabledPrefixStyle.fontSize = 12;
      HasteWindow.DisabledPrefixStyle.normal.textColor = new Color(0.4f, 0.4f, 0.4f);

      if (EditorGUIUtility.isProSkin) {
        HasteWindow.BoldStart = "<color=\"#ddd\"><b>";
      } else {
        HasteWindow.BoldStart = "<color=\"#ddd\"><b>";
      }
      HasteWindow.BoldEnd = "</b></color>";

      HasteWindow.NonHighlightStyle = new GUIStyle();
      HasteWindow.HighlightStyle = new GUIStyle();
      if (EditorGUIUtility.isProSkin) {
        HasteWindow.HighlightStyle.normal.background = HasteUtils.CreateColorSwatch(new Color(0.275f, 0.475f, 0.95f, 0.2f));
      } else {
        HasteWindow.HighlightStyle.normal.background = HasteUtils.CreateColorSwatch(new Color(0.045f, 0.22f, 0.895f, 0.2f));
      }

      // Window
      instance = EditorWindow.CreateInstance<HasteWindow>();
      instance.title = "Haste";

      #if IS_HASTE_PRO
      if (Application.HasProLicense() && EditorGUIUtility.isProSkin) {
        // Blurring
        instance.blur = new HasteBlur(width, height);
      }
      #endif
    }

    public static bool IsOpen {
      get { return instance == EditorWindow.focusedWindow; }
    }

    [MenuItem("Window/Haste %k")]
    public static void Open() {
      if (IsOpen) {
        // Window is already open
        return;
      }

      if (instance == null) {
        Init();
      }

      Haste.UsageCount++;

      instance.position = new Rect(
        (Screen.currentResolution.width - width) / 2,
        (Screen.currentResolution.height - height) / 2,
        width, height
      );
      instance.UpdateBlur();
      instance.ShowPopup();
      instance.Focus();
    }

    void UpdateBlur() {
      #if IS_HASTE_PRO
      if (instance.blur != null) {
        // Must grab texture before Haste is visible
        instance.backgroundTexture = instance.blur.BlurTexture(
          HasteUtils.GrabScreenSwatch(instance.position)
        );
      }
      #endif
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
      if (results.Length > 0 && highlightedIndex >= 0) {
        Close();

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

      // highlightedResult = results[highlightedIndex];
      // highlightedResult.Select();

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
      var resultStyle = index == highlightedIndex ? HasteWindow.HighlightStyle : HasteWindow.NonHighlightStyle;
      using (var horizontal = new HasteHorizontal(resultStyle, GUILayout.Height(itemHeight))) {
        if (GUI.Button(horizontal.Rect, "", GUIStyle.none)) {
          result.Action();
          Close();
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
            var style = PrefixStyle;

            #if !IS_HASTE_PRO
            if (result.Item.Source == HasteMenuItemSource.NAME) {
              style = DisabledPrefixStyle;
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
        EditorGUILayout.LabelField("No results found.", EmptyStyle);
      }
    }

    void DrawUpsell() {
      #if !IS_HASTE_PRO
      int bottomOffset = 8;
      if (GUI.Button(new Rect(0, height - UpgradeStyle.fixedHeight - bottomOffset, width, UpgradeStyle.fixedHeight), "Click here to upgrade to Haste Pro", UpgradeStyle)) {
        UnityEditorInternal.AssetStore.Open(Haste.ASSET_STORE_PRO_URL);
      }
      #endif
    }

    void DrawPlaying() {
      using (new HasteSpace()) {
        EditorGUILayout.LabelField("Haste currently only works when not in play mode.", EmptyStyle,
          GUILayout.Height(EmptyStyle.fixedHeight));
      }
    }

    void DrawIntro() {
      using (new HasteSpace()) {
        EditorGUILayout.LabelField("Just type.", IntroStyle,
          GUILayout.Height(HasteWindow.IntroStyle.fixedHeight));

        if (Haste.IsIndexing) {
          EditorGUILayout.LabelField(String.Format("(Indexing {0}...)", Haste.IndexingCount), IndexingStyle);
        }
      }
    }

    void DrawQuery() {
      using (new HasteFocus("query")) {
        query = EditorGUILayout.TextField(query, QueryStyle,
          GUILayout.Height(HasteWindow.QueryStyle.fixedHeight));
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
      #if IS_HASTE_PRO
      if (backgroundTexture != null) {
        UnityEngine.GUI.DrawTexture(new Rect(0, 0, width, height), backgroundTexture);
      }
      #endif

      OnEvent(Event.current);

      DrawQuery();

      // if (Application.isPlaying) {
      //   DrawPlaying();
      //   return;
      // }

      if (GUI.changed) {
        OnGUIChanged();
      }

      if (query == "") {
        DrawIntro();
        DrawUpsell();
      } else if (results.Length == 0) {
        DrawEmptyResults();
        DrawUpsell();
      } else {
        DrawResults();
      }
    }
  }
}
