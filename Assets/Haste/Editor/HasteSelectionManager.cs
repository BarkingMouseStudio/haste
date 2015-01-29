using UnityEngine;
using UnityEditor;

namespace Haste {

  public static class HasteSelectionManager  {

    private static int activeInstanceID;

    public static void Save() {
      activeInstanceID = Selection.activeInstanceID;
    }

    public static void Restore() {
      Selection.activeInstanceID = activeInstanceID;
    }
  }
}
