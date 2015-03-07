using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Haste {

  public class HasteMenuItemSource : IEnumerable<HasteItem> {

    static readonly Regex modifiers = new Regex(@"\s+[\%\#\&\_]+\w$", RegexOptions.IgnoreCase);

    public static readonly string NAME = "Menu Item";

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

    // Menu items found in the currently loaded assembly
    public static IEnumerator<HasteItem> GetAssemblyMenuItems() {
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

          if (menuItem.menuItem.StartsWith("Window/Haste")) continue;
          if (menuItem.menuItem.StartsWith("internal:")) continue;
          if (menuItem.validate) continue;

          string path = modifiers.Replace(menuItem.menuItem, ""); // Remove keyboard modifiers
          yield return new HasteItem(path, menuItem.priority, NAME);
        }
      }
    }

    // Platform-specific menu items
    public IEnumerator<HasteItem> GetPlatformMenuItems() {
      switch (Application.platform) {
        case RuntimePlatform.OSXEditor:
          foreach (string path in MacPlatformMenuItems) {
            yield return new HasteItem(path, 0, NAME);
          }
          break;
        case RuntimePlatform.WindowsEditor:
          foreach (string path in WindowsPlatformMenuItems) {
            yield return new HasteItem(path, 0, NAME);
          }
          break;
      }
    }

    // User-defined layout menu items
    public IEnumerator<HasteItem> GetLayoutMenuItems() {
      var layouts = HasteUtils.Layouts.Select((layout) => {
        return String.Format("Window/Layouts/{0}", layout);
      });

      foreach (string path in layouts) {
        yield return new HasteItem(path, 0, NAME);
      }
    }

    // Custom menu items that don't really exist in Unity
    public IEnumerator<HasteItem> GetCustomMenuItems() {
      foreach (string path in CustomMenuItems) {
        yield return new HasteItem(path, 0, NAME);
      }
    }

    // Menu items for the running version of Unity
    public IEnumerator<HasteItem> GetVersionMenuItems() {
      if (HasteVersionUtils.IsUnity5) {
        foreach (string path in MenuItemsUnity5) {
          yield return new HasteItem(path, 0, NAME);
        }
      } else {
        foreach (string path in MenuItemsUnity4) {
          yield return new HasteItem(path, 0, NAME);
        }
      }
    }

    public IEnumerator<HasteItem> GetEnumerator() {
      yield return GetAssemblyMenuItems();
      yield return GetPlatformMenuItems();
      yield return GetVersionMenuItems();
      yield return GetCustomMenuItems();
      yield return GetLayoutMenuItems();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}