using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class DownloadsIndex : AbstractIndex {

    public override void Rebuild() {
      index.Clear();

      string downloadsPath = Path.Combine(Utils.GetHomeFolder(), "Library/Unity/Asset Store");

      foreach (string download in Directory.GetFileSystemEntries(downloadsPath)) {
        index.Add(new Item(Path.GetFileName(download), download, Source.Downloads));
      }
    }
  }
}
