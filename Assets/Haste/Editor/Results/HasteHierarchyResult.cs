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
      Selection.activeObject = GameObject.Find(Item.Path);
    }
  }
}