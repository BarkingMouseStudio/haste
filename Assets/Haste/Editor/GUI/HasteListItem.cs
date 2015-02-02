using UnityEditor;
using UnityEngine;

namespace Haste {

  // Wraps results to handle some of the common drawing and interaction tasks.
  public static class HasteListItem {

    // public enum ButtonEvent {
    //   None,
    //   SingleClick,
    //   DoubleClick
    // }

    // public static class HasteGUI {

    //   // Utility for double-click buttons:
    //   //
    //   // var button = HasteGUILayout.Button(EditorGUIUtility.GetControlRect());
    //   // if (button == ButtonEvent.DoubleClicked) {
    //   // } else if (button == ButtonEvent.SingleClicked) {
    //   // }
    //   public static ButtonEvent Button(Rect rect) {
    //     if (!Event.current.isMouse) {
    //       return ButtonEvent.None;
    //     }

    //     if (Event.current.type != EventType.MouseDown) {
    //       return ButtonEvent.None;
    //     }

    //     if (!rect.Contains(Event.current.mousePosition)) {
    //       return ButtonEvent.None;
    //     }

    //     if (Event.current.clickCount == 2) {
    //       return ButtonEvent.DoubleClick;
    //     } else {
    //       return ButtonEvent.SingleClick;
    //     }
    //   }
    // }

    public static void Draw(IHasteResult result, bool isHighlighted) {
      var resultStyle = isHighlighted ? HasteStyles.HighlightStyle : HasteStyles.NonHighlightStyle;

      using (var horizontal = new HasteHorizontal(resultStyle, GUILayout.Height(result.Height(isHighlighted)))) {
        // var button = HasteGUI.Button(horizontal.Rect);
        // // Highlight and selection are different.
        // // Selection doesn't take over until MouseUp
        // switch (button) {
        //   case ButtonEvent.MouseDown:
        //     // TODO: Persist previous selection index.
        //     SetHighlightedIndex(index, false);
        //     Repaint();
        //     break;
        //   case ButtonEvent.MouseDrag:
        //     // On drag begin dragging the highlighted object but select the previously selected object (before opening haste) so you can drag and drop.
        //     // TODO: begin drag, maintain highlight, restore previous selection
        //     break;
        //   case ButtonEvent.MouseUp:
        //     // On mouse up move the selection to the mouse down object unless a drag was started, then just restore it to wherever it was before mouse down.
        //     // SetSelection();
        //     break;
        //   case ButtonEvent.DoubleClick:
        //     Close();
        //     SelectResult(result);
        //     break;
        //   case ButtonEvent.SingleClick:
        //     SetHighlightedIndex(index, false);
        //     Repaint();
        //     break;
        // }

        result.Draw(isHighlighted);
      }
    }
  }
}
