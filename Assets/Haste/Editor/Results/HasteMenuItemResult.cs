using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteMenuItemResult : AbstractHasteResult {

    public HasteMenuItemResult(HasteItem item, float score, List<int> indices) : base(item, score, indices, HasteIntent.Action) {}

    public override void Action() {
      HasteActions.MenuItemFallbackDelegate menuItemFallback;
      if (HasteActions.MenuItemFallbacks.TryGetValue(Item.Path, out menuItemFallback)) {
        try {
          menuItemFallback();
        } catch (NotImplementedException ex) {
          Debug.LogException(ex);
        }
      } else {
        EditorApplication.ExecuteMenuItem(Item.Path);
      }
    }
  }
}
