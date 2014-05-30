using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

namespace Haste {

  public class HasteScrollView : IDisposable {

    public Vector2 ScrollPosition { get; protected set; }

    public HasteScrollView(Vector2 scrollPosition, params GUILayoutOption[] options) {
      ScrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, options);
    }

    public void Dispose() {
      EditorGUILayout.EndScrollView();
    }
  }
}