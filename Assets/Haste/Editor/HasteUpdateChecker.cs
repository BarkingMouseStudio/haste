using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public enum HasteUpdateStatus {
    UpToDate,
    Available,
    Failed,
    InProgress
  }

  public class HasteUpdateChecker {

    static readonly string HASTE_UPDATE_URL =
      "http://barkingmousestudio.com/haste-version/version.json";

    public TimeSpan Interval { get; private set; }
    public HasteUpdateStatus Status { get; private set; }
    public float Progress { get; private set; }

    public HasteUpdateChecker() {
      Status = HasteUpdateStatus.UpToDate;
      Progress = 0.0f;
      Interval = TimeSpan.FromHours(1);
    }

    public IEnumerator Check() {
      if (!HasteSettings.CheckForUpdates) {
        // Disabled
        yield break;
      }

      if (DateTime.Now < HasteSettings.LastUpdateCheckDate.Add(Interval)) {
        // Too soon
        yield break;
      }

      if (Status == HasteUpdateStatus.InProgress) {
        // Don't check for updates while checking for updates
        yield break;
      }

      HasteSettings.LastUpdateCheck = DateTime.Now.Ticks;

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
