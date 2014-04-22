using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HierarchyIndex : AbstractIndex {
    private IDictionary<int, string> paths = new Dictionary<int, string>();

    public override void Rebuild() {
      index.Clear();
      paths.Clear();

      Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();

      Transform transform;
      Item result;
      for (int i = 0; i < transforms.Length; i++) {
        transform = transforms[i];

        if (!transform.gameObject.activeInHierarchy) {
          continue;
        }

        result = new Item(transform.gameObject.name,
          GetPath(transform.gameObject),
          Source.Hierarchy,
          EditorGUIUtility.ObjectContent(null, typeof(GameObject)).image);

        AddItem(result);
      }
    }

    private string GetPath(GameObject go) {
      int id = go.GetInstanceID();
      string path;

      if (paths.TryGetValue(id, out path)) {
        return path;
      }

      if (go.transform.parent == null) {
        path = go.name;
      } else {
        path = GetPath(go.transform.parent.gameObject) + "/" + go.name;
      }

      paths.Add(id, path);
      return path;
    }
  }
}
