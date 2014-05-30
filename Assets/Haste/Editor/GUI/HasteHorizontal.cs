using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

namespace Haste {

  public class HasteHorizontal : IDisposable {

    public Rect Rect { get; protected set; }

    public HasteHorizontal() {
      Rect = EditorGUILayout.BeginHorizontal();
    }

    public HasteHorizontal(params GUILayoutOption[] options) {
      Rect = EditorGUILayout.BeginHorizontal(options);
    }

    public HasteHorizontal(GUIStyle style, params GUILayoutOption[] options) {
      Rect = EditorGUILayout.BeginHorizontal(style, options);
    }

    public void Dispose() {
      EditorGUILayout.EndHorizontal();
    }
  }
}