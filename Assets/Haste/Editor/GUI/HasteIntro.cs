using UnityEditor;
using UnityEngine;

namespace Haste {

  public class HasteIntro : ScriptableObject {

    string tip;

    public HasteIntro Init(string tip) {
      this.tip = tip;
      return this;
    }

    void OnEnable() {
      base.hideFlags = HideFlags.HideAndDontSave;
    }

    public void OnGUI() {
      EditorGUILayout.Space();

      var introStyle = HasteStyles.GetStyle("Intro");
      EditorGUILayout.LabelField("Just type.", introStyle, GUILayout.Height(introStyle.fixedHeight));

      using (new HasteVertical(GUILayout.ExpandHeight(true))) {}

      HasteFooter.Draw(tip);

      EditorGUILayout.Space();
    }
  }
}
