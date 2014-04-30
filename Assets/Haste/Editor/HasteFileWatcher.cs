using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public class HasteFileWatcher : HasteWatcher, IEnumerable {

    string watchPath;

    public HasteFileWatcher(string watchPath) : base() {
      this.watchPath = watchPath;
    }

    public override IEnumerator GetEnumerator() {
      Queue<string> directories = new Queue<string>();

      // Start with our top-level directory
      directories.Enqueue(watchPath);

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
            OnCreated(filePath);
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
            OnCreated(directoryPath);
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
          OnDeleted(path);
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