#define DEBUG

using UnityEngine;
using System;
using System.Linq;
using System.Collections;

namespace Haste {

  public static class HasteLogger {

    [System.Diagnostics.Conditional("DEBUG")]
    public static void Info(params object[] args) {
      string[] sargs = ((IEnumerable)args)
        .Cast<object>()
        .Select(x => x == null ? "null" : x.ToString())
        .ToArray();
      Debug.Log(String.Join(", ", sargs));
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void Warn(params object[] args) {
      string[] sargs = ((IEnumerable)args)
        .Cast<object>()
        .Select(x => x == null ? "null" : x.ToString())
        .ToArray();
      Debug.LogWarning(String.Join(", ", sargs));
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void Error(params object[] args) {
      string[] sargs = ((IEnumerable)args)
        .Cast<object>()
        .Select(x => x == null ? "null" : x.ToString())
        .ToArray();
      Debug.LogError(String.Join(", ", sargs));
    }
  }
}
