using UnityEngine;
using UnityEditor;

namespace Haste {

  public static class HasteUpdates {

    public static void DrawPreferences() {
      if (!HasteSettings.CheckForUpdates) {
        return;
      }

      switch (Haste.UpdateChecker.Status) {
        case HasteUpdateStatus.Available:
          if (GUILayout.Button("Update Haste", GUILayout.Width(128))) {
            #if IS_HASTE_PRO
              UnityEditorInternal.AssetStore.Open(Haste.ASSET_STORE_PRO_URL);
            #else
              UnityEditorInternal.AssetStore.Open(Haste.ASSET_STORE_FREE_URL);
            #endif
          }
          break;
        case HasteUpdateStatus.UpToDate:
          EditorGUILayout.LabelField("Haste is up to date.");
          break;
        case HasteUpdateStatus.Failed:
          EditorGUILayout.LabelField("Failed to check for updates.");
          break;
        case HasteUpdateStatus.InProgress:
          EditorGUILayout.LabelField("Checking for updates...");
          break;
      }
    }

    public static void DrawFooter() {
      if (!HasteSettings.CheckForUpdates) {
        return;
      }

      switch (Haste.UpdateChecker.Status) {
        case HasteUpdateStatus.Available:
          if (GUILayout.Button("Update Haste", HasteStyles.GetStyle("Upgrade"))) {
            #if IS_HASTE_PRO
              UnityEditorInternal.AssetStore.Open(Haste.ASSET_STORE_PRO_URL);
            #else
              UnityEditorInternal.AssetStore.Open(Haste.ASSET_STORE_FREE_URL);
            #endif
          }
          break;
        case HasteUpdateStatus.InProgress:
          EditorGUILayout.LabelField("Checking for updates...", HasteStyles.GetStyle("Tip"));
          break;
      }
    }
  }
}
