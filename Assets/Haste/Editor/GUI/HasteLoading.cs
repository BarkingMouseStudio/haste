using UnityEditor;
using UnityEngine;
using System;

namespace Haste {

  public class HasteLoading : ScriptableObject {

    void OnEnable() {
      base.hideFlags = HideFlags.HideAndDontSave;
    }

    public void OnGUI() {
      EditorGUILayout.Space();
      EditorGUILayout.LabelField("Loading...", HasteStyles.GetStyle("Empty"));
    }
  }
}
