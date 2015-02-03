using UnityEngine;
using UnityEditor;

namespace Haste {

  public static class HastePreferences {

    static Vector2 scrollPosition = Vector2.zero;

    [PreferenceItem("Haste")]
    public static void PreferencesGUI() {
      using (var scrollView = new HasteScrollView(scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true))) {
        scrollPosition = scrollView.ScrollPosition;

        EditorGUILayout.Space();

        #if !IS_HASTE_PRO
        EditorGUILayout.HelpBox("Upgrade to Haste Pro to enable more features (like actions).", MessageType.Warning);
        EditorGUILayout.Space();
        if (GUILayout.Button("Upgrade to Haste Pro", GUILayout.Width(128))) {
          UnityEditorInternal.AssetStore.Open(Haste.ASSET_STORE_PRO_URL);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        #endif

        EditorGUILayout.LabelField("Times Opened", HasteSettings.UsageCount.ToString());

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Available Sources");
        EditorGUILayout.Space();

        using (var toggleGroup = new HasteToggleGroup("Haste Enabled", HasteSettings.Enabled)) {
          HasteSettings.Enabled = toggleGroup.Enabled;
          EditorGUILayout.Space();

          foreach (var watcher in Haste.Watchers) {
            string label = System.String.Format("{0} ({1})", watcher.Key, watcher.Value.IndexedCount);
            bool enabled = EditorGUILayout.Toggle(label, watcher.Value.Enabled);
            EditorPrefs.SetBool(HasteSettings.GetPrefKey(HasteSetting.Source, watcher.Key), enabled);
            Haste.Watchers.ToggleSource(watcher.Key, enabled);
          }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Ignore Paths");
        EditorGUILayout.Space();
        HasteSettings.IgnorePaths = EditorGUILayout.TextField(HasteSettings.IgnorePaths);
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Comma-separated paths to ignore when indexing assets. Useful for excluding folders you do not want to see in results.", MessageType.Info);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (Haste.IsIndexing) {
          EditorGUILayout.LabelField("Indexing...", Haste.IndexingCount.ToString());
        } else {
          EditorGUILayout.LabelField("Index Size", Haste.IndexSize.ToString());
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("Rebuild Index", GUILayout.Width(128))) {
          Haste.Rebuild();
        }
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Rebuilds the internal index used for fast searching in Haste. Use this if Haste starts providing weird results.", MessageType.Info);
      }
    }
  }
}
