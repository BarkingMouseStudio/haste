using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class EditorIndex : AbstractIndex {

    public EditorIndex() {
      index = new HashSet<Item>() {
        new Item("New Scene", "File/New Scene", Source.Editor),
        new Item("Open Scene", "File/Open Scene...", Source.Editor),
        new Item("Save Scene", "File/Save Scene", Source.Editor)
      };
    }
  }
}