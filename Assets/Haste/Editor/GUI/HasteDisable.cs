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
  public class HasteDisabled : IDisposable {

    public HasteDisabled(bool disabled = true) {
      EditorGUI.BeginDisabledGroup(disabled);
    }

    public void Dispose() {
      EditorGUI.EndDisabledGroup();
    }
  }
}
