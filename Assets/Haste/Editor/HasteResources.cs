using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Haste {

  public static class HasteResources {

    private static readonly string partialPath = "/Haste/Editor/InternalResources/";

    private static string internalResourcesPath = "";
    public static string InternalResourcesPath {
      get {
        if (string.IsNullOrEmpty(internalResourcesPath)) {
          string path;
          if (FindInternalResourcesPath(out path)) {
            internalResourcesPath = path;
          } else {
            Debug.LogError("[Haste] Unable to locate the internal resources folder. Make sure your Haste installation is intact.");
            HasteWindow.Instance.Close();
          }
        }
        return internalResourcesPath;
      }
    }

    public static T Load<T>(string path) where T : UnityEngine.Object {
      var asset = (T)AssetDatabase.LoadAssetAtPath(InternalResourcesPath + path, typeof(T));
      asset.hideFlags = HideFlags.HideAndDontSave;
      return asset;
    }

    private static bool FindInternalResourcesPath(out string path) {
      foreach (string assetPath in AssetDatabase.GetAllAssetPaths()) {
        var index = assetPath.IndexOf(partialPath);
        if (index != -1) {
          path = assetPath.Substring(0, index) + partialPath;
          return true;
        }
      }

      path = "";
      return false;
    }
  }
}
