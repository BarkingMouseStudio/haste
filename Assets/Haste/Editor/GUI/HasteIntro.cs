using UnityEditor;
using UnityEngine;

namespace Haste {

  public class HasteIntro : ScriptableObject {

    string tip;

    public HasteIntro Init(string tip) {
      this.tip = tip;
      return this;
    }

    public void OnGUI() {
      EditorGUILayout.Space();

      EditorGUILayout.LabelField("Just type.", HasteStyles.IntroStyle,
        GUILayout.Height(HasteStyles.IntroStyle.fixedHeight));

      EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
      EditorGUILayout.EndVertical();

      if (Haste.IsIndexing) {
        EditorGUILayout.LabelField(string.Format("(Indexing {0}...)", Haste.IndexingCount), HasteStyles.IndexingStyle);
      } else if (!string.IsNullOrEmpty(tip)) {
        EditorGUILayout.LabelField(tip, HasteStyles.TipStyle);
      }

      #if !IS_HASTE_PRO
      if (GUILayout.Button("Click here to upgrade to Haste Pro", HasteStyles.UpgradeStyle)) {
        UnityEditorInternal.AssetStore.Open(Haste.ASSET_STORE_PRO_URL);
      }
      #endif

      EditorGUILayout.Space();
    }
  }
}
