using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteMenuItemResult : AbstractHasteResult {

    public HasteMenuItemResult(IHasteItem item, string query, int queryLen) : base(item, query, queryLen) {}

    public override void Draw(bool isHighlighted, bool highlightMatches) {
      #if IS_HASTE_PRO
        base.Draw(isHighlighted, highlightMatches);
      #else
        using (new HasteVertical()) {
          EditorGUILayout.LabelField(Item.Path, isHighlighted ? HasteStyles.HighlightedDisabledNameStyle : HasteStyles.DisabledNameStyle);
          EditorGUILayout.LabelField("Upgrade to Haste Pro to enable", isHighlighted ? HasteStyles.HighlightedDisabledDescriptionStyle : HasteStyles.DisabledDescriptionStyle);
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
