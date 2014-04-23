using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {
  public class HasteFileWatcher : HasteWatcher {

    protected string watchPath;

    public HasteFileWatcher(string watchPath) : base() {
      this.watchPath = watchPath;
    }

    public HasteFileWatcher(string watchPath, IEnumerable<string> collection) : base(collection) {
      this.watchPath = watchPath;
    }

    public override IEnumerator GetEnumerator() {
      Queue<string> directories = new Queue<string>();
      HashSet<string> newCollection;

      // Poll indefinitely
      while (true) {
        // Initialize a new collection
        newCollection = new HashSet<string>();

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
            if (!collection.Contains(filePath)) {
              OnCreated(filePath);
            }

            newCollection.Add(filePath);
          }

          foreach (string directoryPath in Directory.GetDirectories(currentPath)) {
            if (Path.GetFileName(directoryPath).StartsWith(".")) {
              continue; // Ignore hidden files
            }

            directories.Enqueue(directoryPath);

            // If our original collection doesn't contain the directory, its new
            if (!collection.Contains(directoryPath)) {
              OnCreated(directoryPath);
            }

            newCollection.Add(directoryPath);
          }

          yield return null;
        }

        // Check for deleted paths
        foreach (string path in collection.Except(newCollection)) {
          // If an item from our original collection is not found
          // in our new collection, it has been removed.
          OnDeleted(path);
        }

        collection = newCollection;

        int ticks = 1000; // Wait about 10s
        while (ticks-- > 0) {
          yield return null;
        }
      }
    }
  }
}