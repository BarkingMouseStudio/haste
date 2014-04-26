using UnityEngine;
using System;
using System.Linq;
using System.Collections;

namespace Haste {

  public static class HasteLogger {

    public static void Info(params object[] args) {
      string[] sargs = ((IEnumerable)args)
        .Cast<object>()
        .Select(x => x.ToString())
        .ToArray();
      Debug.Log(String.Join(", ", sargs));
    }

    public static void Warn(params object[] args) {
      string[] sargs = ((IEnumerable)args)
        .Cast<object>()
        .Select(x => x.ToString())
        .ToArray();
      Debug.LogWarning(String.Join(", ", sargs));
    }

    public static void Error(params object[] args) {
      string[] sargs = ((IEnumerable)args)
        .Cast<object>()
        .Select(x => x.ToString())
        .ToArray();
      Debug.LogError(String.Join(", ", sargs));
    }
}
}
