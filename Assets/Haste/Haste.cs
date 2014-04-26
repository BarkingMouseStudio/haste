using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  [InitializeOnLoad]
  public static class Haste {

    public static HasteScheduler Scheduler;
    public static HasteIndex Index;

    static HasteFileWatcher projectWatcher;
    static HasteHierarchyWatcher hierarchyWatcher;

    static string currentScene;

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

    static bool IsPaused {
      get {
        return EditorApplication.isCompiling ||
               EditorApplication.isPaused ||
               EditorApplication.isPlaying ||
               EditorApplication.isPlayingOrWillChangePlaymode ||
               EditorApplication.isUpdating;
      }
    }

    static bool restartHierarchy = false;
    static bool restartProject = false;

    public static void SceneChanged(string currentScene, string previousScene) {
      Scheduler.Stop("Hierarchy");
      restartHierarchy = true;
    }

    public static void PlaymodeStateChanged() {
      Scheduler.StopAll();

      if (!IsPaused) {
        restartHierarchy = true;
        restartProject = true;
      }
    }

    public static void ProjectWindowChanged() {
      Scheduler.Stop("Project");
      restartProject = true;
    }

    public static void HierarchyWindowChanged() {
      Scheduler.Stop("Hierarchy");
      restartHierarchy = true;
    }

    public static void HierarchyWindowItemOnGUI(int instanceId, Rect selectionRect) {
      hierarchyWatcher.Add(instanceId);
    }

    public static void AssetCreated(string path) {
      Index.Add(path, HasteSource.Project);
    }

    public static void AssetDeleted(string path) {
      Index.Remove(path, HasteSource.Project);
    }

    public static void GameObjectCreated(string path) {
      Index.Add(path, HasteSource.Hierarchy);
    }

    public static void GameObjectDeleted(string path) {
      Index.Remove(path, HasteSource.Hierarchy);
    }

    public static void Update() {
      if (!IsPaused) {
        if (currentScene != EditorApplication.currentScene) {
          string previousScene = currentScene;
          currentScene = EditorApplication.currentScene;
          SceneChanged(currentScene, previousScene);
        }

        Scheduler.Tick();

        if (restartHierarchy) {
          Scheduler.Start(hierarchyWatcher, "Hierarchy");
          restartHierarchy = false;
        }

        if (restartProject) {
          Scheduler.Start(projectWatcher, "Project");
          restartProject = false;
        }
      }
    }
  }
}