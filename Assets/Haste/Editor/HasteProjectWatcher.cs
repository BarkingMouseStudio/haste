using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public class HasteProjectWatcher : HasteWatcher {

    protected HashSet<string> currentCollection; 
    protected HashSet<string> nextCollection; 

    public HasteProjectWatcher() {
      currentCollection = new HashSet<string>();
      nextCollection = new HashSet<string>();

      EditorApplication.projectWindowChanged += ProjectWindowChanged;
    }

    public override void Reset() {
      // Forget everything
      currentCollection.Clear();
      nextCollection.Clear();
    }

    void ProjectWindowChanged() {
      // Start again
      nextCollection.Clear();
      Restart();
    }

    public override IEnumerator GetEnumerator() {
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

          // If our original collection doesn't contain the file, its new
          if (!currentCollection.Contains(filePath)) {
            OnCreated(new HasteItem(HasteUtils.GetRelativeAssetPath(filePath), 0, HasteSource.Project));
          }

          nextCollection.Add(filePath);
        }

        foreach (string directoryPath in Directory.GetDirectories(currentPath)) {
          if (Path.GetFileName(directoryPath).StartsWith(".")) {
            continue; // Ignore hidden files
          }

          directories.Enqueue(directoryPath);

          // If our original collection doesn't contain the directory, its new
          if (!currentCollection.Contains(directoryPath)) {
            OnCreated(new HasteItem(HasteUtils.GetRelativeAssetPath(directoryPath), 0, HasteSource.Project));
          }

          nextCollection.Add(directoryPath);
        }

        yield return null;
      }

      // Check for deleted paths
      foreach (string path in currentCollection) {
        // If an item from our original collection is not found
        // in our new collection, it has been removed.
        if (!nextCollection.Contains(path)) {
          OnDeleted(new HasteItem(path, 0, HasteSource.Project));
        }

        yield return null;
      }

      var temp = currentCollection;
      currentCollection = nextCollection;

      nextCollection = temp;
      nextCollection.Clear(); // We clear it when we're done (not at the beginning in case something was added)
    }
  }
}