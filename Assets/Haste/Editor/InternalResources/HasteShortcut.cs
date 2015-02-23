using UnityEngine;
using UnityEditor;

namespace Haste {

  public static class HasteShortcut {

    [MenuItem("Window/Haste %k", true)]
    public static bool IsHasteEnabled() {
      return HasteSettings.Enabled;
    }

    [MenuItem("Window/Haste %k")]
    public static void Open() {
      HasteWindow.Open();
    }
  }
}
