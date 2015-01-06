using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteMenuItemResult : AbstractHasteResult {

    public HasteMenuItemResult(HasteItem item, float score, List<int> indices) : base(item, score, indices) {}

    public override void Draw() {
      #if IS_HASTE_PRO
        base.Draw();
      #else
        using (new HasteVertical()) {
          EditorGUILayout.LabelField(Path.GetFileName(Item.Path), HasteWindow.DisabledNameStyle);
          EditorGUILayout.LabelField("Upgrade to Haste Pro to enable", HasteWindow.DisabledDescriptionStyle);
        }
      #endif
    }

    public override void Action() {
      #if IS_HASTE_PRO
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
      #else
        UnityEditorInternal.AssetStore.Open(Haste.ASSET_STORE_PRO_URL);
      #endif
    }
  }
}
