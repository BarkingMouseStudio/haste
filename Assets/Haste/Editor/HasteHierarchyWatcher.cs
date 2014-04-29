using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {
  public class HasteHierarchyWatcher : HasteWatcher {

    protected IDictionary<int, string> paths;

    public HasteHierarchyWatcher() : base() {
      this.paths = new Dictionary<int, string>();
    }

    public HasteHierarchyWatcher(IEnumerable<string> collection) : base(collection) {
      this.paths = new Dictionary<int, string>();
    }

    public void Add(int instanceId) {
      GameObject go = (GameObject)EditorUtility.InstanceIDToObject(instanceId);

      // We want to add children first since the rest of our search is bottom-up
      // (and paths are built that way).
      foreach (Transform child in go.transform) {
        Add(child.gameObject.GetInstanceID());
      }

      string path = GetPath(go.transform);

      if (IsRunning) {
        if (!nextCollection.Contains(path)) {
          // We have to add to the next collection since we don't know where GetEnumerator
          // is at in the run and next collection is used in checking deletions.
          nextCollection.Add(path);
        }
      }

      // - If we are running, we want to add and trigger the event manually
      // since a flush won't work if we're already past that step.
      // - If we're not running we want to treat it as a one-off.
      if (!currentCollection.Contains(path)) {
        currentCollection.Add(path);
        OnCreated(path);
      }
    }

    public override IEnumerator GetEnumerator() {
      IsRunning = true;

      // Add active objects
      foreach (GameObject go in Object.FindObjectsOfType<GameObject>()) {
        if (!go.activeInHierarchy) {
          continue;
        }

        string path = GetPath(go.transform);

        if (!nextCollection.Contains(path)) {
          if (!currentCollection.Contains(path)) {
            OnCreated(path);
          }

          nextCollection.Add(path);
        }

        yield return null;
      }

      // Add recent objects
      foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>()) {
        if (!go.activeInHierarchy) {
          continue;
        }

        string path = GetPath(go.transform);

        if (!nextCollection.Contains(path)) {
          if (!currentCollection.Contains(path)) {
            OnCreated(path);
          }

          nextCollection.Add(path);
        }

        yield return null;
      }

      // Check for deleted paths
      foreach (string path in currentCollection) {
        // If an item from our original collection is not found
        // in our new collection, it has been removed.
        if (!nextCollection.Contains(path)) {
          OnDeleted(path);
        }

        yield return null;
      }

      var temp = currentCollection;
      currentCollection = nextCollection;

      nextCollection = temp;
      nextCollection.Clear(); // We clear it when we're done (not at the beginning in case something was added)

      paths.Clear();

      IsRunning = false;
    }

    protected string GetPath(Transform transform) {
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