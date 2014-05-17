using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public class HasteHierarchySource : IEnumerable<HasteItem> {

    IDictionary<int, string> paths = new Dictionary<int, string>();

    HashSet<HasteItem> incoming = new HashSet<HasteItem>();

    Texture gameObjectIcon;

    public HasteHierarchySource() {
      gameObjectIcon = EditorGUIUtility.ObjectContent(null, typeof(GameObject)).image;

      EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
    }

    void HierarchyWindowItemOnGUI(int instanceId, Rect selectionRect) {
      AddGameObject((GameObject)EditorUtility.InstanceIDToObject(instanceId));
    }

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

    void AddGameObject(GameObject go) {
      // We want to add children first since the rest of our search is bottom-up
      // (and paths are built that way).
      foreach (Transform child in go.transform) {
        AddGameObject(child.gameObject);
      }

      incoming.Add(new HasteItem(
        GetTransformPath(go.transform),
        go.GetInstanceID(),
        HasteSource.Hierarchy,
        gameObjectIcon));
    }

    public IEnumerator<HasteItem> GetEnumerator() {
      // Empty incoming queue
      foreach (HasteItem item in incoming) {
        yield return item;
      }

      incoming.Clear();

      // Add active objects
      foreach (GameObject go in Object.FindObjectsOfType<GameObject>()) {
        yield return new HasteItem(
          GetTransformPath(go.transform),
          go.GetInstanceID(),
          HasteSource.Hierarchy,
          gameObjectIcon);
      }

      // Add recent objects
      foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>()) {
        yield return new HasteItem(
          GetTransformPath(go.transform),
          go.GetInstanceID(),
          HasteSource.Hierarchy,
          gameObjectIcon);
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}