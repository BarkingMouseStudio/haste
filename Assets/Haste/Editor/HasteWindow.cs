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
    public static GUIStyle IntroStyle;
    public static GUIStyle NonHighlightStyle;
    public static GUIStyle HighlightStyle;
    public static GUIStyle EmptyStyle;
    public static GUIStyle DisabledNameStyle;
    public static GUIStyle DisabledDescriptionStyle;

    public static Texture GameObjectIcon;

    static HasteWindow instance;

    IHasteResult[] results = new IHasteResult[0];
    IHasteResult selectedResult;

    Vector2 scrollPosition = Vector2.zero;

    string query = "";

    int highlightedIndex = 0;

    const int itemHeight = 46;
    const int prefixWidth = 96;
    const int groupSpacing = 10;

    const int resultCount = 5;

    [PreferenceItem("Haste")]
    public static void PreferencesGUI() {
      EditorGUILayout.Space();
      if (GUILayout.Button("Rebuild Index", GUILayout.Width(128))) {
        Haste.Rebuild();
      }

      EditorGUILayout.Space();
      EditorGUILayout.HelpBox("Rebuilds the internal index used for fast searching in Haste. Use this if Haste starts providing weird results.", MessageType.Info);

      EditorGUILayout.Space();
      EditorGUILayout.LabelField("Open Count", Haste.UsageCount.ToString());

      EditorGUILayout.Space();
      EditorGUILayout.HelpBox("Indicates how many times you have opened Haste.", MessageType.Info);
    }

    static void Init() {
      HasteWindow.GameObjectIcon = EditorGUIUtility.ObjectContent(null, typeof(GameObject)).image;

      // Styles
      HasteWindow.QueryStyle = new GUIStyle(EditorStyles.textField);
      HasteWindow.QueryStyle.fixedHeight = 64;
      HasteWindow.QueryStyle.alignment = TextAnchor.MiddleLeft;
      HasteWindow.QueryStyle.fontSize = 32;

      HasteWindow.IntroStyle = new GUIStyle(EditorStyles.largeLabel);
      HasteWindow.IntroStyle.fixedHeight = 64;
      HasteWindow.IntroStyle.alignment = TextAnchor.MiddleCenter;
      HasteWindow.IntroStyle.fontSize = 32;

      HasteWindow.NameStyle = new GUIStyle(EditorStyles.largeLabel);
      HasteWindow.NameStyle.alignment = TextAnchor.MiddleLeft;
      HasteWindow.NameStyle.fixedHeight = 24;
      HasteWindow.NameStyle.fontSize = 16;

      HasteWindow.EmptyStyle = new GUIStyle(EditorStyles.largeLabel);
      HasteWindow.EmptyStyle.alignment = TextAnchor.MiddleCenter;
      HasteWindow.EmptyStyle.fixedHeight = 24;
      HasteWindow.EmptyStyle.fontSize = 16;

      HasteWindow.DisabledNameStyle = new GUIStyle(EditorStyles.largeLabel);
      HasteWindow.DisabledNameStyle.alignment = TextAnchor.MiddleLeft;
      HasteWindow.DisabledNameStyle.fixedHeight = 24;
      HasteWindow.DisabledNameStyle.fontSize = 16;
      HasteWindow.DisabledNameStyle.normal.textColor = new Color(0.45f, 0.45f, 0.45f);

      HasteWindow.DisabledDescriptionStyle = new GUIStyle(EditorStyles.largeLabel);
      HasteWindow.DisabledDescriptionStyle.alignment = TextAnchor.MiddleLeft;
      HasteWindow.DisabledDescriptionStyle.fixedHeight = 24;
      HasteWindow.DisabledDescriptionStyle.fontSize = 12;
      HasteWindow.DisabledDescriptionStyle.normal.textColor = new Color(0.45f, 0.45f, 0.45f);

      HasteWindow.DescriptionStyle = new GUIStyle(EditorStyles.largeLabel);
      HasteWindow.DescriptionStyle.alignment = TextAnchor.MiddleLeft;
      HasteWindow.DescriptionStyle.fixedHeight = 24;
      HasteWindow.DescriptionStyle.fontSize = 12;
      HasteWindow.DescriptionStyle.richText = true;

      HasteWindow.PrefixStyle = new GUIStyle(EditorStyles.largeLabel);
      HasteWindow.PrefixStyle.alignment = TextAnchor.MiddleRight;
      HasteWindow.PrefixStyle.fixedHeight = 18;
      HasteWindow.PrefixStyle.fontSize = 12;

      HasteWindow.NonHighlightStyle = new GUIStyle();
      HasteWindow.HighlightStyle = new GUIStyle();

      if (EditorGUIUtility.isProSkin) {
        HasteWindow.HighlightStyle.normal.background = HasteUtils.CreateTexture(new Color(0.275f, 0.475f, 0.95f, 0.2f));
      } else {
        HasteWindow.HighlightStyle.normal.background = HasteUtils.CreateTexture(new Color(0.045f, 0.22f, 0.895f, 0.2f));
      }

      // Window
      instance = EditorWindow.CreateInstance<HasteWindow>();

      int width = 500;
      int height = 300;
      int x = (Screen.currentResolution.width - width) / 2;
      int y = (Screen.currentResolution.height - height) / 2;
      instance.position = new Rect(x, y, width, height);
      instance.minSize = instance.maxSize = new Vector2(instance.position.width, instance.position.height);
      instance.title = "Haste";
    }

    [MenuItem("Window/Haste %k")]
    public static void Open() {
      if (instance == null) {
        Init();
      }

      Haste.UsageCount++;

      instance.ShowPopup();
      instance.Focus();
    }

    void OnEscape() {
      // On escape, clear context, clear the query or close the window
      if (query != "") {
        ClearQuery();
      } else {
        Close();
      }
    }

    void OnReturn() {
      if (results.Length > 0) {
        selectedResult = results[highlightedIndex];
        selectedResult.Action();

        Close();
      }
    }

    void OnUpArrow() {
      highlightedIndex = Math.Max(highlightedIndex - 1, 0);
      UpdateScroll();
    }

    void OnDownArrow() {
      highlightedIndex = Math.Min(highlightedIndex + 1, results.Length - 1);
      UpdateScroll();
    }

    void UpdateScroll() {
      int previousGroups = 0;
      for (int i = 0; i <= highlightedIndex; i++) {
        if (i > 0 && results[i].Item.Source != results[i - 1].Item.Source) {
          previousGroups++;
        }
      }

      // Account for leading and between group spacing
      int highlightOffset = highlightedIndex * itemHeight +
        (groupSpacing * highlightedIndex > 0 ? 1 : 0) +
        (groupSpacing * previousGroups);
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
        if (UnityEngine.GUI.Button(horizontal.Rect, "", GUIStyle.none)) {
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
            // Begin group
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
              result.Item.Source,
              PrefixStyle,
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

    void DrawIntro() {
      using (new HasteSpace()) {
        EditorGUILayout.LabelField("Just Type.", IntroStyle);
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
      if (this != EditorWindow.focusedWindow) {
        // Check if we lost focus and close:
        // Cannot use OnLostFocus due to render bug in Unity
        Close();
      }
    }

    void UpdateResults(IHasteResult[] updatedResults) {
      results = updatedResults;
      highlightedIndex = 0;
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
      OnEvent(Event.current);

      DrawQuery();

      if (UnityEngine.GUI.changed) {
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
