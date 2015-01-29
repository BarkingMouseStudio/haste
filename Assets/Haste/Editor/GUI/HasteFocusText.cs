using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

namespace Haste {

  // Utility for focusing a text field:
  //
  // using (new HasteFocusText("MyField")) {
  //   myStr = EditorGUILayout.TextField(myStr);
  // }
  public class HasteFocusText : IDisposable {

    public string Name { get; protected set; }

    public HasteFocusText(string name) {
      Name = name;
      UnityEngine.GUI.SetNextControlName(Name);
    }

    public void Dispose() {
      EditorGUI.FocusTextInControl(Name);
    }
  }
}