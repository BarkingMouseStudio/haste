using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Haste {

  public static class HasteUtils {

    public static Texture GetIconForSource(HasteSource source, string path) {
      switch (source) {
        case HasteSource.Hierarchy:
          return EditorGUIUtility.ObjectContent(null, typeof(GameObject)).image;
        case HasteSource.Project:
          return AssetDatabase.GetCachedIcon(path);
        default:
          return null;
      }
    }

    public static string GetHomeFolder() {
      return String.Join(
        Path.DirectorySeparatorChar.ToString(),
        Application.dataPath.Split(Path.DirectorySeparatorChar)
          .Slice(0, 3)
      );
    }

    public static HasteResult GetResultFromObject(UnityEngine.Object obj) {
      if (AssetDatabase.Contains(obj)) {
        return new HasteResult(AssetDatabase.GetAssetPath(obj), HasteSource.Project);
      } else if (obj.GetType() == typeof(Transform)) {
        Transform transform = (Transform)obj;
        return new HasteResult(HasteUtils.GetHierarchyPath(transform), HasteSource.Hierarchy);
      } else if (obj.GetType() == typeof(GameObject)) {
        GameObject go = (GameObject)obj;
        return new HasteResult(HasteUtils.GetHierarchyPath(go.transform), HasteSource.Hierarchy);
      } else {
        return new HasteResult(obj.name, HasteSource.Unknown);
      }
    }

    public static HasteResult[] GetResultsFromObjects(UnityEngine.Object[] objects) {
      return objects.Select(obj => GetResultFromObject(obj)).ToArray();
    }

    public static Regex GetFuzzyFilterRegex(string query) {
      return new Regex(String.Join(".*", query.ToLower().ToCharArray().Select(c => c.ToString()).ToArray()));
    }

    public static string GetRelativeAssetPath(string assetPath) {
      return "Assets/" + assetPath.TrimStart(Application.dataPath + "/");
    }

    public static string GetHierarchyPath(Transform transform) {
      string path;

      if (transform.parent == null) {
        path = transform.gameObject.name;
      } else {
        path = HasteUtils.GetHierarchyPath(transform.parent) +
          Path.DirectorySeparatorChar + transform.gameObject.name;
      }

      return path;
    }
  }
}
