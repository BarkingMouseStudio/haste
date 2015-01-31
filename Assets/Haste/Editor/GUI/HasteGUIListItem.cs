using UnityEditor;
using UnityEngine;

namespace Haste {

  // Wraps results to handle some of the common drawing and interaction tasks.
  public class HasteGUIListItem<T> : ScriptableObject {

    // void OnGUI(IHasteResult result, int index) {
    //   var isHighlighted = index == highlightedIndex;
    //   var resultStyle = isHighlighted ? HasteStyles.HighlightStyle : HasteStyles.NonHighlightStyle;

    //   using (var horizontal = new HasteHorizontal(resultStyle, GUILayout.Height(itemHeight))) {
    //     var button = HasteGUI.Button(horizontal.Rect);
    //     // Highlight and selection are different.
    //     // Selection doesn't take over until MouseUp
    //     switch (button) {
    //       case ButtonEvent.MouseDown:
    //         // TODO: Persist previous selection index.
    //         SetHighlightedIndex(index, false);
    //         Repaint();
    //         break;
    //       case ButtonEvent.MouseDrag:
    //         // On drag begin dragging the highlighted object but select the previously selected object (before opening haste) so you can drag and drop.
    //         // TODO: begin drag, maintain highlight, restore previous selection
    //         break;
    //       case ButtonEvent.MouseUp:
    //         // On mouse up move the selection to the mouse down object unless a drag was started, then just restore it to wherever it was before mouse down.
    //         // SetSelection();
    //         break;
    //       case ButtonEvent.DoubleClick:
    //         Close();
    //         SelectResult(result);
    //         break;
    //       case ButtonEvent.SingleClick:
    //         SetHighlightedIndex(index, false);
    //         Repaint();
    //         break;
    //     }

    //     result.Draw(isHighlighted);
    //   }
    // }
  }
}
