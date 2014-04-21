using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class EditorIndex : AbstractIndex {

    public EditorIndex() {
      index = new HashSet<Item>() {
        // Unity
        new Item("Preferences...", "Unity/Preferences...", Source.Editor),

        // File
        new Item("New Scene", "File/New Scene", Source.Editor),
        new Item("Open Scene", "File/Open Scene...", Source.Editor),

        new Item("Save Scene", "File/Save Scene", Source.Editor),
        new Item("Save Scene as...", "File/Save Scene as...", Source.Editor),

        new Item("New Project", "File/New Project", Source.Editor),
        new Item("Open Project...", "File/Open Project...", Source.Editor),
        new Item("Save Project", "File/Save Project", Source.Editor),

        new Item("Build Settings...", "File/Build Settings...", Source.Editor),
        new Item("Build & Run", "File/Build & Run", Source.Editor),

        // Window
        new Item("Scene", "Window/Scene", Source.Editor),
        new Item("Game", "Window/Game", Source.Editor),
        new Item("Inspector", "Window/Inspector", Source.Editor),
        new Item("Hierarchy", "Window/Hierarchy", Source.Editor),
        new Item("Project", "Window/Project", Source.Editor),
        new Item("Animation", "Window/Animation", Source.Editor),
        new Item("Profiler", "Window/Profiler", Source.Editor),
        new Item("Asset Store", "Window/Asset Store", Source.Editor),
        new Item("Version Control", "Window/Version Control", Source.Editor),
        new Item("Animator", "Window/Animator", Source.Editor),
        new Item("Sprite Editor", "Window/Sprite Editor", Source.Editor),
        new Item("Sprite Packer (Developer Preview)", "Window/Sprite Packer (Developer Preview)", Source.Editor),
        // ---
        new Item("Lightmapping", "Window/Lightmapping", Source.Editor),
        new Item("Occlusion Culling", "Window/Occlusion Culling", Source.Editor),
        new Item("Navigation", "Window/Navigation", Source.Editor),
        // ---
        new Item("Console", "Window/Console", Source.Editor),

        // Edit
        new Item("Frame Selected", "Edit/Frame Selected", Source.Editor),
      };
    }
  }
}