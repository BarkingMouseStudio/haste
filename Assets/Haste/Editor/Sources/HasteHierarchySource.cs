using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public class HasteHierarchySource : IEnumerable<HasteItem> {

    public static readonly string NAME = "Hierarchy";

    IDictionary<int, string> paths = new Dictionary<int, string>();

    // TODO: Use StringBuilder: pass it in and down; String.Concat is slow
    // TODO: Remove recursion
    string GetTransformPath(Transform transform) {
      int id = transform.gameObject.GetInstanceID();
      string path;

      if (!paths.TryGetValue(id, out path)) {
        if (transform.parent == null) {
          path = transform.gameObject.name;
        } else {
          path = GetTransformPath(transform.parent) + Path.DirectorySeparatorChar + transform.gameObject.name;
        }

        paths.Add(id, path);
      }

      return path;
    }

    public IEnumerator<HasteItem> GetEnumerator() {
      var allFlags = HideFlags.NotEditable |
        HideFlags.DontSave |
        HideFlags.HideAndDontSave |
        HideFlags.HideInInspector |
        HideFlags.HideInHierarchy;

      foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>()) {
        if (go == null) {
          // Null-check required since we yield, meaning the
          // results of the find could become invalid.
          continue;
        }

        if ((go.hideFlags & allFlags) != 0) {
          continue;
        }

        var prefabType = PrefabUtility.GetPrefabType(go);
        if (prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab) {
          continue;
        }

        string path = GetTransformPath(go.transform);
        int id = go.GetInstanceID();
        yield return new HasteItem(path, id, NAME);
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}