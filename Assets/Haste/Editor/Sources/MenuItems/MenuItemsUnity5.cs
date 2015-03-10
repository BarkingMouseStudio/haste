using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public class MenuItemsUnity5 : IEnumerable<string> {

    static string[] FileMenuItems = new string[]{
      "File/New Scene",
      "File/Open Scene...",
      // ---
      "File/Save Scene",
      "File/Save Scene as...",
      // ---
      // TODO: "File/New Project...",
      // TODO: "File/Open Project...",
      "File/Save Project",
      // ---
      "File/Build Settings...",
      "File/Build & Run",
      "File/Build in Cloud..."
    };

    static string[] EditMenuItems = new string[]{
      "Edit/Undo",
      "Edit/Redo",
      // ---
      "Edit/Cut",
      "Edit/Copy",
      "Edit/Paste",
      // ---
      "Edit/Duplicate",
      "Edit/Delete",
      // ---
      "Edit/Frame Selected",
      "Edit/Lock View To Selected",
      "Edit/Select All",
      // ---
      "Edit/Play",
      "Edit/Pause",
      "Edit/Step",
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
      // ---
      "Edit/Snap Settings..."
    };

    static string[] AssetsMenuItems = new string[]{
      "Assets/Create/Folder",
      // ---
      "Assets/Create/Javascript",
      "Assets/Create/C# Script",
      "Assets/Create/Shader",
      "Assets/Create/Compute Shader",
      // ---
      "Assets/Create/Prefab",
      // ---
      "Assets/Create/Audio Mixer",
      // ---
      "Assets/Create/Material",
      "Assets/Create/Lens Flare",
      "Assets/Create/Render Texture",
      "Assets/Create/Lightmap Parameters",
      // ---
      "Assets/Create/Animator Controller",
      "Assets/Create/Animation",
      "Assets/Create/Animator Override Controller",
      "Assets/Create/Avatar Mask",
      // ---
      "Assets/Create/Physic Material",
      "Assets/Create/Physics2D Material",
      // ---
      "Assets/Create/GUI Skin",
      "Assets/Create/Custom Font",
      "Assets/Create/Shader Variant Collection",
      // ---
      "Assets/Create/Legacy/Cubemap",
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
      "Assets/Run API Updater...",
      // ---
      "Assets/Sync MonoDevelop Project"
    };

    static string[] GameObjectMenuItems = new string[]{
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
      // ---
      "GameObject/3D Object/Terrain",
      "GameObject/3D Object/Tree",
      "GameObject/3D Object/Wind Zone",
      // ---
      "GameObject/3D Object/3D Text",
      // ---
      "GameObject/2D Object/Sprite",
      // ---
      "GameObject/Light/Directional Light",
      "GameObject/Light/Point Light",
      "GameObject/Light/Spotlight",
      "GameObject/Light/Area Light",
      // ---
      "GameObject/Light/Reflection Probe",
      "GameObject/Light/Light Probe Group",
      // ---
      "GameObject/Audio/Audio Source",
      "GameObject/Audio/Audio Reverb Zone",
      // ---
      "GameObject/UI/Panel",
      "GameObject/UI/Button",
      "GameObject/UI/Text",
      "GameObject/UI/Image",
      "GameObject/UI/Raw Image",
      "GameObject/UI/Slider",
      "GameObject/UI/Scrollbar",
      "GameObject/UI/Toggle",
      "GameObject/UI/Input Field",
      "GameObject/UI/Canvas",
      "GameObject/UI/Event System",
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
    };

    static string[] ComponentMenuItems = new string[]{
      "Component/Add...",

      "Component/Mesh/Mesh Filter",
      "Component/Mesh/Text Mesh",
      // ---
      "Component/Mesh/Mesh Renderer",
      "Component/Mesh/Skinned Mesh Renderer",

      "Component/Effects/Particle System",
      "Component/Effects/Trail Renderer",
      "Component/Effects/Line Renderer",
      "Component/Effects/Lens Flare",
      "Component/Effects/Halo",
      "Component/Effects/Projector",

      "Component/Effects/Legacy Particles/Ellipsoid Particle Emitter",
      "Component/Effects/Legacy Particles/Mesh Particle Emitter",
      // ---
      "Component/Effects/Legacy Particles/Particle Animator",
      "Component/Effects/Legacy Particles/World Particle Collider",
      // ---
      "Component/Effects/Legacy Particles/Particle Renderer",

      "Component/Physics/Rigidbody",
      "Component/Physics/Character Controller",
      // ---
      "Component/Physics/Box Collider",
      "Component/Physics/Sphere Collider",
      "Component/Physics/Capsule Collider",
      "Component/Physics/Mesh Collider",
      "Component/Physics/Wheel Collider",
      "Component/Physics/Terrain Collider",
      // ---
      "Component/Physics/Cloth",
      // ---
      "Component/Physics/Hinge Joint",
      "Component/Physics/Fixed Joint",
      "Component/Physics/Spring Joint",
      "Component/Physics/Character Joint",
      "Component/Physics/Configurable Joint",
      // ---
      "Component/Physics/Constant Force",

      "Component/Physics 2D/Rigidbody 2D",
      // ---
      "Component/Physics 2D/Circle Collider 2D",
      "Component/Physics 2D/Box Collider 2D",
      "Component/Physics 2D/Edge Collider 2D",
      "Component/Physics 2D/Polygon Collider 2D",
      // ---
      "Component/Physics 2D/Spring Joint 2D",
      "Component/Physics 2D/Distance Joint 2D",
      "Component/Physics 2D/Hinge Joint 2D",
      "Component/Physics 2D/Slider Joint 2D",
      "Component/Physics 2D/Wheel Joint 2D",
      // ---
      "Component/Physics 2D/Constant Force 2D",
      // ---
      "Component/Physics 2D/Area Effector 2D",
      "Component/Physics 2D/Point Effector 2D",
      "Component/Physics 2D/Platform Effector 2D",
      "Component/Physics 2D/Surface Effector 2D",

      "Component/Navigation/Nav Mesh Agent",
      "Component/Navigation/Off Mesh Link",
      "Component/Navigation/Nav Mesh Obstacle",

      "Component/Audio/Audio Listener",
      "Component/Audio/Audio Source",
      "Component/Audio/Audio Reverb Zone",
      // ---
      "Component/Audio/Audio Low Pass Filter",
      "Component/Audio/Audio High Pass Filter",
      "Component/Audio/Audio Echo Filter",
      "Component/Audio/Audio Distortion Filter",
      "Component/Audio/Audio Reverb Filter",
      "Component/Audio/Audio Chorus Filter",

      "Component/Rendering/Camera",
      "Component/Rendering/Skybox",
      "Component/Rendering/Flare Layer",
      "Component/Rendering/GUI Layer",
      // ---
      "Component/Rendering/Light",
      "Component/Rendering/Light Probe Group",
      "Component/Rendering/Reflection Probe",
      // ---
      "Component/Rendering/Occlusion Area",
      "Component/Rendering/Occlusion Portal",
      "Component/Rendering/LOD Group",
      // ---
      "Component/Rendering/Sprite Renderer",
      "Component/Rendering/Canvas Renderer",
      // ---
      "Component/Rendering/GUI Texture",
      "Component/Rendering/GUI Text",

      "Component/Layout/Rect Transform",
      "Component/Layout/Canvas",
      "Component/Layout/Canvas Group",
      // ---
      "Component/Layout/Canvas Scaler",
      // ---
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
      "Component/Miscellaneous/Terrain",
      "Component/Miscellaneous/Billboard Renderer",

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
      // ---
      "Component/UI/Button",
      "Component/UI/Input Field",
      "Component/UI/Scrollbar",
      "Component/UI/Scroll Rect",
      "Component/UI/Slider",
      "Component/UI/Toggle",
      "Component/UI/Toggle Group",
      // ---
      "Component/UI/Selectable",

      "Component/Scripts/Editor Label",
      "Component/Scripts/Enable Camera Depth In Forward"
    };

    static string[] WindowMenuItems = new string[]{
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
      "Window/Audio Mixer",
      "Window/Asset Store",
      "Window/Version Control",
      "Window/Animator Parameter",
      "Window/Animator",
      "Window/Sprite Packer",
      // ---
      "Window/Lighting",
      "Window/Occlusion Culling",
      "Window/Frame Debugger",
      "Window/Navigation",
      // ---
      "Window/Console"
    };

    public IEnumerator<string> GetEnumerator() {
      foreach (string path in FileMenuItems) {
        yield return path;
      }
      foreach (string path in EditMenuItems) {
        yield return path;
      }
      foreach (string path in AssetsMenuItems) {
        yield return path;
      }
      foreach (string path in GameObjectMenuItems) {
        yield return path;
      }
      foreach (string path in ComponentMenuItems) {
        yield return path;
      }
      foreach (string path in WindowMenuItems) {
        yield return path;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}
