using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {
  public class HasteHierarchyWatcher : HasteWatcher {

    public override IEnumerator GetEnumerator() {
      IDictionary<int, string> paths = new Dictionary<int, string>();

      HashSet<string> newCollection;

      // Poll indefinitely
      while (true) {
        // Initialize a new collection
        newCollection = new HashSet<string>();

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