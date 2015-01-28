using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

namespace Haste {

  // Utility for wrapping a GUI section in spacers:
  //
  // using (new HasteSpace()) {
  // }
  public class HasteSpace : IDisposable {

    public HasteSpace() {
      EditorGUILayout.Space();
    }

    public void Dispose() {
      EditorGUILayout.Space();
    }
  }
}