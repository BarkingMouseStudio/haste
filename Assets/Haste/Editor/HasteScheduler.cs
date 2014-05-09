using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteSchedulerNode {
    public IEnumerator Fiber;
    public bool IsStopping;

    public HasteSchedulerNode(IEnumerator fiber) {
      Fiber = fiber;
    }

    public void Stop() {
      IsStopping = true;
    }
  }

  public class HasteScheduler {

    private LinkedList<HasteSchedulerNode> coroutines;
   
    public bool IsRunning {
      get {
        return coroutines.Count > 0;
      }
    }

    public HasteScheduler() {
      coroutines = new LinkedList<HasteSchedulerNode>();
    }

    public HasteSchedulerNode Start(IEnumerator fiber) {
      var node = new HasteSchedulerNode(fiber);
      coroutines.AddFirst(node);
      return node;
    }

    public HasteSchedulerNode Start(IEnumerable enumerable) {
      var node = new HasteSchedulerNode(enumerable.GetEnumerator());
      coroutines.AddFirst(node);
      return node;
    }

    public void Stop() {
      coroutines.Clear();
    }
   
    public void Tick() {
      LinkedListNode<HasteSchedulerNode> coroutine = coroutines.First;

      while (coroutine != null) {
        LinkedListNode<HasteSchedulerNode> next = coroutine.Next;

        // Remove the coroutine if its marked as stopping
        if (coroutine.Value.IsStopping) {
          coroutines.Remove(coroutine);
        } else {
          if (!coroutine.Value.Fiber.MoveNext()) {
            coroutines.Remove(coroutine);
          }
        }

        coroutine = next;
      }
    }
  }
}
