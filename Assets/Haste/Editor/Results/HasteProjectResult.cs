using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteProjectResult : AbstractHasteResult {

    private UnityEngine.Object unityObject;
    public override UnityEngine.Object Object {
      get {
        if (unityObject == null) {
          unityObject = AssetDatabase.LoadMainAssetAtPath(Item.path);
        }
        return unityObject;
      }
    }

    public override bool IsDraggable {
      get { return true; }
    }

    public override string DragLabel {
      get { return Object.name; }
    }

    public HasteProjectResult(HasteItem item, float score, string queryLower) : base(item, score, queryLower) {}

    public override void Draw(bool isHighlighted) {
      var icon = AssetDatabase.GetCachedIcon(Item.path);
      if (icon != null) {
        var rect = EditorGUILayout.GetControlRect(GUILayout.Width(32), GUILayout.Height(32));
        rect.y += 5; // center the icon vertically
        UnityEngine.GUI.DrawTexture(rect, icon);
      }

      base.Draw(isHighlighted);
    }

    public override void Action() {
      EditorApplication.ExecuteMenuItem("Window/Project");
      EditorUtility.FocusProjectWindow();
      Selection.objects = new UnityEngine.Object[]{Object};
      EditorGUIUtility.PingObject(Selection.activeObject);
    }
  }
}
