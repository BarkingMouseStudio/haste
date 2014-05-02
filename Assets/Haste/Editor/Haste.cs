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
    static HasteFileWatcher projectWatcher;
    static HasteHierarchyWatcher hierarchyWatcher;

    static string currentScene;

    static Haste() {
      currentScene = EditorApplication.currentScene;

      Index = new HasteIndex();

      projectWatcher = new HasteProjectWatcher();
      projectWatcher.Created += AssetCreated;
      projectWatcher.Deleted += AssetDeleted;
      projectWatcher.Start();

      hierarchyWatcher = new HasteHierarchyWatcher();
      hierarchyWatcher.Created += GameObjectCreated;
      hierarchyWatcher.Deleted += GameObjectDeleted;
      hierarchyWatcher.Start();

      EditorApplication.update += Update;
    }

    public static void Rebuild() {
      Index.Clear();

      hierarchyWatcher.ClearAndRestart();
      projectWatcher.ClearAndRestart();
    }

    static void AssetCreated(string path) {
      Index.Add(HasteUtils.GetRelativeAssetPath(path), HasteSource.Project);
    }

    static void AssetDeleted(string path) {
      Index.Remove(HasteUtils.GetRelativeAssetPath(path), HasteSource.Project);
    }

    static void GameObjectCreated(string path) {
      Index.Add(path, HasteSource.Hierarchy);
    }

    static void GameObjectDeleted(string path) {
      Index.Remove(path, HasteSource.Hierarchy);
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
    }
  }
}