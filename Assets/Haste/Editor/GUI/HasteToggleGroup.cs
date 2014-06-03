using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

namespace Haste {

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