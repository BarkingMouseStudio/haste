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

      if (!string.IsNullOrEmpty(tip)) {
        EditorGUILayout.LabelField(tip, HasteStyles.Skin.GetStyle("Tip"));
      }

      #if !IS_HASTE_PRO
      if (GUILayout.Button("Click here to upgrade to Haste Pro", HasteStyles.Skin.GetStyle("Upgrade"))) {
        UnityEditorInternal.AssetStore.Open(Haste.ASSET_STORE_PRO_URL);
      }
      #endif

      EditorGUILayout.Space();
    }
  }
}
