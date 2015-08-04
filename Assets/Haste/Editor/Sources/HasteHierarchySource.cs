using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public class HasteHierarchySource : IEnumerable<HasteItem> {

    public const string NAME = "Hierarchy";

    // TODO: Put this somewhere better
    public static IDictionary<int, UnityEngine.Object> Scene =
      new Dictionary<int, UnityEngine.Object>();

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
          path = GetTransformPath(transform.parent) + "/" + transform.gameObject.name;
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

      Scene.Clear();

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

        var path = GetTransformPath(go.transform);
        var id = go.transform.GetSiblingIndex(); // go.GetInstanceID();
        var item = new HasteItem(path, id, NAME);
        var hash = item.GetHashCode();
        Scene[hash] = go;
        yield return item;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}
