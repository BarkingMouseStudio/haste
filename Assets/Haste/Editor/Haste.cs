using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Haste {

  public delegate void SceneChangedHandler(string currentScene, string previousScene);
  public delegate void SelectionChangedHandler();

  public delegate void HasteWindowAction();

  [InitializeOnLoad]
  public static class Haste {

    public static readonly string VERSION = "1.6.1";
    public static readonly string ASSET_STORE_PRO_URL = "content/18584";
    // public static readonly string DEFAULT_SHORTCUT = "%k";

    public static event SceneChangedHandler SceneChanged;
    public static event SelectionChangedHandler SelectionChanged;

    public static HasteScheduler Scheduler;
    public static HasteIndex Index;
    public static HasteWatcherManager Watchers;

    internal static event HasteWindowAction WindowAction;

    static string currentScene;
    static bool isCompiling = false;
    static int activeInstanceID;
    // static object prefKey;

    public static bool IsApplicationBusy {
      get {
        var willPlay = EditorApplication.isPlayingOrWillChangePlaymode &&
          !EditorApplication.isPlaying;

        return !HasteSettings.Enabled ||
               willPlay ||
               EditorApplication.isCompiling ||
               EditorApplication.isUpdating;
      }
    }

    public static int IndexedCount {
      get { return Watchers.IndexedCount; }
    }

    public static int IndexingCount {
      get { return Watchers.IndexingCount; }
    }

    public static bool IsIndexing {
      get { return Watchers.IsIndexing; }
    }

    static Haste() {
      // prefKey = HasteReflection.Instantiate(HasteReflection.EditorAssembly, "UnityEditor.PrefKey", "Window/Haste", DEFAULT_SHORTCUT);

      currentScene = EditorApplication.currentScene;
      isCompiling = EditorApplication.isCompiling;

      Scheduler = new HasteScheduler();
      Index = new HasteIndex();
      Watchers = new HasteWatcherManager();

      Watchers.AddSource(HasteProjectSource.NAME,
        EditorPrefs.GetBool(HasteSettings.GetPrefKey(HasteSetting.Source, HasteProjectSource.NAME), true),
        () => new HasteProjectSource());
      Watchers.AddSource(HasteHierarchySource.NAME,
        EditorPrefs.GetBool(HasteSettings.GetPrefKey(HasteSetting.Source, HasteHierarchySource.NAME), true),
        () => new HasteHierarchySource());
      Watchers.AddSource(HasteMenuItemSource.NAME,
        EditorPrefs.GetBool(HasteSettings.GetPrefKey(HasteSetting.Source, HasteMenuItemSource.NAME), true),
        () => new HasteMenuItemSource());

      HasteSettings.ChangedBool += BoolSettingChanged;
      HasteSettings.ChangedString += StringSettingChanged;
      EditorApplication.projectWindowChanged += ProjectWindowChanged;
      EditorApplication.hierarchyWindowChanged += HierarchyWindowChanged;

      SceneChanged += HandleSceneChanged;
      EditorApplication.update += Update;

      // AddGlobalEventHandler();

      if (HasteSettings.UsageSince == 0L) {
        HasteSettings.UsageSince = DateTime.Now.Ticks;
      }

      HasteSettings.Version = VERSION;
    }

    // static void AddGlobalEventHandler() {
    //   var fieldInfo = typeof(EditorApplication).GetField("globalEventHandler", BindingFlags.NonPublic|BindingFlags.Static);

    //   var origHandler = (EditorApplication.CallbackFunction)fieldInfo.GetValue(null);
    //   var newHandler = new EditorApplication.CallbackFunction(GlobalEventHandler);
    //   fieldInfo.SetValue(null, Delegate.Combine(
    //     origHandler,
    //     newHandler
    //   ));
    // }

    static void BoolSettingChanged(HasteSetting setting, bool before, bool after) {
      switch (setting) {
        case HasteSetting.Enabled:
          if (after) {
            Rebuild();
          } else {
            Stop();
          }
          break;
      }
    }

    static void StringSettingChanged(HasteSetting setting, string before, string after) {
      switch (setting) {
        case HasteSetting.Version:
          HasteSettings.UsageCount = 0;
          HasteSettings.UsageSince = DateTime.Now.Ticks;
          Rebuild();
          break;
      }
    }

    static void ProjectWindowChanged() {
      Watchers.RestartSource(HasteProjectSource.NAME);
    }

    static void HierarchyWindowChanged() {
      Watchers.RestartSource(HasteHierarchySource.NAME);
    }

    static void HandleSceneChanged(string currentScene, string previousScene) {
      Watchers.RestartSource(HasteHierarchySource.NAME);
    }

    public static void Rebuild() {
      Index.Clear();
      Watchers.Rebuild();
    }

    public static void Stop() {
      Index.Clear();
      Watchers.Stop();
    }

    static void OnSceneChanged(string currentScene, string previousScene) {
      if (SceneChanged != null) {
        SceneChanged(currentScene, previousScene);
      }
    }

    static void OnSelectionChanged() {
      if (SelectionChanged != null) {
        SelectionChanged();
      }
    }

    static void OnScriptsCompiled() {
      #if IS_HASTE_PRO
        Watchers.RestartSource(HasteMenuItemSource.NAME);
      #endif
    }

    // static void HasteShortcutHandler() {
    //   if (HasteReflection.GetPropValue<bool>(prefKey, "activated")) {
    //     Event.current.Use();
    //     HasteWindow.Open();
    //   }
    // }

    // static void GlobalEventHandler() {
    //   HasteShortcutHandler();
    // }

    // Main update loop in Haste—run's scheduler
    static void Update() {
      // We must delay the window action to handle actions
      // that affect layout state to prevent bugs in Unity.
      if (WindowAction != null && HasteWindow.Instance == null) {
        WindowAction();
        WindowAction = null;
      }

      // Compiling state changed
      if (isCompiling != EditorApplication.isCompiling) {
        isCompiling = EditorApplication.isCompiling;

        // Done compiling
        if (!isCompiling) {
          OnScriptsCompiled();
        }
      }

      if (currentScene != EditorApplication.currentScene) {
        string previousScene = currentScene;
        currentScene = EditorApplication.currentScene;
        OnSceneChanged(currentScene, previousScene);
      }

      if (activeInstanceID != Selection.activeInstanceID) {
        activeInstanceID = Selection.activeInstanceID;
        OnSelectionChanged();
      }

      if (!IsApplicationBusy) {
        Scheduler.Tick();
      }
    }
  }
}
