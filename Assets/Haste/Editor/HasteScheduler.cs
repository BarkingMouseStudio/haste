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
      IsStopping = true;
    }
  }

  // Custom co-routine scheduler to support starting and stopping coroutines individually.
  public class HasteScheduler {

    private readonly LinkedList<HasteSchedulerNode> coroutines;

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
          // Check if the coroutine yielded another coroutine
          var current = coroutine.Value.Fiber.Current as HasteSchedulerNode;

          // If it did, check if it's still running
          bool isWaiting = current != null && current.IsRunning;
          // UnityEngine.Debug.Log(coroutine.GetHashCode() + " " + (current != null ? current.GetHashCode() : 0) + " " + isWaiting);


          // Don't advance while there's a sub-coroutine running
          if (!isWaiting) {
            // Advance until we can't, then cleanup
            if (!coroutine.Value.Fiber.MoveNext()) {
              coroutine.Value.Stop();
              coroutines.Remove(coroutine);
            }
          }
        }

        coroutine = next;
      }
    }
  }
}
