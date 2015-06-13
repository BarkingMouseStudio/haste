using UnityEditor;
using UnityEngine;
using System;

namespace Haste {

  public class HasteLoading : ScriptableObject {

    public TimeSpan Duration;

    void OnEnable() {
      base.hideFlags = HideFlags.HideAndDontSave;
    }

    public void OnGUI() {
      EditorGUILayout.Space();

      if (Duration.TotalSeconds >= 1.0) {
        EditorGUILayout.LabelField(string.Format("Loading... {0}s", Duration.TotalSeconds), HasteStyles.GetStyle("Empty"));
      } else {
        EditorGUILayout.LabelField(string.Format("Loading... {0}ms", Duration.TotalMilliseconds), HasteStyles.GetStyle("Empty"));
      }
    }
  }
}
