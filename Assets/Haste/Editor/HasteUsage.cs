#define IS_HASTE_PRO

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public static class HasteUsage {

    private static int count = -1;
    public static int Count {
      get {
        if (count < 0) {
          count = EditorPrefs.GetInt("HasteUsageCount", 0);
        }
        return count;
      }
      set {
        count = value;
        EditorPrefs.SetInt("HasteUsageCount", count);
      }
    }
  }
}
