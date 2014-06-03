using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public delegate IEnumerable<HasteItem> HasteSourceFactory();

  public class HasteWatcherManager {

    public bool IsIndexing {
      get { return watchers.Any(w => w.Value.IsIndexing); }
    }

    public int IndexingCount {
      get { return watchers.Sum(w => w.Value.IndexingCount); }
    }

    public ICollection<string> Keys {
      get { return watchers.Keys; }
    }

    static IDictionary<string, IHasteWatcher> watchers =
      new Dictionary<string, IHasteWatcher>();

    string GetPrefKey(string name) {
      return String.Format("Haste:{0}", name);
    }

    void StartSource(IHasteWatcher watcher) {
      watcher.Created += AddToIndex;
      watcher.Deleted += RemoveFromIndex;

      watcher.Start();
    }

    void StopSource(IHasteWatcher watcher) {
      watcher.Stop();

      watcher.Created -= AddToIndex;
      watcher.Deleted -= RemoveFromIndex;
    }

    public void ToggleSource(string name, bool enabled) {
      IHasteWatcher watcher;
      if (watchers.TryGetValue(name, out watcher)) {

        // State changed
        if (enabled != watcher.Enabled) {
          watcher.Enabled = enabled;
          EditorPrefs.SetBool(GetPrefKey(name), enabled);

          if (enabled) {
            StartSource(watcher);
          } else {
            watcher.Purge();
            StopSource(watcher);
          }
        }
      }
    }

    public void AddSource(string name, HasteSourceFactory factory) {
      if (!watchers.ContainsKey(name)) {
        IHasteWatcher watcher = new HasteWatcher(factory);
        watcher.Enabled = EditorPrefs.GetBool(GetPrefKey(name), true);

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

    public bool TryGetWatcher(string name, out IHasteWatcher watcher) {
      return watchers.TryGetValue(name, out watcher);
    }

    public void RestartAll() {
      foreach (IHasteWatcher watcher in watchers.Values) {
        if (watcher.Enabled) {
          watcher.Restart();
        }
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
