using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Haste {

  public class HasteMenuItemSource : IEnumerable<HasteItem> {

    static readonly Regex modifiers = new Regex(@"\s+[\%\#\&\_]+\w$", RegexOptions.IgnoreCase);

    public static readonly string NAME = "Menu Item";

    public static string[] CustomMenuItems = new string[]{
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

    public static string[] MacBuiltinMenuItems = new string[]{
      "Unity/About Unity...",
      "Unity/Preferences...",
      "Assets/Reveal in Finder"
    };

    public static string[] WindowsBuiltinMenuItems = new string[]{
      "Help/About Unity...",
      "File/Preferences...",
      "Assets/Show in Explorer"
    };

    public static string[] BuiltinMenuItems = new string[]{
      "File/New Scene",
      "File/Open Scene...",
      "File/Save Scene",
      "File/Save Scene as...",
      // ---
      // TODO: "File/New Project...",
      // ---
      // TODO: "File/Open Project...",
      "File/Save Project",
      // ---
      "File/Build Settings...",
      "File/Build & Run",

      "Edit/Undo",
      "Edit/Redo",
      // ---
      "Edit/Cut",
      "Edit/Copy",
      "Edit/Paste",
      // ---
      "Edit/Duplicate",
      "Edit/Delete",
      "Edit/Frame Selected",
      "Edit/Lock View To Selected",
      "Edit/Select All",
      // ---
      "Edit/Snap Settings...",
      // ---
      // TODO: "Edit/Project Settings/Input",
      "Edit/Project Settings/Tags and Layers",
      // TODO: "Edit/Project Settings/Audio",
      // TODO: "Edit/Project Settings/Time",
      "Edit/Project Settings/Player",
      "Edit/Project Settings/Physics",
      "Edit/Project Settings/Physics 2D",
      "Edit/Project Settings/Quality",
      // TODO: "Edit/Project Settings/Graphics",
      // TODO: "Edit/Project Settings/Network",
      "Edit/Project Settings/Editor",
      "Edit/Project Settings/Script Execution Order",
      "Edit/Render Settings",

      "Assets/Create/Folder",
      "Assets/Create/Javascript",
      "Assets/Create/C# Script",
      "Assets/Create/Boo Script",
      "Assets/Create/Shader",
      "Assets/Create/Compute Shader",
      "Assets/Create/Prefab",
      "Assets/Create/Material",
      "Assets/Create/Cubemap",
      "Assets/Create/Lens Flare",
      "Assets/Create/Render Texture",
      "Assets/Create/Animator Controller",
      // ---
      "Assets/Open",
      "Assets/Delete",
      // ---
      "Assets/Import New Asset...",
      "Assets/Import Package/Custom Package...",
      "Assets/Export Package...",
      "Assets/Find References In Scene",
      "Assets/Select Dependencies",
      // ---
      "Assets/Refresh",
      "Assets/Reimport",
      // ---
      "Assets/Reimport All",
      // ---
      "Assets/Sync MonoDevelop Project",

      "GameObject/Create Empty",
      "GameObject/Create Empty Child",
      "GameObject/Create Other/GUI Text",
      "GameObject/Create Other/GUI Texture",
      "GameObject/Create Other/3D Text",
      // ---
      "GameObject/Create Other/Directional Light",
      "GameObject/Create Other/Point Light",
      "GameObject/Create Other/Spotlight",
      "GameObject/Create Other/Area Light",
      // ---
      "GameObject/Create Other/Cube",
      "GameObject/Create Other/Sphere",
      "GameObject/Create Other/Capsule",
      "GameObject/Create Other/Cylinder",
      "GameObject/Create Other/Plane",
      "GameObject/Create Other/Quad",
      // ---
      "GameObject/Create Other/Sprite",
      // ---
      "GameObject/Create Other/Cloth",
      // ---
      "GameObject/Create Other/Audio Reverb Zone",
      // ---
      "GameObject/Create Other/Terrain",
      "GameObject/Create Other/Ragdoll...",
      "GameObject/Create Other/Tree",
      "GameObject/Create Other/Wind Zone",
      "GameObject/Particle System",
      "GameObject/Camera",
      // ---
      "GameObject/Center On Children",
      // ---
      "GameObject/Make Parent",
      "GameObject/Clear Parent",
      "GameObject/Apply Changes To Prefab",
      "GameObject/Break Prefab Instance",
      // ---
      "GameObject/Set as first sibling",
      "GameObject/Set as last sibling",
      "GameObject/Move To View",
      "GameObject/Align With View",
      "GameObject/Align View to Selected",
      "GameObject/Toggle Active State",

      "Component/Add...",

      "Window/Layouts/Save Layout...",
      "Window/Layouts/Delete Layout...",
      "Window/Layouts/Revert Factory Settings...",
      // ---
      "Window/Scene",
      "Window/Game",
      "Window/Inspector",
      "Window/Hierarchy",
      "Window/Project",
      "Window/Animation",
      "Window/Profiler",
      "Window/Asset Store",
      "Window/Version Control",
      "Window/Animator",
      "Window/Sprite Editor",
      "Window/Sprite Packer",
      // ---
      "Window/Lightmapping",
      "Window/Occlusion Culling",
      "Window/Navigation",
      // ---
      "Window/Console",
    };

    public IEnumerator<HasteItem> GetEnumerator() {
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

        foreach (var attribute in HasteReflection.GetAttributesInAssembly(assembly, typeof(MenuItem))) {
          MenuItem menuItem = (MenuItem)attribute;

          if (menuItem.menuItem.Contains("Haste")) continue;
          if (menuItem.menuItem.StartsWith("internal:")) continue;
          if (menuItem.validate) continue;

          string path = modifiers.Replace(menuItem.menuItem, ""); // Remove keyboard modifiers

          yield return new HasteItem(path, menuItem.priority, NAME);
        }
      }

      foreach (string path in BuiltinMenuItems) {
        yield return new HasteItem(path, 0, NAME);
      }

      if (Application.platform == RuntimePlatform.OSXEditor) {
        foreach (string path in MacBuiltinMenuItems) {
          yield return new HasteItem(path, 0, NAME);
        }
      } else if (Application.platform == RuntimePlatform.WindowsEditor) {
        foreach (string path in WindowsBuiltinMenuItems) {
          yield return new HasteItem(path, 0, NAME);
        }
      }

      foreach (string path in CustomMenuItems) {
        yield return new HasteItem(path, 0, NAME);
      }

      // var layouts = HasteReflection.Layouts.Select((layout) => {
      //   return String.Format("Window/Layouts/{0}", layout);
      // });

      // foreach (string path in layouts) {
      //   yield return new HasteItem(path, 0, NAME);
      // }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}