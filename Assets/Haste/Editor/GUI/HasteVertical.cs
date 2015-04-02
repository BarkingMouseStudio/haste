using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

namespace Haste {

  // Utility for creating vertical groups:
  //
  // using (var vertical = new HasteVertical()) {
  //   Debug.Log(vertical.Rect);
  // }
  public class HasteVertical : IDisposable {

    public Rect Rect { get; protected set; }

    public HasteVertical() {
      Rect = EditorGUILayout.BeginVertical();
    }

    public HasteVertical(params GUILayoutOption[] options) {
      Rect = EditorGUILayout.BeginVertical(options);
    }

    public void Dispose() {
      EditorGUILayout.EndVertical();
    }
  }
}
