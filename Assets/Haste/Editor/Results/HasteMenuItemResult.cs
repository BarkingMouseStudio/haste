using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteMenuItemResult : AbstractHasteResult {

    public HasteMenuItemResult(HasteItem item, float score, List<int> indices) : base(item, score, indices) {}

    public override void Action() {
      EditorApplication.ExecuteMenuItem(Item.Path);
    }
  }
}
