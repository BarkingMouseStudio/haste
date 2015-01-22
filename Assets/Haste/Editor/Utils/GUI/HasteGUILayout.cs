using UnityEditor;
using UnityEngine;

namespace Haste {

  // Utility for pushing down content to the bottom:
  //
  // HasteGUILayout.Expander();
  public static class HasteGUILayout {

    public static void Expander() {
      EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
      EditorGUILayout.EndVertical();
    }
  }
}
