using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public delegate IEnumerable<HasteItem> HasteSourceFactory();

  public class HasteWatcherManager {

    static IDictionary<string, IHasteWatcher> watchers =
      new Dictionary<string, IHasteWatcher>();

    public void AddSource(string name, HasteSourceFactory factory) {
      if (!watchers.ContainsKey(name)) {
        IHasteWatcher watcher = new HasteWatcher(factory);

        watcher.Created += AddToIndex;
        watcher.Deleted += RemoveFromIndex;

        watcher.Start();

        watchers.Add(name, watcher);
      }
    }

    public void RemoveSource(string name) {
      IHasteWatcher watcher;

      if (watchers.TryGetValue(name, out watcher)) {
        watcher.Stop();

        watcher.Created -= AddToIndex;
        watcher.Deleted -= RemoveFromIndex;

        watchers.Remove(name);
      }
    }

    public void RestartSource(string name) {
      IHasteWatcher watcher;
      if (watchers.TryGetValue(name, out watcher)) {
        watcher.Restart();
      }
    }

    public void RestartAll() {
      foreach (IHasteWatcher watcher in watchers.Values) {
        watcher.Restart();
      }
    }

    void AddToIndex(HasteItem item) {
      Haste.Index.Add(item);
    }

    void RemoveFromIndex(HasteItem item) {
      Haste.Index.Remove(item);
    }
  }
}
