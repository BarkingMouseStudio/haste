using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public static class HasteSelection {

    public static void Draw(Rect position, HashSet<UnityEngine.Object> selection) {
      if (selection.Count > 0) {
        var labelRect = new Rect(position.x, position.y, 48, 16);
        var textureRect = new Rect(position.x + 50, position.y, 16, 16);

        EditorGUI.LabelField(labelRect, selection.Count.ToString(), HasteStyles.CountStyle);

        if (selection.Any(x => x is GameObject)) {
          GUI.DrawTexture(textureRect, HasteHierarchyResult.GameObjectIcon);
        } else {
          var obj = selection.First();
          GUI.DrawTexture(textureRect, EditorGUIUtility.ObjectContent(obj, obj.GetType()).image);
        }
      }
    }
  }
}