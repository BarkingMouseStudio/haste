using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteProjectResult : AbstractHasteResult {

    public HasteProjectResult(HasteItem item, float score, List<int> indices) : base(item, score, indices) {}

    public override void Draw(bool isHighlighted) {
      var icon = AssetDatabase.GetCachedIcon(Item.Path);
      if (icon != null) {
        UnityEngine.GUI.DrawTexture(EditorGUILayout.GetControlRect(GUILayout.Width(32), GUILayout.Height(32)), icon);
      }

      base.Draw(isHighlighted);
    }

    public override void Action() {
      EditorApplication.ExecuteMenuItem("Window/Project");
      Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(Item.Path);
    }

    public override void Select() {
      Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(Item.Path);
    }
  }
}