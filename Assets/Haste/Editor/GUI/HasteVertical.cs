using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

namespace Haste {

  public class HasteVertical : IDisposable {

    public Rect Rect { get; protected set; }

    public HasteVertical() {
      Rect = EditorGUILayout.BeginVertical();
    }

    public void Dispose() {
      EditorGUILayout.EndVertical();
    }
  }
}