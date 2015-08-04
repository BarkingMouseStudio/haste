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

    public static readonly string VERSION = "1.8.5";

    private static Version version;
    public static Version Version {
      get {
        if (version == null) {
          version = new Version(VERSION);
        }
        return version;
      }
    }

    public static readonly string ASSET_STORE_PRO_URL = "content/18584";
    public static readonly string ASSET_STORE_FREE_URL = "content/28390";
    // public static readonly string DEFAULT_SHORTCUT = "%k";

    public static event SceneChangedHandler SceneChanged;
    public static event SelectionChangedHandler SelectionChanged;

    public static HasteScheduler Scheduler;
    public static HasteIndex Index;
    public static HasteSearch Search;
    public static HasteWatcherManager Watchers;

    #if IS_HASTE_PRO
      public static HasteRecommendations Recommendations;
    #endif

    public static HasteUpdateChecker UpdateChecker;

    internal static event HasteWindowAction WindowAction;

    static string currentScene;
    static bool isCompiling = false;
    static int activeInstanceID;
    // static object prefKey;

    static double layoutInterval = 30.0;
    static double lastLayoutCheck = 0.0;

    public static bool IsApplicationBusy {
      get {
        var willPlay = EditorApplication.isPlayingOrWillChangePlaymode &&
          !EditorApplication.isPlaying;

        return !HasteSettings.Enabled ||
               willPlay ||
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
      Search = new HasteSearch(Index);
      Watchers = new HasteWatcherManager();

      #if IS_HASTE_PRO
        Recommendations = HasteRecommendations.Load();
      #endif

      Watchers.AddSource(HasteProjectSource.NAME,
        EditorPrefs.GetBool(HasteSettings.GetPrefKey(HasteSetting.Source, HasteProjectSource.NAME), true),
        () => new HasteProjectSource());
      Watchers.AddSource(HasteHierarchySource.NAME,
        EditorPrefs.GetBool(HasteSettings.GetPrefKey(HasteSetting.Source, HasteHierarchySource.NAME), true),
        () => new HasteHierarchySource());
      Watchers.AddSource(HasteMenuItemSource.NAME,
        EditorPrefs.GetBool(HasteSettings.GetPrefKey(HasteSetting.Source, HasteMenuItemSource.NAME), true),
        () => new HasteMenuItemSource());
      Watchers.AddSource(HasteLayoutSource.NAME,
        EditorPrefs.GetBool(HasteSettings.GetPrefKey(HasteSetting.Source, HasteLayoutSource.NAME), true),
        () => new HasteLayoutSource());

      lastLayoutCheck = EditorApplication.timeSinceStartup;

      UpdateChecker = new HasteUpdateChecker();

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

      // Pre-load icons and styles
      HasteHierarchyResult.LoadGameObjectIcon();
      Scheduler.Start(HasteStyles.Init());
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

    // The maximum time an iteration can spend working per update
    public const float MAX_ITER_TIME = 16.0f / 1000.0f;

    // Main update loop in Haste—run's scheduler
    static void Update() {
      if (HasteSettings.CheckForUpdates) {
        if (DateTime.Now >= HasteSettings.LastUpdateCheckDate.Add(UpdateChecker.Interval)) {
          Scheduler.Start(UpdateChecker.Check());
        }
      }

      // We must delay the window action to handle actions
      // that affect layout state to prevent bugs in Unity.
      if (WindowAction != null && HasteWindow.Instance == null) {
        try {
          WindowAction();
        } finally {
          WindowAction = null;
        }
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
        // Check layouts folder every so often
        double now = EditorApplication.timeSinceStartup;
        if (now - lastLayoutCheck > layoutInterval) {
          lastLayoutCheck = now;
          Watchers.RestartSource(HasteLayoutSource.NAME);
        }

        var start = EditorApplication.timeSinceStartup;
        var duration = 0.0;

        while (duration < MAX_ITER_TIME && Scheduler.IsRunning) {
          Scheduler.Tick();
          duration += (EditorApplication.timeSinceStartup - start);
        }
      }
    }
  }
}
