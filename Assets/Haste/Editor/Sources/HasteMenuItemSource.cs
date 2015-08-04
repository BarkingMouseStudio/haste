using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;

namespace Haste {

  public class HasteMenuItemSource : IEnumerable<HasteItem> {

    static readonly Regex modifiers = new Regex(@"\s+[\%\#\&\_]+\w$", RegexOptions.IgnoreCase);

    public const string NAME = "Menu Item";

    static string[] MacPlatformMenuItems = new string[]{
      "Unity/About Unity...",
      "Unity/Preferences...",
      "Assets/Reveal in Finder"
    };

    static string[] WindowsPlatformMenuItems = new string[]{
      "Help/About Unity...",
      "File/Preferences...",
      "Assets/Show in Explorer"
    };

    static string[] CustomMenuItems = new string[]{
      "Assets/Instantiate Prefab",

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

    bool IsValid(string path) {
      var len = path.Length;
      if (len == 0) {
        return false;
      }
      if (path[len - 1] == '/') {
        return false;
      }
      return true;
    }

    public IEnumerator<HasteItem> GetEnumerator() {
      // Menu items found in the currently loaded assembly
      foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies()) {
        // Exclude built-in assemblies for performance reasons
        if (assembly.FullName.StartsWith("Mono")) continue;
        if (assembly.FullName.StartsWith("ICSharpCode")) continue;
        if (assembly.FullName.StartsWith("System")) continue;
        if (assembly.FullName.StartsWith("nunit")) continue;
        if (assembly.FullName.StartsWith("mscorlib")) continue;
        if (assembly.FullName.StartsWith("Unity.")) continue;
        if (assembly.FullName.StartsWith("UnityScript")) continue;
        if (assembly.FullName.StartsWith("UnityEngine")) continue;
        if (assembly.FullName.StartsWith("UnityEditor")) continue;

        // User assemblies in here:
        // if (assembly.FullName.StartsWith("Assembly-CSharp-Editor")) continue;

        foreach (var result in HasteReflection.GetAttributesInAssembly<MenuItem>(assembly)) {
          MenuItem menuItem = (MenuItem)result.First;

          if (menuItem.menuItem == "Window/Haste") continue;
          if (menuItem.menuItem.StartsWith("internal:")) continue;
          if (menuItem.validate) continue;

          string path = modifiers.Replace(menuItem.menuItem, ""); // Remove keyboard modifiers
          if (IsValid(path)) {
            yield return new HasteItem(path, menuItem.priority, NAME);
          }
        }
      }

      // Platform-specific menu items
      switch (Application.platform) {
        case RuntimePlatform.OSXEditor:
          foreach (string path in MacPlatformMenuItems) {
            if (IsValid(path)) {
              yield return new HasteItem(path, 0, NAME);
            }
          }
          break;
        case RuntimePlatform.WindowsEditor:
          foreach (string path in WindowsPlatformMenuItems) {
            if (IsValid(path)) {
              yield return new HasteItem(path, 0, NAME);
            }
          }
          break;
      }

      // Menu items for the running version of Unity
      if (HasteVersionUtils.IsUnity5) {
        foreach (string path in new MenuItemsUnity5()) {
          if (IsValid(path)) {
            yield return new HasteItem(path, 0, NAME);
          }
        }
      } else {
        foreach (string path in new MenuItemsUnity4()) {
          if (IsValid(path)) {
            yield return new HasteItem(path, 0, NAME);
          }
        }
      }

      // Custom menu items that don't really exist in Unity
      foreach (string path in CustomMenuItems) {
        if (IsValid(path)) {
          yield return new HasteItem(path, 0, NAME);
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}
