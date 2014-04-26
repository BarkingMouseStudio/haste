using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteSchedulerNode {

    public string Name;
    public IEnumerator Enumerator;

    public HasteSchedulerNode(IEnumerator enumerator, string name) {
      Enumerator = enumerator;
      Name = name;
    }
  }

  public class HasteScheduler {

    private LinkedList<HasteSchedulerNode> coroutines;
    private HashSet<string> stopping;
   
    public bool IsRunning {
      get {
        return coroutines.Count > 0;
      }
    }

    public HasteScheduler() {
      stopping = new HashSet<string>();
      coroutines = new LinkedList<HasteSchedulerNode>();
    }

    public void Start(IEnumerator fiber, string name = "") {
      coroutines.AddFirst(new HasteSchedulerNode(fiber, name));
    }

    public void Start(IEnumerable enumerable, string name = "") {
      coroutines.AddFirst(new HasteSchedulerNode(enumerable.GetEnumerator(), name));
    }

    public void Stop(string name) {
      stopping.Add(name);
    }

    public void StopAll() {
      coroutines.Clear();
    }
   
    public void Tick() {
      LinkedListNode<HasteSchedulerNode> coroutine = coroutines.First;

      while (coroutine != null) {
        LinkedListNode<HasteSchedulerNode> next = coroutine.Next;

        if (coroutine.Value.Name != "" && stopping.Contains(coroutine.Value.Name)) {
          coroutines.Remove(coroutine);
        } else if (!coroutine.Value.Enumerator.MoveNext()) {
          coroutines.Remove(coroutine);
        }

        coroutine = next;
      }

      stopping.Clear();
    }
  }
}
