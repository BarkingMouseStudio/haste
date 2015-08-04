using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteMenuItemResult : AbstractHasteResult {

    public HasteMenuItemResult(HasteItem item, float score, string queryLower) : base(item, score, queryLower) {}

    public override void Draw(bool isHighlighted) {
      #if IS_HASTE_PRO
        base.Draw(isHighlighted);
      #else
        using (new HasteVertical()) {
          var nameStyle = isHighlighted ? HasteStyles.GetStyle("HighlightedDisabledName") :
            HasteStyles.GetStyle("DisabledName");
          EditorGUILayout.LabelField(Item.path, nameStyle);

          var descriptionStyle = isHighlighted ? HasteStyles.GetStyle("HighlightedDisabledDescription") :
            HasteStyles.GetStyle("DisabledDescription");
          EditorGUILayout.LabelField("Upgrade to Haste Pro to enable", descriptionStyle);
        }
      #endif
    }

    public override void Action() {
      #if IS_HASTE_PRO
        HasteActions.MenuItemFallbackDelegate menuItemFallback;
        if (HasteActions.MenuItemFallbacks.TryGetValue(Item.path, out menuItemFallback)) {
          try {
            menuItemFallback();
          } catch (NotImplementedException ex) {
            Debug.LogException(ex);
          }
        } else {
          EditorApplication.ExecuteMenuItem(Item.path);
        }
      #else
        UnityEditorInternal.AssetStore.Open(Haste.ASSET_STORE_PRO_URL);
      #endif
    }
  }
}
