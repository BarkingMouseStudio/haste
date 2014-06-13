using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteHierarchyResult : AbstractHasteResult {

    private static Texture _GameObjectIcon;
    public static Texture GameObjectIcon {
      get {
        if (_GameObjectIcon == null) {
          _GameObjectIcon = EditorGUIUtility.ObjectContent(null, typeof(GameObject)).image;
        }
        return _GameObjectIcon;
      }
    }

    public HasteHierarchyResult(HasteItem item, float score, List<int> indices) : base(item, score, indices) {}

    public override void Draw() {
      GUI.DrawTexture(EditorGUILayout.GetControlRect(GUILayout.Width(32), GUILayout.Height(32)),
        GameObjectIcon);

      base.Draw();
    }

    public override void Action() {
      EditorApplication.ExecuteMenuItem("Window/Hierarchy");
      Selection.activeObject = Find(Item.Id);
    }

    // TODO: Move to source and reference from there
    public static GameObject Find(int instanceID) {
      foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>()) {
        if (go.hideFlags == HideFlags.HideInInspector ||
            go.hideFlags == HideFlags.HideInHierarchy ||
            go.hideFlags == HideFlags.DontSave ||
            go.hideFlags == HideFlags.HideAndDontSave) {
          continue;
        }

        if (instanceID == go.GetInstanceID()) {
          return go;
        }
      }

      return null;
    }
  }
}