using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteScheduler {

    private LinkedList<IEnumerator> coroutines;
   
    public bool IsRunning {
      get {
        return coroutines.Count > 0;
      }
    }

    public HasteScheduler() {
      coroutines = new LinkedList<IEnumerator>();
    }

    public void Start(IEnumerator fiber) {
      coroutines.AddFirst(fiber);
    }

    public void Start(IEnumerable enumerable) {
      coroutines.AddFirst(enumerable.GetEnumerator());
    }

    public void Stop() {
      coroutines.Clear();
    }
   
    public void Tick() {
      LinkedListNode<IEnumerator> coroutine = coroutines.First;

      while (coroutine != null) {
        LinkedListNode<IEnumerator> next = coroutine.Next;

        if (!coroutine.Value.MoveNext()) {
          coroutines.Remove(coroutine);
        }

        coroutine = next;
      }
    }
  }
}
