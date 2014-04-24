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
    }

    public override IEnumerator GetEnumerator() {
      HashSet<string> newCollection;

      // Poll indefinitely
      while (true) {
        paths.Clear();

        // Initialize a new collection
        newCollection = new HashSet<string>();

        // Add active objects
        foreach (GameObject go in Object.FindObjectsOfType<GameObject>()) {
          string path = GetPath(go.transform);

          if (!collection.Contains(path)) {
            OnCreated(path);
          }

          newCollection.Add(path);
        }

        // Check for deleted paths
        foreach (string path in collection) {
          // If an item from our original collection is not found
          // in our new collection, it has been removed.
          if (!newCollection.Contains(path)) {
            OnDeleted(path);
          }
        }

        collection = newCollection;

        int ticks = 1000; // Wait about 10s
        while (ticks-- > 0) {
          yield return null;
        }
      }
    }

    protected string GetPath(Transform transform) {
      int id = transform.gameObject.GetInstanceID();
      string path;

      if (!paths.TryGetValue(id, out path)) {
        if (transform.parent == null) {
          path = transform.gameObject.name;
        } else {
          path = GetPath(transform.parent) + "/" + transform.gameObject.name;
        }

        paths.Add(id, path);
      }

      return path;
    }
  }
}