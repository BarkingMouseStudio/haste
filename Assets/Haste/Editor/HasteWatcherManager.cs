using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public delegate IEnumerable<HasteItem> HasteSourceFactory();

  // Manager for various source watchers.
  public class HasteWatcherManager : IEnumerable<KeyValuePair<string, IHasteWatcher>> {

    public bool IsIndexing {
      get { return watchers.Any(w => w.Value.IsIndexing); }
    }

    public int IndexingCount {
      get { return watchers.Sum(w => w.Value.IndexingCount); }
    }

    public int IndexedCount {
      get { return watchers.Sum(w => w.Value.IndexedCount); }
    }

    public ICollection<string> Keys {
      get { return watchers.Keys; }
    }

    static IDictionary<string, IHasteWatcher> watchers =
      new Dictionary<string, IHasteWatcher>();

    void StartSource(IHasteWatcher watcher) {
      watcher.Created += AddToIndex;
      watcher.Deleted += RemoveFromIndex;

      watcher.Start();
    }

    void StopSource(IHasteWatcher watcher, bool purge = false) {
      if (purge) {
        watcher.Purge();
      } else {
        watcher.Stop();
      }

      watcher.Created -= AddToIndex;
      watcher.Deleted -= RemoveFromIndex;
    }

    public void ToggleSource(string name, bool enabled) {
      IHasteWatcher watcher;
      if (watchers.TryGetValue(name, out watcher)) {

        // State changed
        if (enabled != watcher.Enabled) {
          watcher.Enabled = enabled;

          if (enabled) {
            StartSource(watcher);
          } else {
            StopSource(watcher, true);
          }
        }
      }
    }

    public void AddSource(string name, bool enabled, HasteSourceFactory factory) {
      if (!watchers.ContainsKey(name)) {
        IHasteWatcher watcher = new HasteWatcher(factory);
        watcher.Enabled = enabled;

        if (watcher.Enabled) {
          StartSource(watcher);
        }

        watchers.Add(name, watcher);
      }
    }

    public void RemoveSource(string name) {
      IHasteWatcher watcher;
      if (watchers.TryGetValue(name, out watcher)) {
        StopSource(watcher);

        watchers.Remove(name);
      }
    }

    public void RestartSource(string name) {
      IHasteWatcher watcher;
      if (watchers.TryGetValue(name, out watcher)) {
        if (watcher.Enabled) {
          watcher.Restart();
        }
      }
    }

    public void Stop() {
      foreach (IHasteWatcher watcher in watchers.Values) {
        watcher.Stop();
      }
    }

    public void Rebuild() {
      foreach (IHasteWatcher watcher in watchers.Values) {
        if (watcher.Enabled) {
          watcher.Rebuild();
        }
      }
    }

    void AddToIndex(HasteItem item) {
      Haste.Index.Add(item);
    }

    void RemoveFromIndex(HasteItem item) {
      Haste.Index.Remove(item);
    }

    public IEnumerator<KeyValuePair<string, IHasteWatcher>> GetEnumerator() {
      foreach (var watcher in watchers) {
        yield return watcher;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}
