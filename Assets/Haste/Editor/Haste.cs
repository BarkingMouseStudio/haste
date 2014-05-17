#define IS_HASTE_PRO

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

    public static event SceneChangedHandler SceneChanged;

    static string currentScene;

    public static HasteScheduler Scheduler;
    public static HasteIndex Index;

    static HasteWatcherManager watchers;

    static Haste() {
      currentScene = EditorApplication.currentScene;

      Scheduler = new HasteScheduler();
      Index = new HasteIndex();

      watchers = new HasteWatcherManager();

      watchers.AddSource("Project", () => new HasteProjectSource());
      watchers.AddSource("Hierarchy", () => new HasteHierarchySource());

      #if IS_HASTE_PRO
        watchers.AddSource("MenuItems", () => new HasteMenuItemSource());
      #endif

      EditorApplication.projectWindowChanged += ProjectWindowChanged;
      EditorApplication.hierarchyWindowChanged += HierarchyWindowChanged;

      Haste.SceneChanged += HandleSceneChanged;

      EditorApplication.update += Update;
    }

    static void ProjectWindowChanged() {
      watchers.RestartSource("Project");
    }

    static void HierarchyWindowChanged() {
      watchers.RestartSource("Hierarchy");
    }

    static void HandleSceneChanged(string currentScene, string previousScene) {
      watchers.RestartSource("Hierarchy");
    }

    public static void Rebuild() {
      Index.Clear();
      watchers.RestartAll();
    }

    static void OnSceneChanged(string currentScene, string previousScene) {
      if (SceneChanged != null) {
        SceneChanged(currentScene, previousScene);
      }
    }

    public static int frame = 0;

    static void Update() {
      if (currentScene != EditorApplication.currentScene) {
        string previousScene = currentScene;
        currentScene = EditorApplication.currentScene;
        OnSceneChanged(currentScene, previousScene);
      }

      if (!IsApplicationBusy) {
        Scheduler.Tick();
      }

      frame++;
    }
  }
}
