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

      var introStyle = HasteStyles.Skin.GetStyle("Intro");
      EditorGUILayout.LabelField("Just type.", introStyle, GUILayout.Height(introStyle.fixedHeight));

      EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
      EditorGUILayout.EndVertical();

      HasteFooter.Draw(tip);

      EditorGUILayout.Space();
    }
  }
}
