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

    public LinkedListNode<IEnumerator> Start(IEnumerator fiber) {
      return coroutines.AddFirst(fiber);
    }

    public LinkedListNode<IEnumerator> Start(IEnumerable enumerable) {
      return coroutines.AddFirst(enumerable.GetEnumerator());
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
