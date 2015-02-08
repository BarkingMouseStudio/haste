using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Haste {

  public enum HasteWindowState {
    Intro,
    Empty,
    Results
  }

  [Serializable]
  public class HasteWindow : EditorWindow {

    const int RESULT_COUNT = 25;

    HasteWindowState windowState = HasteWindowState.Intro;

    // [SerializeField]
    // HasteBackground background;

    [SerializeField]
    HasteQuery queryInput;

    [SerializeField]
    HasteEmpty empty;

    [SerializeField]
    HasteIntro intro;

    [SerializeField]
    HasteList resultList;

    public static HasteWindow Instance { get; protected set; }

    // internal static void CallDelayed (EditorApplication.CallbackFunction function, float timeFromNow)
    // {
    //   EditorApplication.delayedCallback = function;
    //   EditorApplication.s_DelayedCallbackTime = Time.realtimeSinceStartup + timeFromNow;
    //   EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine (EditorApplication.update, new EditorApplication.CallbackFunction (EditorApplication.CheckCallDelayed));
    // }

    // private static void CheckCallDelayed ()
    // {
    //   if (Time.realtimeSinceStartup > EditorApplication.s_DelayedCallbackTime)
    //   {
    //     EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove (EditorApplication.update, new EditorApplication.CallbackFunction (EditorApplication.CheckCallDelayed));
    //     EditorApplication.delayedCallback ();
    //   }
    // }

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
      HasteSettings.UsageCount++;

      // Save the current selection
      HasteSelectionManager.Save();

      HasteWindow.Instance = EditorWindow.CreateInstance<HasteWindow>();
      HasteWindow.Instance.InitializeInstance();

      HasteWindow.Instance.ShowPopup();
      HasteWindow.Instance.Focus();
    }

    void InitializeInstance() {
      this.title = "Haste";

      this.position = new Rect(
        (Screen.currentResolution.width - HasteStyles.WindowWidth) / 2,
        (Screen.currentResolution.height - HasteStyles.WindowHeight) / 2,
        HasteStyles.WindowWidth, HasteStyles.WindowHeight
      );

      // Disable the resize handle on the window
      this.minSize = this.maxSize =
        new Vector2(this.position.width, this.position.height);

      this.queryInput = ScriptableObject.CreateInstance<HasteQuery>();
      this.queryInput.Changed += OnQueryChanged;

      // this.background = ScriptableObject.CreateInstance<HasteBackground>()
      //   .Init(new Rect(0, 0, this.position.width, this.position.height));
      // this.background.Capture(this.position);

      this.resultList = ScriptableObject.CreateInstance<HasteList>();
      // this.resultList.OnSelect += OnReturn;

      var tip = HasteTips.Random;
      this.intro = ScriptableObject.CreateInstance<HasteIntro>().Init(tip);
      this.empty = ScriptableObject.CreateInstance<HasteEmpty>().Init(tip);
    }

    void OnEscape() {
      HasteSelectionManager.Restore();
      Close();
    }

    void OnReturn() {
      if (this.resultList.HighlightedItem != null) {
        // Restore selection in case the action affects
        // the original selection.
        HasteSelectionManager.Restore();

        // Register action to occur after the window is closed and destroyed.
        // This is done to prevent errors when modifying window layouts and
        // other Unity state while Haste is open.
        Haste.WindowAction += this.resultList.HighlightedItem.Action;
      }
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
        case KeyCode.UpArrow:
          e.Use();
          this.resultList.OnUpArrow();
          break;
        case KeyCode.DownArrow:
          e.Use();
          this.resultList.OnDownArrow();
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
      if (Haste.IsIndexing && this.windowState == HasteWindowState.Intro) {
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

      // this.background.OnGUI();
      this.queryInput.OnGUI();

      if (this.queryInput.Query == "") {
        this.windowState = HasteWindowState.Intro;
      } else if (this.resultList.Size == 0) {
        this.windowState = HasteWindowState.Empty;
      } else {
        this.windowState = HasteWindowState.Results;
      }

      switch (this.windowState) {
        case HasteWindowState.Intro:
          this.intro.OnGUI();
          break;
        case HasteWindowState.Empty:
          this.empty.OnGUI();
          break;
        case HasteWindowState.Results:
          this.resultList.OnGUI();
          break;
      }
    }
  }
}
