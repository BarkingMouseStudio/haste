using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public static class HasteMenuItems {

    public static string[] MenuItems = new string[]{
      "Unity/Preferences",

      "File/New Scene",
      "File/Open Scene...",
      "File/Save Scene",
      "File/Save Scene as...",

      "File/New Project...",

      "File/Open Project...",
      "File/Save Project",

      "File/Build Settings...",
      "File/Build & Run",

      "Edit/Undo",
      "Edit/Redo",

      "Edit/Cut",
      "Edit/Copy",
      "Edit/Paste",

      "Edit/Duplicate",
      "Edit/Delete",
      "Edit/FrameSelected",
      "Edit/Lock View To Selected",
      "Edit/Select All",

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

      "Assets/Import New Asset...",
      // "Assets/Import Package/...",
      "Assets/Export Package...",
      "Assets/Refresh",
      "Assets/Reimport All",
      "Assets/Sync MonoDevelop Project",

      "GameObject/Create Empty",
      "GameObject/Create Other/Particle System",
      "GameObject/Create Other/Camera",
      "GameObject/Create Other/GUI Text",
      "GameObject/Create Other/GUI Texture",
      "GameObject/Create Other/3D Text",
      // "GameObject/Create Other/...",

      // "Component/...",

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
      "Window/Lightmapping",
      "Window/Occlusion Culling",
      "Window/Navigation",
      "Window/Console",
    };
  }
}