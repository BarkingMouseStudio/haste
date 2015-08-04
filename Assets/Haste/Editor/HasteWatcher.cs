using UnityEditor;
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
    void Rebuild();
    void Purge();

    bool Enabled { get; set; }

    bool IsIndexing { get; }
    int IndexingCount { get; }
    int IndexedCount { get; }
  }

  public class HasteWatcher : IHasteWatcher, IEnumerable {

    public event CreatedHandler Created;
    public event DeletedHandled Deleted;

    HashSet<HasteItem> currentCollection = new HashSet<HasteItem>();
    HashSet<HasteItem> nextCollection = new HashSet<HasteItem>();

    readonly HasteSourceFactory factory;
    HasteSchedulerNode node;

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
        node = null;
      }

      // Clear both to free memory
      currentCollection.Clear();
      nextCollection.Clear();
    }

    public void Restart() {
      if (IsIndexing) {
        node.Stop();
        node = null;
      }

      // Don't clear current so we can collect change events
      nextCollection.Clear();
      node = Haste.Scheduler.Start(this);
    }

    public void Rebuild() {
      // Clear both on rebuild, this won't trigger the appropriate
      // change events which is fine since the index should be
      // cleared first.
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
      double startTime = EditorApplication.timeSinceStartup;

      foreach (HasteItem item in factory()) {
        if (!currentCollection.Contains(item)) {
          OnCreated(item);
        }

        nextCollection.Add(item);

        if (EditorApplication.timeSinceStartup - startTime >= Haste.MAX_ITER_TIME) {
          startTime = EditorApplication.timeSinceStartup;
          yield return null;
        }
      }

      startTime = EditorApplication.timeSinceStartup;

      // Check for deleted paths
      foreach (HasteItem item in currentCollection) {
        // If an item from our original collection is not found
        // in our new collection, it has been removed.
        if (!nextCollection.Contains(item)) {
          OnDeleted(item);
        }

        if (EditorApplication.timeSinceStartup - startTime >= Haste.MAX_ITER_TIME) {
          startTime = EditorApplication.timeSinceStartup;
          yield return null;
        }
      }

      var temp = currentCollection;
      currentCollection = nextCollection;

      nextCollection = temp;
      nextCollection.Clear(); // We clear it when we're done (not at the beginning in case something was added)

      node = null;
    }
  }
}
