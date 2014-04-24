using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteWindow : EditorWindow {

    static HasteWindow instance;

    HasteResult[] results = new HasteResult[0];

    GUIStyle queryStyle;
    GUIStyle nameStyle;
    GUIStyle descriptionStyle;
    GUIStyle prefixStyle;
    GUIStyle highlightStyle;

    Vector2 scrollPosition = Vector2.zero;

    string query = "";

    int highlightedIndex = 0;

    const int itemHeight = 44;
    const int prefixWidth = 155;
    const int groupSpacing = 10;

    const int resultCount = 3;

    bool displayActions = false;

    [MenuItem("Window/Haste %p")]
    public static void Open() {
      if (instance == null) {
        instance = EditorWindow.CreateInstance<HasteWindow>();

        instance.queryStyle = new GUIStyle(EditorStyles.textField);
        instance.queryStyle.fixedHeight = 64;
        instance.queryStyle.alignment = TextAnchor.MiddleLeft;
        instance.queryStyle.fontSize = 32;

        instance.nameStyle = new GUIStyle(EditorStyles.largeLabel);
        instance.nameStyle.alignment = TextAnchor.MiddleLeft;
        instance.nameStyle.fixedHeight = 24;
        instance.nameStyle.fontSize = 16;
        instance.nameStyle.hover.textColor = Color.white;
        instance.nameStyle.onHover.textColor = Color.white;

        instance.descriptionStyle = new GUIStyle(EditorStyles.largeLabel);
        instance.descriptionStyle.alignment = TextAnchor.MiddleLeft;
        instance.descriptionStyle.fixedHeight = 24;
        instance.descriptionStyle.fontSize = 12;

        instance.prefixStyle = new GUIStyle(EditorStyles.largeLabel);
        instance.prefixStyle.alignment = TextAnchor.MiddleRight;
        instance.prefixStyle.fixedHeight = 18;
        instance.prefixStyle.fontSize = 12;

        instance.highlightStyle = new GUIStyle(EditorStyles.whiteLargeLabel);
        instance.highlightStyle.alignment = TextAnchor.MiddleLeft;
        instance.highlightStyle.fixedHeight = 24;
        instance.highlightStyle.fontSize = 16;

        int width = 500;
        int height = 300;
        int x = (Screen.currentResolution.width - width) / 2;
        int y = (Screen.currentResolution.height - height) / 2;
        instance.position = new Rect(x, y, width, height);
        instance.minSize = instance.maxSize = new Vector2(instance.position.width, instance.position.height);
        instance.title = "Haste";
      }

      instance.ShowPopup();
      instance.Reset();
      instance.Focus();
    }

    void Reset() {
      highlightedIndex = 0;
      displayActions = false;
    }

    // On ESC, clear the query or close the window
    void OnEscape() {
      if (query == "") {
        Close();
      } else {
        query = "";
      }
    }

    void OnReturn() {
      if (results.Length > 0) {
        OnResultSelected(results[highlightedIndex]);
      }
    }

    void OnLeftArrow() {
      if (results.Length > 0) {
        displayActions = false;
      }
    }

    void OnRightArrow() {
      if (results.Length > 0) {
        displayActions = true;
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
        if (i > 0 && results[i].Source != results[i - 1].Source) {
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
          OnEscape();
          break;
        case KeyCode.Return:
          OnReturn();
          break;
        case KeyCode.UpArrow:
          OnUpArrow();
          break;
        case KeyCode.DownArrow:
          OnDownArrow();
          break;
        case KeyCode.LeftArrow:
          OnLeftArrow();
          break;
        case KeyCode.RightArrow:
          OnRightArrow();
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

    void OnResultSelected(HasteResult result) {
      HasteActions.DefaultAction(result);
      Close();
    }

    void DrawResult(HasteResult result, int index) {
      var rect = EditorGUILayout.BeginHorizontal();

      if (GUI.Button(rect, "", GUIStyle.none)) {
        OnResultSelected(result);
      }

      Texture icon = AssetDatabase.GetCachedIcon(result.Path);
      if (icon != null) {
        GUI.DrawTexture(
          EditorGUILayout.GetControlRect(GUILayout.Width(32), GUILayout.Height(32)),
          icon);
      }

      EditorGUILayout.BeginVertical();
      EditorGUILayout.LabelField(Path.GetFileName(result.Path), index == highlightedIndex ? highlightStyle : nameStyle);
      EditorGUILayout.LabelField(result.Path, descriptionStyle);
      EditorGUILayout.EndVertical();

      EditorGUILayout.EndHorizontal();
      EditorGUILayout.Space();
    }

    void DrawAction(HasteAction action, int index) {

    }

    void DrawActions() {
      scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,
        GUILayout.ExpandWidth(true),
        GUILayout.ExpandHeight(true));

      for (int i = 0; i < results.Length; i++) {
        DrawResult(results[i], i);
      }

      EditorGUILayout.EndScrollView();
    }

    void DrawResults() {
      scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,
        GUILayout.ExpandWidth(true),
        GUILayout.ExpandHeight(true));

      for (int i = 0; i < results.Length; i++) {
        HasteResult result = results[i];

        bool isBeginGroup = false;
        if (i == 0) {
          isBeginGroup = true;
        } else {
          HasteResult prevResult = results[i - 1];
          if (result.Source != prevResult.Source) {
            isBeginGroup = true;
          }
        }

        if (isBeginGroup) {
          // Begin group
          EditorGUILayout.Space();
          EditorGUILayout.BeginHorizontal();
          EditorGUILayout.LabelField(
            Enum.GetName(typeof(HasteSource), result.Source),
            prefixStyle,
            GUILayout.Width(prefixWidth));
          EditorGUILayout.BeginVertical();
        }

        DrawResult(result, i);

        bool isEndGroup = false;
        if (i == results.Length - 1) {
          isEndGroup = true;
        } else {
          HasteResult nextResult = results[i + 1];
          if (result.Source != nextResult.Source) {
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

      EditorGUILayout.EndScrollView();
    }

    void DrawQuery() {
      GUI.SetNextControlName("query");
      query = EditorGUILayout.TextField(query, queryStyle,
        GUILayout.Height(instance.queryStyle.fixedHeight));
      EditorGUI.FocusTextInControl("query");
    }

    void Update() {
      if (this != EditorWindow.focusedWindow) {
        // Check if we lost focus and close:
        // Cannot use OnLostFocus due to render bug in Unity
        Close();
      }
    }

    void OnGUI() {
      OnEvent(Event.current);

      DrawQuery();

      if (GUI.changed) {
        Reset();
        results = Haste.Index.Filter(query, resultCount);
      }

      if (results != null && results.Length > 0) {
        if (displayActions) {
          DrawActions();
        } else {
          DrawResults();
        }
      }
    }
  }
}
