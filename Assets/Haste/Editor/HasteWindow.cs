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

    const int WINDOW_WIDTH = 500;
    const int WINDOW_HEIGHT = 300;

    const int RESULT_COUNT = 25;

    [SerializeField]
    HasteGUIBackground background;

    [SerializeField]
    HasteGUIQuery queryInput;

    [SerializeField]
    HasteGUIEmpty empty;

    [SerializeField]
    HasteGUIIntro intro;

    [SerializeField]
    HasteGUIList<IHasteResult> resultList;

    public static HasteWindow Instance { get; protected set; }

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
      // Increment open count
      HasteSettings.UsageCount += 1;

      // Save the current selection
      HasteSelectionManager.Save();

      HasteWindow.Instance = EditorWindow.CreateInstance<HasteWindow>();
      HasteWindow.Instance.InitializeInstance();
    }

    void InitializeInstance() {
      this.title = "Haste";

      this.position = new Rect(
        (Screen.currentResolution.width - HasteWindow.WINDOW_WIDTH) / 2,
        (Screen.currentResolution.height - HasteWindow.WINDOW_HEIGHT) / 2,
        HasteWindow.WINDOW_WIDTH, HasteWindow.WINDOW_HEIGHT
      );

      // Disable the resize handle on the window
      this.minSize = this.maxSize = new Vector2(HasteWindow.WINDOW_WIDTH, HasteWindow.WINDOW_HEIGHT);

      this.queryInput = ScriptableObject.CreateInstance<HasteGUIQuery>();
      this.queryInput.Changed += OnQueryChanged;

      this.background = ScriptableObject.CreateInstance<HasteGUIBackground>();
      this.resultList = ScriptableObject.CreateInstance<HasteGUIList<IHasteResult>>();

      this.intro = ScriptableObject.CreateInstance<HasteGUIIntro>();
      this.empty = ScriptableObject.CreateInstance<HasteGUIEmpty>();
      this.intro.tip = this.empty.tip = HasteTips.Random;

      ShowPopup();
      Focus();
    }

    void OnEscape() {
      HasteSelectionManager.Restore();
      Close();
    }

    void OnReturn() {
      this.resultList.HighlightedItem.Action();
      Close();
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
      }
    }

    void OnEvent(Event e) {
      switch (e.type) {
        case EventType.KeyDown:
          OnKeyDown(e);
          break;
      }
    }

    void Update() {
      if (Haste.IsIndexing && this.queryInput.Query == "") {
        // This is here to repaint the indexing count
        Repaint();
      }

      if (this != EditorWindow.focusedWindow) {
        // Check if we lost focus and close:
        // Cannot use OnLostFocus due to render bug in Unity
        HasteSelectionManager.Restore();
        Close();
      }
    }

    void OnQueryChanged(string query) {
      if (query == "") {
        this.resultList.Items = new IHasteResult[0];
      } else {
        this.resultList.Items = Haste.Index.Filter(query, RESULT_COUNT);
      }
    }

    void OnGUI() {
      OnEvent(Event.current);

      this.background.OnGUI();
      this.queryInput.OnGUI();

      if (this.queryInput.Query == "") {
        this.intro.OnGUI();
      } else if (this.resultList.Size == 0) {
        this.empty.OnGUI();
      } else {
        this.resultList.OnGUI();
      }
    }
  }
}
