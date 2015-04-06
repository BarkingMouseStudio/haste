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
        EditorGUI.LabelField(labelRect, selection.Count.ToString(), HasteStyles.GetStyle("Count"));

        Rect textureRect;
        int offset = 0;
        int positionOffset;
        foreach (var obj in selection) {
          positionOffset = (offset % 3) * 2;
          textureRect = new Rect(position.x + 50 + positionOffset, position.y, 16, 16);
          if (obj is GameObject) {
            GUI.DrawTexture(textureRect, HasteHierarchyResult.GameObjectIcon);
          } else {
            GUI.DrawTexture(textureRect, EditorGUIUtility.ObjectContent(obj, obj.GetType()).image, ScaleMode.ScaleToFit);
          }
          offset++;
        }
      }
    }
  }
}
