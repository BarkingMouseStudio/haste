using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public class HasteProjectSource : IEnumerable<HasteItem> {

    public IEnumerator<HasteItem> GetEnumerator() {
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

          yield return new HasteItem(
            HasteUtils.GetRelativeAssetPath(filePath),
            0,
            HasteSource.Project,
            AssetDatabase.GetCachedIcon(filePath));
        }

        foreach (string directoryPath in Directory.GetDirectories(currentPath)) {
          if (Path.GetFileName(directoryPath).StartsWith(".")) {
            continue; // Ignore hidden files
          }

          directories.Enqueue(directoryPath);

          yield return new HasteItem(
            HasteUtils.GetRelativeAssetPath(directoryPath),
            0,
            HasteSource.Project,
            AssetDatabase.GetCachedIcon(directoryPath));
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}