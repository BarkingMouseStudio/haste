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

    static Haste() {
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

    public static void PlaymodeStateChanged() {
      if (IsPaused) {
        Logger.Info("PlaymodeStateChanged", "StopAll");
        Scheduler.StopAll();
      } else {
        Logger.Info("PlaymodeStateChanged", "Start (Restarting)");
        Scheduler.Start(projectWatcher, "Project");
        Scheduler.Start(hierarchyWatcher, "Hierarchy");
      }
    }

    public static void ProjectWindowChanged() {
      if (projectWatcher.IsRunning) {
        Logger.Info("ProjectWindowChanged", "Stop Project");
        Scheduler.Stop("Project");
      }

      Logger.Info("ProjectWindowChanged", "Start Project");
      Scheduler.Start(projectWatcher, "Project");
    }

    public static void HierarchyWindowChanged() {
      if (hierarchyWatcher.IsRunning) {
        Logger.Info("HierarchyWindowChanged", "Stop Hierarchy");
        Scheduler.Stop("Hierarchy");
      }

      Logger.Info("HierarchyWindowChanged", "Start Hierarchy");
      Scheduler.Start(hierarchyWatcher, "Hierarchy");
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
        Scheduler.Tick();
      }
    }
  }
}