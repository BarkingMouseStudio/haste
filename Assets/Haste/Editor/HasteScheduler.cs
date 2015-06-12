using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  // Stoppable co-routine scheduler node.
  public class HasteSchedulerNode {

    public IEnumerator Fiber { get; private set; }

    public bool IsStopping { get; private set; }

    public bool IsRunning { get { return !IsStopping; } }

    public HasteSchedulerNode(IEnumerator fiber) {
      Fiber = fiber;
    }

    public void Stop() {
      var current = Fiber.Current as HasteSchedulerNode;
      bool isWaiting = current != null ? current.IsRunning : false;
      if (isWaiting) { // Recursively stop any pending sub-coroutines.
        current.Stop();
      }
      IsStopping = true;
    }

    // Return whether or not it's still running
    public bool Next() {
      // Check if the coroutine yielded another coroutine
      var current = Fiber.Current as HasteSchedulerNode;

      // If it did, check if it's still running
      bool isWaiting = current != null ? current.IsRunning : false;

      // Don't advance while there's a sub-coroutine running
      if (isWaiting) {
        return true;
      }

      // Advance until we can't, then cleanup
      if (Fiber.MoveNext()) {
        return true;
      }

      Stop();
      return false;
    }

    public bool NextSync() {
      var current = Fiber.Current as HasteSchedulerNode;
      if (current != null && current.IsRunning) {
        while (current.NextSync());
      }
      return Fiber.MoveNext();
    }

    public void Sync() {
      while (NextSync());
    }
  }

  // Custom co-routine scheduler to support starting and stopping coroutines individually.
  public class HasteScheduler {

    readonly LinkedList<HasteSchedulerNode> coroutines;

    public bool IsRunning {
      get {
        return coroutines.Count > 0;
      }
    }

    public HasteScheduler() {
      coroutines = new LinkedList<HasteSchedulerNode>();
    }

    public static void Sync(IEnumerator fiber) {
      var node = new HasteSchedulerNode(fiber);
      node.Sync();
    }

    public static void Sync(IEnumerable enumerable) {
      var node = new HasteSchedulerNode(enumerable.GetEnumerator());
      node.Sync();
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
        } else if (!coroutine.Value.Next()) {
          coroutines.Remove(coroutine);
        }

        coroutine = next;
      }
    }
  }
}
