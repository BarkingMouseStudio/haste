using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

namespace Haste {

  public class HasteSpace : IDisposable {

    public HasteSpace() {
      EditorGUILayout.Space();
    }

    public void Dispose() {
      EditorGUILayout.Space();
    }
  }
}