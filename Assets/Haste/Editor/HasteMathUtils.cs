using UnityEngine;

namespace Haste {

  public static class HasteMathUtils {

    // This is a shortcut instead of Mathf.Approximately which is slow.
    // We don't need perfect comparisons: its called "fuzzy matching".
    public static bool Approximately(float a, float b) {
      if (a > b) {
        return (a - b) < Mathf.Epsilon;
      }
      if (a < b) {
        return (b - a) < Mathf.Epsilon;
      }
      return true;
    }
  }
}
