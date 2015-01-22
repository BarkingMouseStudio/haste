using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

namespace Haste {

  // Utility for creating a toggle group:
  //
  // using (var group = new HasteToggleGroup("Checkbox?", false)) {
  //   Debug.Log(group.Enabled);
  // }
  public class HasteToggleGroup : IDisposable {

    public bool Enabled { get; protected set; }

    public HasteToggleGroup(string label, bool toggle) {
      Enabled = EditorGUILayout.BeginToggleGroup(label, toggle);
    }

    public void Dispose() {
      EditorGUILayout.EndToggleGroup();
    }
  }
}