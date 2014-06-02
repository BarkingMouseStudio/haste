using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public delegate void SceneChangedHandler(string currentScene, string previousScene);

  [InitializeOnLoad]
  public static class Haste {

    public static bool IsApplicationBusy {
      get {
        return EditorApplication.isPlayingOrWillChangePlaymode ||
               EditorApplication.isCompiling ||
               EditorApplication.isUpdating ||
               EditorApplication.isPlaying ||
               EditorApplication.isPaused;
      }
    }

    private static int usageCount = -1;
    public static int UsageCount {
      get {
        if (usageCount < 0) {
          usageCount = EditorPrefs.GetInt("HasteUsageCount", 0);
        }
        return usageCount;
      }
      set {
        usageCount = value;
        EditorPrefs.SetInt("HasteUsageCount", usageCount);
      }
    }

    public static event SceneChangedHandler SceneChanged;

    static string currentScene;

    public static HasteScheduler Scheduler;
    public static HasteIndex Index;
    public static HasteWatcherManager Watchers;
    public static HasteTypeManager Types;

    static Haste() {
      currentScene = EditorApplication.currentScene;

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

        Types.AddType(HasteProjectActionSource.NAME, (HasteItem item, float score, List<int> indices) => {
          return new HasteResult(item, score, indices);
        });

        Types.AddType(HasteHierarchyActionSource.NAME, (HasteItem item, float score, List<int> indices) => {
          return new HasteResult(item, score, indices);
        });

        Watchers.AddSource(HasteMenuItemSource.NAME, () => new HasteMenuItemSource());
        Watchers.AddSource(HasteProjectActionSource.NAME, () => new HasteProjectActionSource());
        Watchers.AddSource(HasteHierarchyActionSource.NAME, () => new HasteHierarchyActionSource());
      #endif

      EditorApplication.projectWindowChanged += ProjectWindowChanged;
      EditorApplication.hierarchyWindowChanged += HierarchyWindowChanged;

      Haste.SceneChanged += HandleSceneChanged;

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

    public static void Rebuild() {
      Index.Clear();
      Watchers.RestartAll();
    }

    static void OnSceneChanged(string currentScene, string previousScene) {
      if (SceneChanged != null) {
        SceneChanged(currentScene, previousScene);
      }
    }

    static void Update() {
      if (currentScene != EditorApplication.currentScene) {
        string previousScene = currentScene;
        currentScene = EditorApplication.currentScene;
        OnSceneChanged(currentScene, previousScene);
      }

      if (!IsApplicationBusy) {
        Scheduler.Tick();
      }
    }
  }
}
