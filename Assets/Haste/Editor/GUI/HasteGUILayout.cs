using UnityEditor;
using UnityEngine;

namespace Haste {

  public static class HasteGUILayout {

    // Utility for pushing down content to the bottom:
    //
    // HasteGUILayout.Expander();
    public static void Expander() {
      EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
      EditorGUILayout.EndVertical();
    }
  }
}
