using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public delegate void CreatedHandler(string path);
  public delegate void DeletedHandled(string path);

  public interface IHasteWatcher {
    event CreatedHandler Created;
    event DeletedHandled Deleted;
  }

  public abstract class HasteWatcher : IHasteWatcher, IEnumerable {

    public event CreatedHandler Created;
    public event DeletedHandled Deleted;

    protected HashSet<string> currentCollection; 
    protected HashSet<string> nextCollection; 

    protected HasteScheduler scheduler;

    bool shouldRestart = false;

    public HasteWatcher() {
      this.currentCollection = new HashSet<string>();
      this.nextCollection = new HashSet<string>();
      this.scheduler = new HasteScheduler();

      EditorApplication.playmodeStateChanged += PlaymodeStateChanged;
    }

    public virtual IEnumerator GetEnumerator() {
      return null;
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

    public void Start() {
      scheduler.Start(this);
    }

    void Stop() {
      scheduler.Stop();
    }

    void Clear() {
      // Forget everything
      currentCollection.Clear();
      nextCollection.Clear();
    }

    void Reset() {
      // Start again
      nextCollection.Clear();
    }

    void Restart() {
      Stop();
      shouldRestart = true;
    }

    public void ResetAndRestart() {
      Restart();
      Reset();
    }

    public void ClearAndRestart() {
      Restart();
      Clear();
    }

    void PlaymodeStateChanged() {
      Restart();
    }

    public void Tick() {
      scheduler.Tick();

      if (shouldRestart) {
        shouldRestart = false;
        Start();
      }
    }
  }
}