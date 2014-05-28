#define IS_HASTE_PRO

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

    static HasteWindow instance;

    HasteResult[] results = new HasteResult[0];
    HasteResult selectedResult;

    GUIStyle queryStyle;
    GUIStyle nameStyle;
    GUIStyle descriptionStyle;
    GUIStyle prefixStyle;
    GUIStyle highlightStyle;
    GUIStyle disabledNameStyle;
    GUIStyle disabledDescriptionStyle;

    Vector2 scrollPosition = Vector2.zero;

    string query = "";

    int highlightedIndex = 0;

    const int itemHeight = 44;
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
      EditorGUILayout.LabelField("Open Count", HasteUsage.Count.ToString());

      EditorGUILayout.Space();
      EditorGUILayout.HelpBox("Indicates how many times you have opened Haste.", MessageType.Info);
    }

    static void Init() {
      instance = EditorWindow.CreateInstance<HasteWindow>();

      instance.queryStyle = new GUIStyle(EditorStyles.textField);
      instance.queryStyle.fixedHeight = 64;
      instance.queryStyle.alignment = TextAnchor.MiddleLeft;
      instance.queryStyle.fontSize = 32;

      instance.nameStyle = new GUIStyle(EditorStyles.largeLabel);
      instance.nameStyle.alignment = TextAnchor.MiddleLeft;
      instance.nameStyle.fixedHeight = 24;
      instance.nameStyle.fontSize = 16;

      instance.disabledNameStyle = new GUIStyle(EditorStyles.largeLabel);
      instance.disabledNameStyle.alignment = TextAnchor.MiddleLeft;
      instance.disabledNameStyle.fixedHeight = 24;
      instance.disabledNameStyle.fontSize = 16;
      instance.disabledNameStyle.normal.textColor = new Color(0.45f, 0.45f, 0.45f);

      instance.disabledDescriptionStyle = new GUIStyle(EditorStyles.largeLabel);
      instance.disabledDescriptionStyle.alignment = TextAnchor.MiddleLeft;
      instance.disabledDescriptionStyle.fixedHeight = 24;
      instance.disabledDescriptionStyle.fontSize = 12;
      instance.disabledDescriptionStyle.normal.textColor = new Color(0.45f, 0.45f, 0.45f);

      instance.descriptionStyle = new GUIStyle(EditorStyles.largeLabel);
      instance.descriptionStyle.alignment = TextAnchor.MiddleLeft;
      instance.descriptionStyle.fixedHeight = 24;
      instance.descriptionStyle.fontSize = 12;
      instance.descriptionStyle.richText = true;

      instance.prefixStyle = new GUIStyle(EditorStyles.largeLabel);
      instance.prefixStyle.alignment = TextAnchor.MiddleRight;
      instance.prefixStyle.fixedHeight = 18;
      instance.prefixStyle.fontSize = 12;

      instance.highlightStyle = new GUIStyle(EditorStyles.whiteLargeLabel);
      instance.highlightStyle.alignment = TextAnchor.MiddleLeft;
      instance.highlightStyle.fixedHeight = 24;
      instance.highlightStyle.fontSize = 16;

      if (EditorGUIUtility.isProSkin) {
        instance.highlightStyle.normal.textColor = new Color(0.275f, 0.475f, 0.95f);
      } else {
        instance.highlightStyle.normal.textColor = new Color(0.045f, 0.22f, 0.895f);
      }

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

      HasteUsage.Count++;

      instance.ShowPopup();
      instance.Focus();

      // TODO: Populate initial results with "selectedResult" filtered actions?
    }

    void OnEscape() {
      // On escape, clear context, clear the query or close the window
      // TODO: Clear context
      if (query != "") {
        query = "";
      } else {
        Close();
      }
    }

    void OnReturn() {
      if (results.Length > 0) {
        selectedResult = results[highlightedIndex];
      }
    }

    void OnRightArrow() {
      if (results.Length > 0) {
        selectedResult = results[highlightedIndex];
        // TODO: Clear query, re-filter results
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

    string BoldLabel(string str, int[] indices) {
      string bolded = "";

      // TODO: This could iterate indices and use substring instead
      int index = 0;
      for (int i = 0; i < str.Length; i++) {
        if (index < indices.Length && i == indices[index]) {
          bolded += "<color=\"white\">" + str[i] + "</color>";
          index++;
        } else {
          bolded += str[i];
        }
      }

      return bolded;
    }

    void DrawResult(HasteResult result, int index) {
      var rect = EditorGUILayout.BeginHorizontal();

      if (GUI.Button(rect, "", GUIStyle.none)) {
        OnResultSelected(result);
      }

      if (result.Icon != null) {
        GUI.DrawTexture(EditorGUILayout.GetControlRect(GUILayout.Width(32), GUILayout.Height(32)), result.Icon);
      }

      #if IS_HASTE_PRO
        // string description = result.Description;
      #else
        string description = "Download Haste Pro from the Unity Asset Store (Disabled)";
      #endif

      EditorGUILayout.BeginVertical();
      EditorGUILayout.LabelField(Path.GetFileName(result.Path), index == highlightedIndex ? highlightStyle : nameStyle);
      EditorGUILayout.LabelField(BoldLabel(result.Path, result.Indices.ToArray()), descriptionStyle);
      EditorGUILayout.LabelField(result.Description, descriptionStyle);
      EditorGUILayout.EndVertical();

      EditorGUILayout.EndHorizontal();
      EditorGUILayout.Space();
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

    void OnGUIChanged() {
      results = Haste.Index.Filter(query, resultCount);
      highlightedIndex = 0;
    }

    void OnGUI() {
      OnEvent(Event.current);

      DrawQuery();

      if (GUI.changed) {
        OnGUIChanged();
      }

      if (results != null && results.Length > 0) {
        DrawResults();
      }
    }
  }
}
