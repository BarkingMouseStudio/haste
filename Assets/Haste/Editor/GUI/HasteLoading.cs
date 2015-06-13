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

      string label;
      if (Duration.TotalSeconds >= 1.0) {
        label = string.Format("Loading... {0}s", Duration.TotalSeconds);
      } else {
        label = string.Format("Loading... {0}ms", Duration.TotalMilliseconds);
      }
      EditorGUILayout.LabelField(label, HasteStyles.GetStyle("Empty"));
    }
  }
}
