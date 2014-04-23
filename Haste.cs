using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  [InitializeOnLoad]
  public static class Haste {

    static HasteScheduler scheduler = new HasteScheduler();

    static Haste() {
      IHasteWatcher projectWatcher = new HasteFileWatcher(Application.dataPath);
      projectWatcher.Created += OnAssetCreated;
      projectWatcher.Deleted += OnAssetDeleted;

      // IHasteWatcher hierarchyWatcher = new HasteHierarchyWatcher();
      // hierarchyWatcher.Created += OnGameObjectCreated;
      // hierarchyWatcher.Deleted += OnGameObjectDeleted;

      scheduler.Start(projectWatcher.GetEnumerator());
      // scheduler.Start(hierarchyWatcher.GetEnumerator());

      EditorApplication.update += Update;
    }

    public static void OnAssetCreated(string path) {
      Logger.Info("Asset Created", path);
    }

    public static void OnAssetDeleted(string path) {
      Logger.Info("Asset Deleted", path);
    }

    public static void Update() {
      scheduler.Tick();
    }
  }
}