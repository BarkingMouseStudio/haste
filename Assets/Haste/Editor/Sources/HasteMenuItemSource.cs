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
      // ---
      "GameObject/3D Object/Cube",
      "GameObject/3D Object/Sphere",
      "GameObject/3D Object/Capsule",
      "GameObject/3D Object/Cylinder",
      "GameObject/3D Object/Plane",
      "GameObject/3D Object/Quad",
      // ---
      "GameObject/3D Object/Ragdoll...",
      "GameObject/3D Object/Cloth",
      // ---
      "GameObject/3D Object/Terrain",
      "GameObject/3D Object/Tree",
      "GameObject/3D Object/Wind Zone",
      // ---
      "GameObject/2D Object/Sprite",
      // ---
      "GameObject/Light/Directional Light",
      "GameObject/Light/Point Light",
      "GameObject/Light/Spotlight",
      "GameObject/Light/Area Light",
      // ---
      "GameObject/Audio/Audio Source",
      "GameObject/Audio/Audio Reverb Zone",
      // ---
      "GameObject/UI/Panel",
      "GameObject/UI/Button",
      "GameObject/UI/Text",
      "GameObject/UI/Image",
      "GameObject/UI/RawImage",
      "GameObject/UI/Slider",
      "GameObject/UI/Scrollbar",
      "GameObject/UI/Toggle",
      "GameObject/UI/InputField",
      "GameObject/UI/Canvas",
      "GameObject/UI/EventSystem",
      // ---
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

      "Component/Mesh/Mesh Filter",
      "Component/Mesh/Text Mesh",
      "Component/Mesh/Mesh Renderer",

      "Component/Effects/Particle System",
      "Component/Effects/Trail Renderer",
      "Component/Effects/Line Renderer",
      "Component/Effects/Lens Flare",
      "Component/Effects/Halo",
      "Component/Effects/Projector",
      "Component/Effects/Legacy Particles/Ellipsoid Particle Emitter",
      "Component/Effects/Legacy Particles/Mesh Particle Emitter",
      "Component/Effects/Legacy Particles/Particle Animator",
      "Component/Effects/Legacy Particles/World Particle Collider",
      "Component/Effects/Legacy Particles/Particle Renderer",

      "Component/Physics/Rigidbody",
      "Component/Physics/Character Controller",
      "Component/Physics/Box Collider",
      "Component/Physics/Sphere Collider",
      "Component/Physics/Capsule Collider",
      "Component/Physics/Mesh Collider",
      "Component/Physics/Wheel Collider",
      "Component/Physics/Terrain Collider",
      "Component/Physics/Interactive Cloth",
      "Component/Physics/Skinned Cloth",
      "Component/Physics/Cloth Renderer",
      "Component/Physics/Hinge Joint",
      "Component/Physics/Fixed Joint",
      "Component/Physics/Spring Joint",
      "Component/Physics/Character Joint",
      "Component/Physics/Configurable Joint",
      "Component/Physics/Constant Force",

      "Component/Physics 2D/Rigidbody 2D",
      "Component/Physics 2D/Circle Collider 2D",
      "Component/Physics 2D/Box Collider 2D",
      "Component/Physics 2D/Edge Collider 2D",
      "Component/Physics 2D/Polygon Collider 2D",
      "Component/Physics 2D/Spring Joint 2D",
      "Component/Physics 2D/Distance Joint 2D",
      "Component/Physics 2D/Hinge Joint 2D",
      "Component/Physics 2D/Slider Joint 2D",
      "Component/Physics 2D/Wheel Joint 2D",

      "Component/Navigation/Nav Mesh Agent",
      "Component/Navigation/Off Mesh Link",
      "Component/Navigation/Nav Mesh Obstacle",

      "Component/Audio/Audio Listener",
      "Component/Audio/Audio Source",
      "Component/Audio/Audio Reverb Zone",
      "Component/Audio/Audio Low Pass Filter",
      "Component/Audio/Audio High Pass Filter",
      "Component/Audio/Audio Echo Filter",
      "Component/Audio/Audio Distortion Filter",
      "Component/Audio/Audio Reverb Filter",
      "Component/Audio/Audio Chorus Filter",

      "Component/Rendering/Camera",
      "Component/Rendering/Skybox",
      "Component/Rendering/Flare Layer",
      "Component/Rendering/GUILayer",
      "Component/Rendering/Light",
      "Component/Rendering/Light Probe Group",
      "Component/Rendering/Occlusion Area",
      "Component/Rendering/Occlusion Portal",
      "Component/Rendering/LODGroup",
      "Component/Rendering/Sprite Renderer",
      "Component/Rendering/Canvas Renderer",
      "Component/Rendering/GUITexture",
      "Component/Rendering/GUIText",

      "Component/Layout/Rect Transform",
      "Component/Layout/Canvas",
      "Component/Layout/Canvas Group",
      "Component/Layout/Canvas Scaler",
      "Component/Layout/Layout Element",
      "Component/Layout/Content Size Fitter",
      "Component/Layout/Aspect Ratio Fitter",
      "Component/Layout/Horizontal Layout Group",
      "Component/Layout/Vertical Layout Group",
      "Component/Layout/Grid Layout Group",

      "Component/Miscellaneous/Animator",
      "Component/Miscellaneous/Animation",
      "Component/Miscellaneous/Network View",
      "Component/Miscellaneous/Wind Zone",

      "Component/Event/Event System",
      "Component/Event/Event Trigger",
      "Component/Event/Physics 2D Raycaster",
      "Component/Event/Physics Raycaster",
      "Component/Event/Standalone Input Module",
      "Component/Event/Touch Input Module",
      "Component/Event/Graphic Raycaster",

      "Component/UI/Effects/Shadow",
      "Component/UI/Effects/Outline",
      "Component/UI/Effects/Position As UV1",
      "Component/UI/Image",
      "Component/UI/Text",
      "Component/UI/Raw Image",
      "Component/UI/Mask",
      "Component/UI/Button",
      "Component/UI/Input Field",
      "Component/UI/Scrollbar",
      "Component/UI/Scroll Rect",
      "Component/UI/Slider",
      "Component/UI/Toggle",
      "Component/UI/Toggle Group",
      "Component/UI/Selectable",

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