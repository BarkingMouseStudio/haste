using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Haste {

  public static class HasteIgnore {

    static List<string> GetIgnoredPaths() {
      return HasteSettings.IgnorePaths.Trim().Split(',')
        .Select((path) => path.Trim())
        .Where((path) => !string.IsNullOrEmpty(path))
        .ToList();
    }

    static void SaveIgnoredPaths(string[] paths) {
      HasteSettings.IgnorePaths = string.Join(",", paths);
    }

    static bool IgnorePath(string path) {
      var paths = GetIgnoredPaths();
      var count = paths.Count;
      paths.Add(path);

      var pathsArr = paths.Distinct().ToArray();
      SaveIgnoredPaths(pathsArr);
      return count != pathsArr.Length;
    }

    static bool UnignorePath(string path) {
      var paths = GetIgnoredPaths();
      var count = paths.Count;
      paths.Remove(path);

      var pathsArr = paths.Distinct().ToArray();
      SaveIgnoredPaths(pathsArr);
      return count != pathsArr.Length;
    }

    static bool IsIgnored(string path) {
      return GetIgnoredPaths().Contains(path);
    }

    [MenuItem("Assets/Haste/Ignore")]
    public static void Ignore() {
      var path = AssetDatabase.GetAssetPath(Selection.activeObject);
      if (IgnorePath(path)) {
        Haste.Rebuild();
      }
    }

    [MenuItem("Assets/Haste/Ignore", true)]
    public static bool CanIgnore() {
      var selection = Selection.activeObject;
      if (selection == null) {
        return false;
      }
      if (!AssetDatabase.Contains(selection)) {
        return false; // invalid asset
      }
      var path = AssetDatabase.GetAssetPath(selection);
      if (!Directory.Exists(path)) {
        return false; // invalid directory
      }
      if (IsIgnored(path)) {
        return false; // already ignored
      }
      return true;
    }

    [MenuItem("Assets/Haste/Unignore")]
    public static void Unignore() {
      var path = AssetDatabase.GetAssetPath(Selection.activeObject);
      if (UnignorePath(path)) {
        Haste.Rebuild();
      }
    }

    [MenuItem("Assets/Haste/Unignore", true)]
    public static bool CanUnignore() {
      var selection = Selection.activeObject;
      if (selection == null) {
        return false;
      }
      if (!AssetDatabase.Contains(selection)) {
        return false; // invalid asset
      }
      var path = AssetDatabase.GetAssetPath(selection);
      if (!Directory.Exists(path)) {
        return false; // invalid directory
      }
      if (!IsIgnored(path)) {
        return false; // not ignored
      }
      return true;
    }
  }
}
