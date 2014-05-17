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

    bool IsRunning { get; }
  }

  public class HasteWatcher : IHasteWatcher, IEnumerable {

    public event CreatedHandler Created;
    public event DeletedHandled Deleted;

    HashSet<HasteItem> currentCollection = new HashSet<HasteItem>();
    HashSet<HasteItem> nextCollection = new HashSet<HasteItem>();

    HasteSchedulerNode node;
    SourceFactory factory;

    public HasteWatcher(SourceFactory factory) {
      this.factory = factory;
    }

    public bool IsRunning {
      get {
        return node != null ? node.IsRunning : false;
      }
    }

    public void Start() {
      if (!IsRunning) {
        nextCollection.Clear(); // Clear next to begin traversal again
        node = Haste.Scheduler.Start(this);
      }
    }

    public void Stop() {
      if (IsRunning) {
        node.Stop();

        // Clear both to free memory
        currentCollection.Clear();
        nextCollection.Clear();
      }
    }

    public void Restart() {
      if (IsRunning) {
        node.Stop();
      }

      nextCollection.Clear(); // Clear next to begin traversal again
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
      foreach (HasteItem item in factory()) {
        if (!currentCollection.Contains(item)) {
          OnCreated(item);
        }

        yield return null;
      }

      // Check for deleted paths
      foreach (HasteItem item in currentCollection) {
        // If an item from our original collection is not found
        // in our new collection, it has been removed.
        if (!nextCollection.Contains(item)) {
          OnDeleted(item);
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