using UnityEngine;
using UnityEditor;
using System;

namespace Haste {

  public static class HastePreferences {

    static Vector2 scrollPosition = Vector2.zero;

    [PreferenceItem("Haste")]
    public static void PreferencesGUI() {
      using (var scrollView = new HasteScrollView(scrollPosition)) {
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

        EditorGUILayout.LabelField(String.Format("Haste has been opened {0:N0} times since {1} (about {2:N0} times per day).",
          HasteSettings.UsageCount,
          HasteSettings.UsageSinceDate.ToLongDateString(),
          HasteSettings.UsageAverage
        ), HasteStyles.UsageStyle);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Current Version", Haste.VERSION);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        bool backspaceEnabled = EditorGUILayout.Toggle("Should backspace clear?", HasteSettings.BackspaceClears);
        if (backspaceEnabled != HasteSettings.BackspaceClears) {
          HasteSettings.BackspaceClears = backspaceEnabled;
        }
        EditorGUILayout.HelpBox("When enabled (default) pressing backspace will clear the entire query inside the search box instead of one character at a time.", MessageType.Info);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Available Sources");
        EditorGUILayout.Space();

        using (var toggleGroup = new HasteToggleGroup("Haste Enabled", HasteSettings.Enabled)) {
          HasteSettings.Enabled = toggleGroup.Enabled;
          EditorGUILayout.Space();

          foreach (var watcher in Haste.Watchers) {
            string label = System.String.Format("{0} ({1})", watcher.Key, watcher.Value.IndexedCount);
            bool watchedEnabled = EditorGUILayout.Toggle(label, watcher.Value.Enabled);
            if (watchedEnabled != watcher.Value.Enabled) {
              EditorPrefs.SetBool(HasteSettings.GetPrefKey(HasteSetting.Source, watcher.Key), watchedEnabled);
              Haste.Watchers.ToggleSource(watcher.Key, watchedEnabled);
            }
          }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Indexed Count", Haste.IndexedCount.ToString());

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Ignore Paths");
        EditorGUILayout.Space();
        HasteSettings.IgnorePaths = EditorGUILayout.TextField(HasteSettings.IgnorePaths);
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Comma-separated paths to ignore when indexing assets. Useful for excluding folders you do not want to see in results. Rebuild the index to apply changes (see below).", MessageType.Info);

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
