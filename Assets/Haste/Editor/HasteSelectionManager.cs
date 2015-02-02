using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Haste {

  public static class HasteSelectionManager  {

    private static Stack<int[]> selections = new Stack<int[]>(1);

    public static void Save() {
      selections.Push(Selection.instanceIDs);
    }

    public static void Restore() {
      Selection.instanceIDs = selections.Pop();
    }
  }
}
