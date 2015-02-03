using UnityEditor;
using UnityEngine;

namespace Haste {

    public enum ButtonEvent {
      None,
      SingleClick,
      DoubleClick,
      MouseDrag
    }

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

  // Wraps results to handle some of the common drawing and interaction tasks.
  public static class HasteListItem {

    public static void Draw(IHasteResult result, bool isHighlighted) {
      var resultStyle = isHighlighted ? HasteStyles.HighlightStyle : HasteStyles.NonHighlightStyle;

      using (var horizontal = new HasteHorizontal(resultStyle, GUILayout.Height(result.Height(isHighlighted)))) {
        // Highlight and selection are different.
        // Selection doesn't take over until MouseUp
        if (Event.current.type == EventType.MouseDown) {
          if (horizontal.Rect.Contains(Event.current.mousePosition)) {
            HasteDebug.Info("{0}, {1}", result.Item.Path, Event.current);
          }
        }

        result.Draw(isHighlighted);
      }
    }
  }
}
