using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public delegate void CreatedHandler(HasteItem item);
  public delegate void DeletedHandled(HasteItem item);

  public interface IHasteWatcher {
    event CreatedHandler Created;
    event DeletedHandled Deleted;

    void Start();
    void Stop();
    void Restart();
    void Purge();

    bool Enabled { get; set; }

    bool IsIndexing { get; }
    int IndexingCount { get; }
    int IndexedCount { get; }
  }

  public class HasteWatcher : IHasteWatcher, IEnumerable {

    // The maximum time an iteration can spend indexing
    // before a yield so we don't stall the editor.
    const float MAX_ITER_TIME = 4.0f / 1000.0f; // 4ms

    public event CreatedHandler Created;
    public event DeletedHandled Deleted;

    HashSet<HasteItem> currentCollection = new HashSet<HasteItem>();
    HashSet<HasteItem> nextCollection = new HashSet<HasteItem>();

    HasteSchedulerNode node;
    HasteSourceFactory factory;

    public HasteWatcher(HasteSourceFactory factory) {
      this.factory = factory;
    }

    public bool Enabled { get; set; }

    public bool IsIndexing {
      get { return node != null ? node.IsRunning : false; }
    }

    public int IndexingCount {
      get { return nextCollection.Count; }
    }

    public int IndexedCount {
      get { return currentCollection.Count; }
    }

    public void Start() {
      if (!IsIndexing) {
        nextCollection.Clear(); // Clear next to begin traversal again
        node = Haste.Scheduler.Start(this);
      }
    }

    public void Purge() {
      foreach (HasteItem item in currentCollection) {
        OnDeleted(item);
      }

      Stop();
    }

    public void Stop() {
      if (IsIndexing) {
        node.Stop();
      }

      // Clear both to free memory
      currentCollection.Clear();
      nextCollection.Clear();
    }

    public void Restart() {
      Stop();
      node = Haste.Scheduler.Start(this);
    }

    void OnCreated(HasteItem item) {
      if (Created != null) {
        Created(item);
      }
    }

    void OnDeleted(HasteItem item) {
      if (Deleted != null) {
        Deleted(item);
      }
    }

    public IEnumerator GetEnumerator() {
      float iterTime = 0.0f;

      foreach (HasteItem item in factory()) {
        if (!currentCollection.Contains(item)) {
          OnCreated(item);
        }

        nextCollection.Add(item);

        if (iterTime > MAX_ITER_TIME) {
          iterTime = 0.0f;
          yield return null;
        }

        iterTime += Time.deltaTime;
      }

      // Check for deleted paths
      foreach (HasteItem item in currentCollection) {
        // If an item from our original collection is not found
        // in our new collection, it has been removed.
        if (!nextCollection.Contains(item)) {
          OnDeleted(item);
        }

        if (iterTime > MAX_ITER_TIME) {
          iterTime = 0.0f;
          yield return null;
        }

        iterTime += Time.deltaTime;
      }

      var temp = currentCollection;
      currentCollection = nextCollection;

      nextCollection = temp;
      nextCollection.Clear(); // We clear it when we're done (not at the beginning in case something was added)

      node = null;
    }
  }
}