using UnityEngine;
using UnityEditor;

namespace Haste {
  public static class HasteFooter {
    public static void Draw(string tip) {
      if (!string.IsNullOrEmpty(tip)) {
        EditorGUILayout.LabelField(tip, HasteStyles.GetStyle("Tip"));
      }

      if (Haste.IsIndexing) {
        EditorGUILayout.LabelField(string.Format("(Indexing {0}...)", Haste.IndexingCount), HasteStyles.GetStyle("Indexing"));
        return;
      }

      HasteUpdates.DrawFooter();

      #if !IS_HASTE_PRO
      if (GUILayout.Button("Upgrade to Haste Pro", HasteStyles.GetStyle("Upgrade"))) {
        UnityEditorInternal.AssetStore.Open(Haste.ASSET_STORE_PRO_URL);
      }
      #endif
    }
  }
}
