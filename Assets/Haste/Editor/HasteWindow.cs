using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Haste {

  [Serializable]
  public class HasteWindow : EditorWindow {

    public static HasteWindow Instance { get; protected set; }

    const int itemHeight = 46;
    const int width = 500;
    const int height = 300;

    const int resultCount = 25;

    IHasteResult[] results = new IHasteResult[0];
    IHasteResult selectedResult;
    IHasteResult highlightedResult;

    Vector2 scrollPosition = Vector2.zero;

    int highlightedIndex = 0;

    HasteBlur blur;

    [SerializeField]
    Texture backgroundTexture;

    string currentTip;

    [SerializeField]
    HasteGUIQuery queryInput;

    public static bool IsOpen {
      get { return Instance == EditorWindow.focusedWindow; }
    }

    [MenuItem("Window/Haste %k", true)]
    public static bool IsHasteEnabled() {
      return HasteSettings.Enabled;
    }

    [MenuItem("Window/Haste %k")]
    public static void Open() {
      if (HasteWindow.Instance == null) {
        HasteWindow.Init();
      } else {
        // Return the existing window if it exists
        EditorWindow.GetWindow<HasteWindow>();
      }
    }

    // Creates a new window instance and initializes it
    static void Init() {
      HasteSettings.UsageCount = HasteSettings.UsageCount + 1;
      HasteSelectionManager.Save();

      HasteWindow.Instance = EditorWindow.CreateInstance<HasteWindow>();
      HasteWindow.Instance.InitializeInstance();
    }

    void InitializeInstance() {
      this.title = "Haste";

      this.currentTip = HasteTips.Random;

      this.position = new Rect(
        (Screen.currentResolution.width - width) / 2,
        (Screen.currentResolution.height - height) / 2,
        width, height
      );

      // Disable the resize handle on the window
      this.minSize = this.maxSize = new Vector2(width, height);

      this.queryInput = ScriptableObject.CreateInstance<HasteGUIQuery>();
      this.queryInput.Changed += QueryChanged;

      this.DestroyBlur(); // Must include since it leaks otherwise...
      this.UpdateBlur();
      this.ShowPopup();
      this.Focus();
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
        blur = new HasteBlur(width, height, HasteColors.BlurColor);

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
      HasteSelectionManager.Restore();
      base.Close();
    }

    void OnEscape() {
      Close();
    }

    void OnReturn() {
      Close();

      if (results.Length > 0 && highlightedIndex >= 0) {
        SelectResult(results[highlightedIndex]);
      }
    }

    void SelectResult(IHasteResult result) {
      selectedResult = result;
      selectedResult.Action();
    }

    void SetHighlightedIndex(int index, bool updateScroll = true) {
      highlightedIndex = index;

      if (highlightedIndex < 0 || highlightedIndex > results.Length - 1) {
        if (updateScroll) {
          ResetScroll();
        }
        return;
      }

      highlightedResult = results[highlightedIndex];
      highlightedResult.Select();

      if (updateScroll) {
        UpdateScroll();
      }
    }

    void OnUpArrow() {
      int index = Math.Max(highlightedIndex - 1, 0);
      SetHighlightedIndex(index);
    }

    void OnDownArrow() {
      int index = Math.Min(highlightedIndex + 1, results.Length - 1);
      SetHighlightedIndex(index);
    }

    void ResetScroll() {
      scrollPosition = Vector2.zero;
    }

    void UpdateScroll() {
      int highlightOffset = highlightedIndex * itemHeight;
      scrollPosition = new Vector2(scrollPosition.x, highlightOffset);
    }

    void OnKeyDown(Event e) {
      switch (e.keyCode) {
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

    void OnMouseDrag(Event e) {
      // TODO: highlightedResult is wrong
      DragAndDrop.PrepareStartDrag();
      DragAndDrop.objectReferences = new UnityEngine.Object[]{highlightedResult.Object};
      DragAndDrop.StartDrag(highlightedResult.DragLabel);
      Event.current.Use();
    }

    void OnEvent(Event e) {
      switch (e.type) {
        case EventType.MouseDrag:
          OnMouseDrag(e);
          break;
        case EventType.KeyDown:
          OnKeyDown(e);
          break;
      }
    }

    void DrawResult(IHasteResult result, int index) {
      var isHighlighted = index == highlightedIndex;
      var resultStyle = isHighlighted ? HasteStyles.HighlightStyle : HasteStyles.NonHighlightStyle;

      using (var horizontal = new HasteHorizontal(resultStyle, GUILayout.Height(itemHeight))) {
        var e = Event.current;

        var button = HasteGUILayout.Button(horizontal.Rect);
        switch (button) {
          case ButtonEvent.DoubleClick:
            Close();
            SelectResult(result);
            break;
          case ButtonEvent.SingleClick:
            SetHighlightedIndex(index, false);
            break;
        }

        result.Draw(isHighlighted);
      }
    }

    void DrawResults() {
      using (var scrollView = new HasteScrollView(scrollPosition,
        GUILayout.ExpandWidth(true),
        GUILayout.ExpandHeight(true))) {

        scrollPosition = scrollView.ScrollPosition;

        for (int i = 0; i < results.Length; i++) {
          IHasteResult result = results[i];
          DrawResult(result, i);
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

    void Update() {
      if (queryInput.Query == "") {
        // XXX: This is only here to repaint the indexing count
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
      SetHighlightedIndex(0);
    }

    void QueryChanged(string query) {
      if (query == "") {
        UpdateResults(new IHasteResult[0]);
      } else {
        UpdateResults(Haste.Index.Filter(query, resultCount));
      }
    }

    void OnGUI() {
      if (backgroundTexture != null) {
        UnityEngine.GUI.DrawTexture(new Rect(0, 0, width, height), backgroundTexture);
      }

      OnEvent(Event.current);

      this.queryInput.OnGUI();

      if (this.queryInput.Query == "") {
        DrawIntro();
      } else if (results.Length == 0) {
        DrawEmptyResults();
      } else {
        DrawResults();
      }
    }
  }
}
