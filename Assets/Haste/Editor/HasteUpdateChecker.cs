using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public enum HasteUpdateStatus {
    Available,
    UpToDate,
    Failed,
    InProgress
  }

  public class HasteUpdateChecker {

    static readonly string HASTE_UPDATE_URL =
      "https://evening-basin-5533.herokuapp.com/version";

    public TimeSpan Interval { get; private set; }
    public HasteUpdateStatus Status { get; private set; }
    public float Progress { get; private set; }

    public HasteUpdateChecker() {
      Status = HasteUpdateStatus.UpToDate;
      Progress = 0.0f;
      Interval = TimeSpan.FromHours(1);
    }

    public void OnGUI() {
      if (!HasteSettings.CheckForUpdates) {
        return;
      }

      switch (Status) {
        case HasteUpdateStatus.Available:
          if (GUILayout.Button("An update is available!", HasteStyles.UpgradeStyle)) {
            #if IS_HASTE_PRO
              UnityEditorInternal.AssetStore.Open(Haste.ASSET_STORE_PRO_URL);
            #else
              UnityEditorInternal.AssetStore.Open(Haste.ASSET_STORE_FREE_URL);
            #endif
          }
          break;
        case HasteUpdateStatus.UpToDate:
          EditorGUILayout.LabelField("Haste is up to date.", HasteStyles.TipStyle);
          break;
        case HasteUpdateStatus.Failed:
          EditorGUILayout.LabelField("Failed to check for updates.", HasteStyles.TipStyle);
          break;
        case HasteUpdateStatus.InProgress:
          EditorGUILayout.LabelField("Checking for updates...", HasteStyles.TipStyle);
          break;
      }
    }

    public IEnumerator Check() {
      HasteSettings.LastUpdateCheck = DateTime.Now.Ticks;

      if (Status == HasteUpdateStatus.InProgress) {
        // Don't check for updates while checking for updates
        yield break;
      }

      Status = HasteUpdateStatus.InProgress;
      Progress = 0.0f;

      WWW www = new WWW(HASTE_UPDATE_URL);
      while (!www.isDone) {
        Progress = www.progress;
        yield return null;
      }

      // Check for errors
      if (!String.IsNullOrEmpty(www.error)) {
        Status = HasteUpdateStatus.Failed;
        yield break;
      }

      // Try to parse the response as a version
      Version latestVersion;
      if (!HasteVersionUtils.TryParse(www.text, out latestVersion)) {
        Status = HasteUpdateStatus.Failed;
        yield break;
      }

      // Check if there's a newer version
      if (latestVersion > Haste.Version) {
        Status = HasteUpdateStatus.Available;
        yield break;
      }

      Status = HasteUpdateStatus.UpToDate;
    }
  }
}
