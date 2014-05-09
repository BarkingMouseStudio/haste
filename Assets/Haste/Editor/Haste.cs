#define IS_PRO

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public delegate void SceneChangedHandler(string currentScene, string previousScene);

  [InitializeOnLoad]
  public static class Haste {

    public static event SceneChangedHandler SceneChanged;

    public static HasteScheduler Scheduler;

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

    public static bool IsApplicationBusy {
      get {
        return EditorApplication.isPlayingOrWillChangePlaymode ||
               EditorApplication.isCompiling ||
               EditorApplication.isUpdating ||
               EditorApplication.isPlaying ||
               EditorApplication.isPaused;
      }
    }

    public static HasteIndex Index;
    static IHasteWatcher projectWatcher;
    static IHasteWatcher hierarchyWatcher;

    static string currentScene;

    static Haste() {
      currentScene = EditorApplication.currentScene;

      Scheduler = new HasteScheduler();
      Index = new HasteIndex();

      projectWatcher = new HasteProjectWatcher();
      projectWatcher.Created += AddToIndex;
      projectWatcher.Deleted += RemoveFromIndex;
      projectWatcher.Start();

      hierarchyWatcher = new HasteHierarchyWatcher();
      hierarchyWatcher.Created += AddToIndex;
      hierarchyWatcher.Deleted += RemoveFromIndex;
      hierarchyWatcher.Start();

      EditorApplication.update += Update;
      EditorApplication.playmodeStateChanged += PlaymodeStateChanged;
    }

    public static void Rebuild() {
      Scheduler.Stop();

      Index.Clear();

      hierarchyWatcher.Reset();
      projectWatcher.Reset();

      hierarchyWatcher.Restart();
      projectWatcher.Restart();
    }

    static void AddToIndex(HasteItem item) {
      Index.Add(item);
    }

    static void RemoveFromIndex(HasteItem item) {
      Index.Remove(item);
    }

    static void PlaymodeStateChanged() {
      Scheduler.Stop();
      // TODO: Restart the watchers
    }

    static void OnSceneChanged(string currentScene, string previousScene) {
      if (SceneChanged != null) {
        SceneChanged(currentScene, previousScene);
      }
    }

    static void Update() {
      if (IsApplicationBusy) {
        return;
      }

      if (currentScene != EditorApplication.currentScene) {
        string previousScene = currentScene;
        currentScene = EditorApplication.currentScene;
        OnSceneChanged(currentScene, previousScene);
      }

      hierarchyWatcher.Tick();
      projectWatcher.Tick();

      Scheduler.Tick();
    }
  }
}