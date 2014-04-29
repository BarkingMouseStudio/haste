using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  [InitializeOnLoad]
  public static class Haste {

    internal static readonly bool IS_PRO = true;

    public static HasteScheduler Scheduler;
    public static HasteIndex Index;

    public static int usageCount = -1;
    public static int UsageCount {
      get {
        if (usageCount < 0) {
          usageCount = EditorPrefs.GetInt("HasteUsageCount", 0);
        }
        return usageCount;
      }
      internal set {
        usageCount = value;
        EditorPrefs.SetInt("HasteUsageCount", usageCount);
      }
    }

    static HasteFileWatcher projectWatcher;
    static HasteHierarchyWatcher hierarchyWatcher;

    static string currentScene;

    static bool restartHierarchy = false;
    static bool restartProject = false;

    static Haste() {
      currentScene = EditorApplication.currentScene;

      Scheduler = new HasteScheduler();
      Index = new HasteIndex();

      projectWatcher = new HasteFileWatcher(Application.dataPath);
      projectWatcher.Created += AssetCreated;
      projectWatcher.Deleted += AssetDeleted;
      Scheduler.Start(projectWatcher, "Project");

      hierarchyWatcher = new HasteHierarchyWatcher();
      hierarchyWatcher.Created += GameObjectCreated;
      hierarchyWatcher.Deleted += GameObjectDeleted;
      Scheduler.Start(hierarchyWatcher, "Hierarchy");

      EditorApplication.playmodeStateChanged += PlaymodeStateChanged;

      EditorApplication.projectWindowChanged += ProjectWindowChanged;

      EditorApplication.hierarchyWindowChanged += HierarchyWindowChanged;
      EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;

      EditorApplication.update += Update;
    }

    public static void Rebuild() {
      Scheduler.StopAll();

      Index.Clear();

      hierarchyWatcher.Clear();
      projectWatcher.Clear();

      restartHierarchy = true;
      restartProject = true;
    }

    static bool IsPaused {
      get {
        return EditorApplication.isCompiling ||
               EditorApplication.isPaused ||
               EditorApplication.isPlaying ||
               EditorApplication.isPlayingOrWillChangePlaymode ||
               EditorApplication.isUpdating;
      }
    }

    static void SceneChanged(string currentScene, string previousScene) {
      Scheduler.Stop("Hierarchy");
      restartHierarchy = true;
    }

    static void PlaymodeStateChanged() {
      Scheduler.StopAll();

      if (!IsPaused) {
        restartHierarchy = true;
        restartProject = true;
      }
    }

    static void ProjectWindowChanged() {
      Scheduler.Stop("Project");
      restartProject = true;
    }

    static void HierarchyWindowChanged() {
      Scheduler.Stop("Hierarchy");
      restartHierarchy = true;
    }

    static void HierarchyWindowItemOnGUI(int instanceId, Rect selectionRect) {
      hierarchyWatcher.Add(instanceId);
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

    static void Update() {
      if (!IsPaused) {
        if (currentScene != EditorApplication.currentScene) {
          string previousScene = currentScene;
          currentScene = EditorApplication.currentScene;
          SceneChanged(currentScene, previousScene);
        }

        Scheduler.Tick();

        if (restartHierarchy) {
          restartHierarchy = false;

          hierarchyWatcher.Restart();
          Scheduler.Start(hierarchyWatcher, "Hierarchy");
        }

        if (restartProject) {
          restartProject = false;

          projectWatcher.Restart();
          Scheduler.Start(projectWatcher, "Project");
        }
      }
    }
  }
}