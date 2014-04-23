using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  // public class HasteCoroutine {
  //   public IEnumerator Fiber;
  //   public bool Done = false;
  //   public LinkedListNode<IEnumerator> Child;
  //   public HasteCoroutine(IEnumerator fiber) {
  //     Fiber = fiber;
  //   }
  // }

  public class HasteScheduler {

    private LinkedList<IEnumerator> coroutines;
   
    public HasteScheduler() {
      coroutines = new LinkedList<IEnumerator>();
    }

    /**
     * Starts a coroutine. The coroutine does not run immediately but on the
     * next call to Tick. The execution of a coroutine can
     * be paused at any point using the yield statement. The yield return value
     * specifies when the coroutine is resumed.
     */
    public LinkedListNode<IEnumerator> Start(IEnumerator fiber) {
      return coroutines.AddFirst(fiber);
    }
   
    /**
     * Stops all coroutines running on this behaviour. Use of this method is
     * discouraged, think of a natural way for your coroutines to finish
     * on their own instead of being forcefully stopped before they finish.
     * If you need finer control over stopping coroutines you can use multiple
     * schedulers.
     */
    public void Clear() {
      coroutines.Clear();
    }
   
    /**
     * Runs all active coroutines until their next yield. Caller must provide
     * the current frame and time. This allows for schedulers to run under
     * frame and time regimes other than the Unity's main game loop.
     */
    public void Tick() {
      LinkedListNode<IEnumerator> coroutine = coroutines.First;

      while (coroutine != null) {
        // Store listNext before coroutine finishes and is removed from the list
        LinkedListNode<IEnumerator> next = coroutine.Next;

        if (!coroutine.Value.MoveNext()) {
          coroutines.Remove(coroutine);
        }

        coroutine = next;
      }
    }
  }
}
