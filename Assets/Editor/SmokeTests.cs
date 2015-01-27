using UnityEngine;
using UnityEditor;

namespace Haste {

  public static class Tests {

    [MenuItem("Window/Haste/Verify Menu Items")]
    public static void VerifyMenuItems() {
      foreach (var menuItem in HasteMenuItemSource.CustomMenuItems) {
        EditorApplication.ExecuteMenuItem(menuItem);
      }

      if (Application.platform == RuntimePlatform.OSXEditor) {
        foreach (var menuItem in HasteMenuItemSource.MacBuiltinMenuItems) {
          EditorApplication.ExecuteMenuItem(menuItem);
        }
      } else if (Application.platform == RuntimePlatform.WindowsEditor) {
        foreach (var menuItem in HasteMenuItemSource.WindowsBuiltinMenuItems) {
          EditorApplication.ExecuteMenuItem(menuItem);
        }
      }

      foreach (var menuItem in HasteMenuItemSource.BuiltinMenuItems) {
        EditorApplication.ExecuteMenuItem(menuItem);
      }
    }
  }
}