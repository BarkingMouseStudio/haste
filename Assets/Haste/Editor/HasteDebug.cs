using UnityEngine;
using System;
using System.Linq;
using System.Collections;

namespace Haste {

  public class AssertionException: Exception {
    public AssertionException() {}
    public AssertionException(string message) : base(message) {}
  }

  public static class HasteDebug {

    static string[] Args(params object[] args) {
      return ((IEnumerable)args)
        .Cast<object>()
        .Select(x => x == null ? "null" : x.ToString())
        .ToArray();
    }

    public static void Assert(bool condition, string message = "") {
      if (!condition) {
        throw new AssertionException(message);
      }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void Info(params object[] args) {
      Debug.Log(String.Join(", ", Args(args)));
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void Info(string format, params object[] args) {
      Debug.Log(String.Format(format, Args(args)));
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void Info(string msg) {
      Debug.Log(msg);
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void Warn(params object[] args) {
      Debug.LogWarning(String.Join(", ", Args(args)));
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void Warn(string format, params object[] args) {
      Debug.LogWarning(String.Format(format, Args(args)));
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void Warn(string msg) {
      Debug.LogWarning(msg);
    }

    public static void Error(params object[] args) {
      Debug.LogError(String.Join(", ", Args(args)));
    }

    public static void Error(string format, params object[] args) {
      Debug.LogError(String.Format(format, Args(args)));
    }

    public static void Error(string msg) {
      Debug.LogError(msg);
    }
  }
}
