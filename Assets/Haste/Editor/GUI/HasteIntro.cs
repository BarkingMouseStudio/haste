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

      if (Haste.IsIndexing) {
        EditorGUILayout.LabelField(string.Format("(Indexing {0}...)", Haste.IndexingCount), HasteStyles.Skin.GetStyle("Indexing"));
      } else if (!string.IsNullOrEmpty(tip)) {
        EditorGUILayout.LabelField(tip, HasteStyles.Skin.GetStyle("Tip"));
      }

      Haste.Updates.OnGUI();

      #if !IS_HASTE_PRO
      if (GUILayout.Button("Click here to upgrade to Haste Pro", HasteStyles.Skin.GetStyle("Upgrade"))) {
        UnityEditorInternal.AssetStore.Open(Haste.ASSET_STORE_PRO_URL);
      }
      #endif

      EditorGUILayout.Space();
    }
  }
}
