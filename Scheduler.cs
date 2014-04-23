using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

namespace Haste {

  // http://wiki.unity3d.com/index.php?title=CoroutineScheduler
  public class CoroutineScheduler {

    private LinkedList<IEnumerator> coroutines;
   
    public CoroutineScheduler() {
      coroutines = new LinkedList<IEnumerator>();

      EditorApplication.update += UpdateAllCoroutines;
    }

    /**
     * Starts a coroutine, the coroutine does not run immediately but on the
     * next call to UpdateAllCoroutines. The execution of a coroutine can
     * be paused at any point using the yield statement. The yield return value
     * specifies when the coroutine is resumed.
     */
    public LinkedListNode<IEnumerator> StartCoroutine(IEnumerator fiber) {
      // Create coroutine node and run until we reach first yield
      return coroutines.AddFirst(fiber);
    }
   
    /**
     * Stops all coroutines running on this behaviour. Use of this method is
     * discouraged, think of a natural way for your coroutines to finish
     * on their own instead of being forcefully stopped before they finish.
     * If you need finer control over stopping coroutines you can use multiple
     * schedulers.
     */
    public void StopAllCoroutines() {
      coroutines.Clear();
    }
   
    /**
     * Returns true if this scheduler has any coroutines. You can use this to
     * check if all coroutines have finished or been stopped.
     */
    public bool HasCoroutines() {
      return coroutines.Count > 0;
    }
   
    /**
     * Runs all active coroutines until their next yield. Caller must provide
     * the current frame and time. This allows for schedulers to run under
     * frame and time regimes other than the Unity's main game loop.
     */
    public void UpdateAllCoroutines() {
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
