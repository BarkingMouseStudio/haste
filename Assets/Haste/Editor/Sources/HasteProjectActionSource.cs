using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Haste {

  #if IS_HASTE_PRO
  public class HasteProjectActionSource : IEnumerable<HasteItem> {

    public static readonly string NAME = "ProjectAction";

    public static string[] Actions = new string[]{
      "Assets/Open",
      "Assets/Instantiate Prefab"
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