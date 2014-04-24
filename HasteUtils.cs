using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

namespace Haste {

  public static class HasteUtils {

    public static string GetHomeFolder() {
      return String.Join(
        Path.DirectorySeparatorChar.ToString(),
        Application.dataPath.Split(Path.DirectorySeparatorChar)
          .Slice(0, 3)
      );
    }

    public static string GetRelativeAssetPath(string assetPath) {
      return "Assets/" + assetPath.TrimStart(Application.dataPath + "/");
    }
  }
}
