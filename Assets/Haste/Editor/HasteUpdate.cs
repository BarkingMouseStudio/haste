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

  public static class HasteUpdate {

    public static HasteUpdateStatus Status { get; private set; }

    public static float Progress { get; private set; }

    static HasteUpdate() {
      Status = HasteUpdateStatus.UpToDate;
      Progress = 0.0f;
    }

    public static IEnumerator CheckForUpdates() {
      if (Status == HasteUpdateStatus.InProgress) {
        yield break;
      }

      Status = HasteUpdateStatus.InProgress;
      Progress = 0.0f;

      WWW www = new WWW("http://www.google.com");
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
      if (!HasteVersion.TryParse(www.text, out latestVersion)) {
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
