using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public class HasteHierarchyWatcher : HasteWatcher {

    HashSet<HasteItem> currentCollection; 
    HashSet<HasteItem> nextCollection; 

    IDictionary<int, string> paths = new Dictionary<int, string>();

    public HasteHierarchyWatcher() {
      this.currentCollection = new HashSet<HasteItem>();
      this.nextCollection = new HashSet<HasteItem>();

      EditorApplication.hierarchyWindowChanged += HierarchyWindowChanged;
      EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;

      Haste.SceneChanged += SceneChanged;
    }

    public override void Reset() {
      // Forget everything
      currentCollection.Clear();
      nextCollection.Clear();
    }

    void SceneChanged(string currentScene, string previousScene) {
      // Start again
      nextCollection.Clear();
      Restart();
    }

    void HierarchyWindowChanged() {
      // Start again
      nextCollection.Clear();
      Restart();
    }

    void HierarchyWindowItemOnGUI(int instanceId, Rect selectionRect) {
      AddItem((GameObject)EditorUtility.InstanceIDToObject(instanceId));
    }

    void AddItem(GameObject go) {
      // We want to add children first since the rest of our search is bottom-up
      // (and paths are built that way).
      foreach (Transform child in go.transform) {
        AddItem(child.gameObject);
      }

      HasteItem item = new HasteItem(GetPath(go.transform), go.GetInstanceID(), HasteSource.Hierarchy);

      if (scheduler.IsRunning) {
        // We have to add to the next collection since we don't know where GetEnumerator
        // is at in the run and next collection is used in checking deletions.
        nextCollection.Add(item);
      }

      // If we are running, we want to add and trigger the event manually
      // since a flush won't work if we're already past that step.

      // If we're not running we want to treat it as a one-off.

      if (!currentCollection.Contains(item)) {
        currentCollection.Add(item);
        OnCreated(item);
      }
    }

    public override IEnumerator GetEnumerator() {
      // TODO: Should we move reset code here?

      // Add active objects
      foreach (GameObject go in Object.FindObjectsOfType<GameObject>()) {
        HasteItem item = new HasteItem(GetPath(go.transform), go.GetInstanceID(), HasteSource.Hierarchy);

        if (!nextCollection.Contains(item)) {
          if (!currentCollection.Contains(item)) {
            OnCreated(item);
          }

          nextCollection.Add(item);
        }

        yield return null;
      }

      // Add recent objects
      foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>()) {
        HasteItem item = new HasteItem(GetPath(go.transform), go.GetInstanceID(), HasteSource.Hierarchy);

        if (!nextCollection.Contains(item)) {
          if (!currentCollection.Contains(item)) {
            OnCreated(item);
          }

          nextCollection.Add(item);
        }

        yield return null;
      }

      // Check for deleted paths
      foreach (HasteItem item in currentCollection) {
        // If an item from our original collection is not found
        // in our new collection, it has been removed.
        if (!nextCollection.Contains(item)) {
          OnDeleted(item);
        }

        yield return null;
      }

      var temp = currentCollection;
      currentCollection = nextCollection;

      nextCollection = temp;
      nextCollection.Clear(); // We clear it when we're done (not at the beginning in case something was added)

      paths.Clear();
    }

    string GetPath(Transform transform) {
      int id = transform.gameObject.GetInstanceID();
      string path;

      if (!paths.TryGetValue(id, out path)) {
        if (transform.parent == null) {
          path = transform.gameObject.name;
        } else {
          path = GetPath(transform.parent) + Path.DirectorySeparatorChar + transform.gameObject.name;
        }

        paths.Add(id, path);
      }

      return path;
    }
  }
}