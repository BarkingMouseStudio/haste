using UnityEditor;
using UnityEngine;

namespace Haste {

  public class HasteEmpty : ScriptableObject {

    string tip;

    public HasteEmpty Init(string tip) {
      this.tip = tip;
      return this;
    }

    public void OnGUI() {
      using (new HasteSpace()) {
        EditorGUILayout.LabelField("No results found.", HasteStyles.EmptyStyle);

        HasteGUILayout.Expander();

        if (!string.IsNullOrEmpty(tip)) {
          EditorGUILayout.LabelField(tip, HasteStyles.TipStyle);
        }

        #if !IS_HASTE_PRO
        if (GUILayout.Button("Click here to upgrade to Haste Pro", HasteStyles.UpgradeStyle)) {
          UnityEditorInternal.AssetStore.Open(Haste.ASSET_STORE_PRO_URL);
        }
        #endif
      }
    }
  }
}
