using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Haste {

  #if IS_HASTE_PRO
  public class HasteHierarchyActionSource : IEnumerable<HasteItem> {

    public static readonly string NAME = "HierarchyAction";

    public static string[] Actions = new string[]{
      "Focus",
      "Create Empty GameObject",
      "Reset Transform",
      "Clone",
      "Select Prefab",
      "Break Prefab",
      "Revert to Prefab",
      "Reconnect to Prefab",
      "Select Parent",
      "Select Children",
      "Delete"
    };

    public IEnumerator<HasteItem> GetEnumerator() {
      foreach (string path in Actions) {
        yield return new HasteItem(path, 0, NAME);
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
  #endif
}