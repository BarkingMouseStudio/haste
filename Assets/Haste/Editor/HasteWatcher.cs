using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public delegate void CreatedHandler(string path);
  public delegate void DeletedHandled(string path);

  public interface IHasteWatcher : IEnumerable {
    event CreatedHandler Created;
    event DeletedHandled Deleted;

    bool IsRunning { get; }
  }

  public abstract class HasteWatcher : IHasteWatcher {

    public event CreatedHandler Created;
    public event DeletedHandled Deleted;

    protected HashSet<string> currentCollection; 
    protected HashSet<string> nextCollection; 

    public bool IsRunning { get; protected set; }

    public virtual void Add(string path) {}

    public virtual void Clear() {
      currentCollection.Clear();
      nextCollection.Clear();
    }

    public virtual void Restart() {
      nextCollection.Clear();
    }

    protected void OnCreated(string path) {
      if (Created != null) {
        Created(path);
      }
    }

    protected void OnDeleted(string path) {
      if (Deleted != null) {
        Deleted(path);
      }
    }

    public HasteWatcher() {
      this.currentCollection = new HashSet<string>();
      this.nextCollection = new HashSet<string>();
    }

    public HasteWatcher(IEnumerable<string> collection) {
      this.currentCollection = new HashSet<string>(collection);
      this.nextCollection = new HashSet<string>();
    }

    public virtual IEnumerator GetEnumerator() {
      return null;
    }
  }
}