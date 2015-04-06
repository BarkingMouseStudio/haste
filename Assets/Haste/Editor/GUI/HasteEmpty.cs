using UnityEditor;
using UnityEngine;

namespace Haste {

  public class HasteEmpty : ScriptableObject {

    string tip;

    public HasteEmpty Init(string tip) {
      this.tip = tip;
      return this;
    }

    void OnEnable() {
      base.hideFlags = HideFlags.HideAndDontSave;
    }

    public void OnGUI() {
      EditorGUILayout.Space();

      EditorGUILayout.LabelField("No results found.", HasteStyles.GetStyle("Empty"));

      using (new HasteVertical(GUILayout.ExpandHeight(true))) {}

      HasteFooter.Draw(tip);

      EditorGUILayout.Space();
    }
  }
}
