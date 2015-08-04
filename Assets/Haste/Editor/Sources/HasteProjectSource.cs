using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public class HasteProjectSource : IEnumerable<HasteItem> {

    public const string NAME = "Project";

    static bool IsIgnored(string[] ignorePaths, string path) {
      foreach (var ignorePath in ignorePaths) {
        if (path.IndexOf(ignorePath) == 0) {
          return true;
        }
      }
      return false;
    }

    static string GetNormalizedAssetPath(string assetPath) {
      var path = Path.Combine("Assets", assetPath.TrimStart(Application.dataPath + Path.DirectorySeparatorChar));
      // Normalize Windows paths
      if (Path.DirectorySeparatorChar != '/') {
        path = path.Replace(Path.DirectorySeparatorChar, '/');
      }
      return path;
    }

    public IEnumerator<HasteItem> GetEnumerator() {
      var ignorePaths = HasteSettings.IgnorePaths.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries)
        .Select((s) => s.Trim())
        .ToArray();

      Queue<string> directories = new Queue<string>();

      // Start with our top-level directory
      directories.Enqueue(Application.dataPath);

      // Traverse all directories
      while (directories.Count > 0) {
        // Traverse current directory
        string currentPath = directories.Dequeue();

        foreach (string filePath in Directory.GetFiles(currentPath)) {
          if (Path.GetExtension(filePath) == ".meta") {
            continue; // Ignore meta files
          }

          if (Path.GetFileName(filePath).StartsWith(".")) {
            continue; // Ignore hidden files
          }

          string path = GetNormalizedAssetPath(filePath);
          if (IsIgnored(ignorePaths, path)) {
            continue;
          }

          yield return new HasteItem(path, 0, NAME);
        }

        foreach (string directoryPath in Directory.GetDirectories(currentPath)) {
          if (Path.GetFileName(directoryPath).StartsWith(".")) {
            continue; // Ignore hidden files
          }

          string path = GetNormalizedAssetPath(directoryPath);
          if (IsIgnored(ignorePaths, path)) {
            continue;
          }

          directories.Enqueue(directoryPath);

          yield return new HasteItem(path, 0, NAME);
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}
