using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteHierarchyResult : AbstractHasteResult {

    public HasteHierarchyResult(HasteItem item, float score, List<int> indices) : base(item, score, indices, HasteIntent.Search) {}

    public override void Draw() {
      GUI.DrawTexture(EditorGUILayout.GetControlRect(GUILayout.Width(32), GUILayout.Height(32)),
        HasteWindow.GameObjectIcon);

      base.Draw();
    }

    public override void Action() {
      // EditorApplication.ExecuteMenuItem("Window/Hierarchy");
      Selection.activeObject = GameObject.Find(Item.Path);
    }
  }
}