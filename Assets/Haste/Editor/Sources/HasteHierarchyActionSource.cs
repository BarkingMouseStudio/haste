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
      "GameObject/Lock",
      "GameObject/Unlock",
      "GameObject/Activate",
      "GameObject/Deactivate",
      "GameObject/Reset Transform",
      "GameObject/Select Parent",
      "GameObject/Select Children",

      // Prefab
      "GameObject/Select Prefab",
      "GameObject/Revert to Prefab",
      "GameObject/Reconnect to Prefab"
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