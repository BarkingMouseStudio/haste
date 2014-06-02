using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Haste {

  #if IS_HASTE_PRO
  public class HasteMenuItemSource : IEnumerable<HasteItem> {

    public static readonly string NAME = "MenuItem";

    public static string[] BuiltinMenuItems = new string[]{
      "Unity/About Unity...",
      "Unity/Preferences...",

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
      "Edit/Project Settings/Input",
      "Edit/Project Settings/Tags and Layers",
      "Edit/Project Settings/Audio",
      "Edit/Project Settings/Time",
      "Edit/Project Settings/Player",
      "Edit/Project Settings/Physics",
      "Edit/Project Settings/Physics 2D",
      "Edit/Project Settings/Quality",
      "Edit/Project Settings/Graphics",
      "Edit/Project Settings/Network",
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
      "Assets/Create/Animation Controller",
      // ---
      "Assets/Reveal in Finder",
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
      "GameObject/Create Other/Particle System",
      "GameObject/Create Other/Camera",
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

      "Component/Add...",

      "Window/Minimize",
      "Window/Zoom",
      // ---
      "Window/Bring All to Front",
      "Window/Layouts/2 by 3",
      "Window/Layouts/4 Split",
      "Window/Layouts/Default",
      "Window/Layouts/Tall",
      "Window/Layouts/Wide",
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
      "Window/Sprite Packer (Developer Preview)",
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
        if (assembly.FullName.StartsWith("UnityScript")) continue;
        if (assembly.FullName.StartsWith("ICSharpCode")) continue;
        if (assembly.FullName.StartsWith("nunit")) continue;
        if (assembly.FullName.StartsWith("Assembly-CSharp-Editor")) continue;
        if (assembly.FullName.StartsWith("System")) continue;
        if (assembly.FullName.StartsWith("Unity.")) continue;
        if (assembly.FullName.StartsWith("UnityEngine")) continue;
        if (assembly.FullName.StartsWith("UnityEditor")) continue;
        if (assembly.FullName.StartsWith("mscorlib")) continue;

        foreach (var attribute in HasteUtils.GetAttributesInAssembly(assembly, typeof(MenuItem))) {
          MenuItem menuItem = (MenuItem)attribute;

          if (menuItem.menuItem.StartsWith("CONTEXT")) continue;
          if (menuItem.menuItem.StartsWith("internal:")) continue;
          if (menuItem.validate) continue;

          string path = menuItem.menuItem;
          int keyIndex = path.LastIndexOf('%');
          if (keyIndex != -1) {
            path = path.Remove(keyIndex);
          }

          yield return new HasteItem(path, menuItem.priority, NAME);
        }
      }

      foreach (string path in BuiltinMenuItems) {
        yield return new HasteItem(path, 0, NAME);
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
  #endif
}