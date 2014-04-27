using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteWindow : EditorWindow {

    static HasteWindow instance;

    HasteResult[] results = new HasteResult[0];
    HasteResult selectedResult;

    HasteAction[] actions = new HasteAction[0];

    GUIStyle queryStyle;
    GUIStyle nameStyle;
    GUIStyle descriptionStyle;
    GUIStyle prefixStyle;
    GUIStyle highlightStyle;

    Vector2 scrollPosition = Vector2.zero;

    string query = "";

    int highlightedIndex = 0;

    const int itemHeight = 44;
    const int prefixWidth = 96;
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
      instance.HideActions();
      instance.Focus();
      instance.PrefillResults();
    }

    void PrefillResults() {
      results = HasteUtils.GetResultsFromObjects(Selection.objects);
    }

    void HideActions() {
      highlightedIndex = 0;
      displayActions = false;
    }

    void ShowActions() {
      highlightedIndex = 0;
      displayActions = true;
      query = "";
    }

    void OnEscape() {
      // On escape, backtrack out of actions, clear the query or close the window
      if (displayActions) {
        HideActions();
      } else if (query != "") {
        query = "";
      } else {
        Close();
      }
    }

    void OnReturn() {
      if (displayActions) {
        OnActionSelected(actions[highlightedIndex]);
      } else {
        if (!displayActions && results.Length > 0) {
          OnResultSelected(results[highlightedIndex]);
        }
      }
    }

    void OnRightArrow() {
      if (displayActions || results.Length == 0) {
        return;
      }

      selectedResult = results[highlightedIndex];

      HasteActions.SelectByResult(selectedResult);

      actions = HasteActions.GetActionsForSource(selectedResult.Source);

      ShowActions();
    }

    void OnUpArrow() {
      highlightedIndex = Math.Max(highlightedIndex - 1, 0);
      UpdateScroll();
    }

    void OnDownArrow() {
      if (displayActions) {
        highlightedIndex = Math.Min(highlightedIndex + 1, actions.Length - 1);
      } else {
        highlightedIndex = Math.Min(highlightedIndex + 1, results.Length - 1);
      }
      UpdateScroll();
    }

    void UpdateScroll() {
      int previousGroups = 0;
      if (!displayActions) {
        for (int i = 0; i <= highlightedIndex; i++) {
          if (i > 0 && results[i].Source != results[i - 1].Source) {
            previousGroups++;
          }
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
        case KeyCode.RightArrow:
          e.Use();
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
      Close();
      HasteActions.SelectByResult(result);
      HasteActions.FocusByResult(result);
    }

    void DrawResult(HasteResult result, int index) {
      var rect = EditorGUILayout.BeginHorizontal();

      if (GUI.Button(rect, "", GUIStyle.none)) {
        OnResultSelected(result);
        return;
      }

      Texture icon = HasteUtils.GetIconForSource(result.Source, result.Path);

      if (icon != null) {
        GUI.DrawTexture(EditorGUILayout.GetControlRect(GUILayout.Width(32), GUILayout.Height(32)), icon);
      }

      EditorGUILayout.BeginVertical();
      EditorGUILayout.LabelField(Path.GetFileName(result.Path), index == highlightedIndex ? highlightStyle : nameStyle);
      EditorGUILayout.LabelField(String.Format("({0}) {1}", result.Score.ToString(), result.Path), descriptionStyle);
      EditorGUILayout.EndVertical();

      EditorGUILayout.EndHorizontal();
      EditorGUILayout.Space();
    }

    void OnActionSelected(HasteAction action) {
      // We close the window first in case an action creates a dialog
      Close();
      action.Action(selectedResult);
    }

    void DrawAction(HasteAction action, int index) {
      var rect = EditorGUILayout.BeginHorizontal();

      if (GUI.Button(rect, "", GUIStyle.none)) {
        OnActionSelected(action);
        return;
      }

      EditorGUILayout.BeginVertical();
      EditorGUILayout.LabelField(action.Name, index == highlightedIndex ? highlightStyle : nameStyle);
      EditorGUILayout.LabelField(action.Description, descriptionStyle);
      EditorGUILayout.EndVertical();

      EditorGUILayout.EndHorizontal();
      EditorGUILayout.Space();
    }

    void DrawActions() {
      scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,
        GUILayout.ExpandWidth(true),
        GUILayout.ExpandHeight(true));

      for (int i = 0; i < actions.Length; i++) {
        DrawAction(actions[i], i);
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
      query = query.Trim();
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
        if (displayActions) {
          actions = HasteActions.GetActionsForSource(selectedResult.Source).FuzzyFilter(query);
        } else {
          results = Haste.Index.Filter(query, resultCount);
        }
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
