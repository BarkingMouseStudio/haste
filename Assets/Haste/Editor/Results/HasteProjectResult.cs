using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteProjectResult : AbstractHasteResult {

    public HasteProjectResult(HasteItem item, float score, List<int> indices) : base(item, score, indices) {}

    public override void Draw() {
      UnityEngine.GUI.DrawTexture(EditorGUILayout.GetControlRect(GUILayout.Width(32), GUILayout.Height(32)),
        AssetDatabase.GetCachedIcon(Item.Path));

      base.Draw();
    }

    public override void Action() {
      // EditorApplication.ExecuteMenuItem("Window/Project");
      // EditorUtility.FocusProjectWindow();

      // Traverse project downwards, pinging each level
      string[] pieces = Item.Path.Split(Path.DirectorySeparatorChar);
      string fullPath = "";
      for (int i = 0; i < pieces.Length; i++) {
        fullPath = Path.Combine(fullPath, pieces[i]);
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(fullPath);
      }
    }
  }
}