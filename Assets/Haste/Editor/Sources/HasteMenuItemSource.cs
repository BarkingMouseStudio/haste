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

  public class HasteMenuItemSource : IEnumerable<IHasteItem> {

    static readonly Regex modifiers = new Regex(@"\s+[\%\#\&\_]+\w$", RegexOptions.IgnoreCase);

    public static readonly string NAME = "Menu Item";

    static string[] Layouts {
      get {
        var WindowLayout = Type.GetType("UnityEditor.WindowLayout,UnityEditor");
        var layoutsPreferencesPath = (string)WindowLayout.GetProperty("layoutsPreferencesPath", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).GetValue(WindowLayout, null);
        return Directory.GetFiles(layoutsPreferencesPath).Select((path) => {
          return Path.GetFileNameWithoutExtension(path);
        }).Where((path) => {
          return !path.Contains("LastLayout");
        }).ToArray();
      }
    }

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

    public IEnumerator<IHasteItem> GetEnumerator() {
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

          if (menuItem.menuItem.StartsWith("Window/Haste")) continue;
          if (menuItem.menuItem.StartsWith("internal:")) continue;
          if (menuItem.validate) continue;

          string path = modifiers.Replace(menuItem.menuItem, ""); // Remove keyboard modifiers
          yield return new HasteMenuItem(path, menuItem.priority, NAME);
        }
      }

      // Platform-specific menu items
      switch (Application.platform) {
        case RuntimePlatform.OSXEditor:
          foreach (string path in MacPlatformMenuItems) {
            yield return new HasteMenuItem(path, 0, NAME);
          }
          break;
        case RuntimePlatform.WindowsEditor:
          foreach (string path in WindowsPlatformMenuItems) {
            yield return new HasteMenuItem(path, 0, NAME);
          }
          break;
      }

      // Menu items for the running version of Unity
      if (HasteVersionUtils.IsUnity5) {
        foreach (string path in new MenuItemsUnity5()) {
          yield return new HasteMenuItem(path, 0, NAME);
        }
      } else {
        foreach (string path in new MenuItemsUnity4()) {
          yield return new HasteMenuItem(path, 0, NAME);
        }
      }

      // Custom menu items that don't really exist in Unity
      foreach (string path in CustomMenuItems) {
        yield return new HasteMenuItem(path, 0, NAME);
      }

      // User-defined layout menu items
      foreach (string layout in Layouts) {
        yield return new HasteMenuItem(String.Format("Window/Layouts/{0}", layout), 0, NAME);
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}