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
    void Reset();
    void Restart();
    void Tick();
  }

  public abstract class HasteWatcher : IHasteWatcher, IEnumerable {

    public event CreatedHandler Created;
    public event DeletedHandled Deleted;

    protected HasteScheduler scheduler;

    bool shouldRestart = false;

    public HasteWatcher() {
      this.scheduler = new HasteScheduler();

      EditorApplication.playmodeStateChanged += PlaymodeStateChanged;
    }

    protected void OnCreated(HasteItem item) {
      if (Created != null) {
        Created(item);
      }
    }

    protected void OnDeleted(HasteItem item) {
      if (Deleted != null) {
        Deleted(item);
      }
    }

    public abstract void Reset();

    public abstract IEnumerator GetEnumerator();

    public void Start() {
      scheduler.Start(this);
    }

    public void Restart() {
      scheduler.Stop();
      shouldRestart = true;
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