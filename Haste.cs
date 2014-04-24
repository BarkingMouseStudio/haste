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
      Scheduler.Start(projectWatcher);

      hierarchyWatcher = new HasteHierarchyWatcher();
      hierarchyWatcher.Created += GameObjectCreated;
      hierarchyWatcher.Deleted += GameObjectDeleted;
      Scheduler.Start(hierarchyWatcher);

      // EditorApplication.hierarchyWindowChanged += HierarchyWindowChanged;
      EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;

      // EditorApplication.projectWindowChanged += ProjectWindowChanged;
      EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;

      EditorApplication.update += Update;
    }

    // public static void HierarchyWindowChanged() {
    //   if (!hierarchyWatcher.IsRunning) {
    //     Scheduler.Start(hierarchyWatcher);
    //   }
    // }

    public static void HierarchyWindowItemOnGUI(int instanceId, Rect selectionRect) {
      hierarchyWatcher.Add(instanceId);
    }

    // public static void ProjectWindowChanged() {
    //   if (!projectWatcher.IsRunning) {
    //     scheduler.Start(projectWatcher);
    //   }
    // }

    public static void ProjectWindowItemOnGUI(string guid, Rect selectionRect) {
      projectWatcher.Add(AssetDatabase.GUIDToAssetPath(guid));
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
      Scheduler.Tick();
    }
  }
}