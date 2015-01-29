using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteProjectResult : AbstractHasteResult {

    private UnityEngine.Object object_;
    public override UnityEngine.Object Object {
      get {
        if (object_ == null) {
          object_ = AssetDatabase.LoadMainAssetAtPath(Item.Path);
        }
        return object_;
      }
    }

    public override bool IsDraggable {
      get { return true; }
    }

    public override string DragLabel {
      get { return Object.name; }
    }

    public HasteProjectResult(HasteItem item, float score, List<int> indices) : base(item, score, indices) {}

    public override void Draw(bool isHighlighted) {
      var icon = AssetDatabase.GetCachedIcon(Item.Path);
      if (icon != null) {
        var rect = EditorGUILayout.GetControlRect(GUILayout.Width(32), GUILayout.Height(32));
        rect.y += 5; // center the icon vertically
        UnityEngine.GUI.DrawTexture(rect, icon);
      }

      base.Draw(isHighlighted);
    }

    public override void Action() {
      EditorApplication.ExecuteMenuItem("Window/Project");
      Selection.activeObject = Object;
    }

    public override void Select() {
      Selection.activeObject = Object;
    }
  }
}