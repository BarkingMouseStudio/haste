using UnityEditor;
using UnityEngine;

namespace Haste {

  // Wraps results to handle some of the common drawing and interaction tasks.
  public static class HasteListItem {

    public static void Draw(IHasteResult result, bool isHighlighted) {
      var resultStyle = isHighlighted ? HasteStyles.HighlightStyle : HasteStyles.NonHighlightStyle;

      using (var horizontal = new HasteHorizontal(resultStyle, GUILayout.Height(result.Height(isHighlighted)))) {
        // Highlight and selection are different.
        // Selection doesn't take over until MouseUp
        // var mouseDown = Event.current.type == EventType.MouseDown;
        // var contains = rect.Contains(Event.current.mousePosition);
        // if (Event.current.isMouse && mouseDown && contains) {
        //     if (Event.current.clickCount == 2) {
        //       // return ButtonEvent.DoubleClick;
        //     } else {
        //       // return ButtonEvent.SingleClick;
        //     }
        // }

        result.Draw(isHighlighted);
      }
    }
  }
}
