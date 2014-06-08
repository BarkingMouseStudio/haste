using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public delegate void SceneChangedHandler(string currentScene, string previousScene);
  public delegate void SelectionChangedHandler();

  [InitializeOnLoad]
  public static class Haste {

    public static bool IsApplicationBusy {
      get {
        return !Enabled ||
               EditorApplication.isPlayingOrWillChangePlaymode ||
               EditorApplication.isCompiling ||
               EditorApplication.isUpdating ||
               EditorApplication.isPlaying ||
               EditorApplication.isPaused;
      }
    }

    public static bool Enabled {
      get { return EditorPrefs.GetBool("HasteEnabled", true); }
      set { EditorPrefs.SetBool("HasteEnabled", value); }
    }

    public static int UsageCount {
      get { return EditorPrefs.GetInt("HasteUsageCount", 0); }
      set { EditorPrefs.SetInt("HasteUsageCount", value); }
    }

    public static int IndexSize {
      get { return Index.Count; }
    }

    public static int IndexingCount {
      get { return Watchers.IndexingCount; }
    }

    public static bool IsIndexing {
      get { return Watchers.IsIndexing; }
    }

    public static event SceneChangedHandler SceneChanged;
    // public static event SelectionChangedHandler SelectionChanged;

    static string currentScene;
    static bool isCompiling = false;
    static int activeInstanceId;

    public static HasteScheduler Scheduler;
    public static HasteIndex Index;
    public static HasteWatcherManager Watchers;
    public static HasteTypeManager Types;

    static Haste() {
      currentScene = EditorApplication.currentScene;
      isCompiling = EditorApplication.isCompiling;

      Scheduler = new HasteScheduler();
      Index = new HasteIndex();
      Watchers = new HasteWatcherManager();
      Types = new HasteTypeManager();

      Types.AddType(HasteProjectSource.NAME, (HasteItem item, float score, List<int> indices) => {
        return new HasteProjectResult(item, score, indices);
      });

      Types.AddType(HasteHierarchySource.NAME, (HasteItem item, float score, List<int> indices) => {
        return new HasteHierarchyResult(item, score, indices);
      });

      Watchers.AddSource(HasteProjectSource.NAME, () => new HasteProjectSource());
      Watchers.AddSource(HasteHierarchySource.NAME, () => new HasteHierarchySource());

      #if IS_HASTE_PRO
        Types.AddType(HasteMenuItemSource.NAME, (HasteItem item, float score, List<int> indices) => {
          return new HasteMenuItemResult(item, score, indices);
        });

        Watchers.AddSource(HasteMenuItemSource.NAME, () => {
          return new HasteMenuItemSource();
        });
      #endif

      EditorApplication.projectWindowChanged += ProjectWindowChanged;
      EditorApplication.hierarchyWindowChanged += HierarchyWindowChanged;

      Haste.SceneChanged += HandleSceneChanged;
      // Haste.SelectionChanged += HandleSelectionChanged;

      EditorApplication.update += Update;
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

    // static void HandleSelectionChanged() {
    //   if (Selection.activeObject) {
    //     var obj = Selection.activeObject;
    //     HasteLogger.Info(obj, obj.GetType());
    //   }
    // }

    public static void Rebuild() {
      Index.Clear();
      Watchers.RestartAll();
    }

    static void OnSceneChanged(string currentScene, string previousScene) {
      if (SceneChanged != null) {
        SceneChanged(currentScene, previousScene);
      }
    }

    static void OnScriptsCompiled() {
      #if IS_HASTE_PRO
        Watchers.RestartSource(HasteMenuItemSource.NAME);
      #endif
    }

    // static void OnSelectionChanged() {
    //   if (SelectionChanged != null) {
    //     SelectionChanged();
    //   }
    // }

    static void Update() {
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

      // if (activeInstanceId != Selection.activeInstanceID) {
      //   activeInstanceId = Selection.activeInstanceID;
      //   OnSelectionChanged();
      // }

      if (!IsApplicationBusy) {
        Scheduler.Tick();
      }
    }
  }
}
