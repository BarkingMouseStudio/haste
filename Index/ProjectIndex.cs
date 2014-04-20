using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Haste {

  public class ProjectIndex : AbstractIndex {

    public override void Rebuild() {
      index.Clear();

      string[] assetPaths = Directory.GetFileSystemEntries(Application.dataPath);

      string assetPath, assetName, assetExtension;
      Item result;
      for (int i = 0; i < assetPaths.Length; i++) {
        assetPath = assetPaths[i];
        assetName = Path.GetFileName(assetPath);
        assetExtension = Path.GetExtension(assetPath);

        if (assetExtension == ".meta") {
          continue; // Ignore meta files
        }

        if (assetName.StartsWith(".")) {
          continue; // Ignore hidden files
        }

        result = new Item();
        result.Name = assetName;
        result.Path = "Assets/" + assetPath.TrimStart(Application.dataPath + "/");
        result.Source = Source.Project;
        result.Icon = AssetDatabase.GetCachedIcon(result.Path);

        index.Add(result);
      }
    }
  }
}
