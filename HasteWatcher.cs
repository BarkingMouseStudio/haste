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

    protected HashSet<string> collection; 

    public bool IsRunning { get; protected set; }

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
      this.collection = new HashSet<string>();
    }

    public HasteWatcher(IEnumerable<string> collection) {
      this.collection = new HashSet<string>(collection);
    }

    public virtual IEnumerator GetEnumerator() {
      return null;
    }
  }
}