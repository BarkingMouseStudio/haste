using UnityEditor;
using UnityEngine;

namespace Haste {

  public class HasteGUIEmpty : ScriptableObject {

    public string tip;

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
