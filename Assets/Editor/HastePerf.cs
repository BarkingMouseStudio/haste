using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Haste {

  public static class HastePerf {

    [MenuItem("Window/Populate GameObjects")]
    public static void PopulateGameObjects() {
      GameObject top = new GameObject("Top");

      // TODO: Variation of depth, names and distribution
      GameObject parent, child;
      for (var i = 0; i < 10; i++) {
        parent = new GameObject("Parent");
        parent.transform.parent = top.transform;

        for (var j = 0; j < 10; j++) {
          child = new GameObject("Child");
          child.transform.parent = parent.transform;
        }
      }
    }

    [MenuItem("Window/Populate Assets")]
    public static void PopulateAssets() {
      // TODO: Random file names with word boundaries
      var testPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder("Assets", "PerfTest"));
      var asset = HasteUtils.CreateColorSwatch(Color.black);
      var assetPath = testPath + Path.DirectorySeparatorChar + "texture.png";
      AssetDatabase.CreateAsset(asset, assetPath);
    }
  }
}
