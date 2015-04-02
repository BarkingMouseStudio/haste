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

      EditorGUILayout.LabelField("No results found.", HasteStyles.Skin.GetStyle("Empty"));

      EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
      EditorGUILayout.EndVertical();

      HasteFooter.Draw(tip);

      EditorGUILayout.Space();
    }
  }
}
