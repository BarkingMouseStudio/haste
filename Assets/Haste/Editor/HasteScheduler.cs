using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  internal class HasteScheduler {

    private LinkedList<IEnumerator> coroutines;
   
    internal bool IsRunning {
      get {
        return coroutines.Count > 0;
      }
    }

    internal HasteScheduler() {
      coroutines = new LinkedList<IEnumerator>();
    }

    internal void Start(IEnumerator fiber) {
      coroutines.AddFirst(fiber);
    }

    internal void Start(IEnumerable enumerable) {
      coroutines.AddFirst(enumerable.GetEnumerator());
    }

    internal void Stop() {
      coroutines.Clear();
    }
   
    internal void Tick() {
      LinkedListNode<IEnumerator> coroutine = coroutines.First;

      while (coroutine != null) {
        LinkedListNode<IEnumerator> next = coroutine.Next;

        if (!coroutine.Value.Enumerator.MoveNext()) {
          coroutines.Remove(coroutine);
        }

        coroutine = next;
      }
    }
  }
}
