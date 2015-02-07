using UnityEditor;
using UnityEngine;

namespace Haste {

  public enum HasteListItemEvent {
    None,
    MouseDown,
    Click,
    DoubleClick
  }

  // Wraps results to handle some of the common drawing and interaction tasks.
  public static class HasteListItem {

    public static HasteListItemEvent Draw(IHasteResult result, bool isHighlighted) {
      var resultStyle = isHighlighted ? HasteStyles.HighlightStyle : HasteStyles.NonHighlightStyle;
      var result = HasteListItemEvent.None;

      using (new HasteHorizontal(resultStyle, GUILayout.Height(result.Height(isHighlighted))))) {
        // Highlight and selection are different
        var e = Event.current;
        var isMouseContained = horizontal.Rect.Contains(e.mousePosition);
        if (isMouseContained) {
          switch (e.type) {
            case EventType.MouseDown:
              // Highlight
              if (e.clickCount == 2) {
                result = HasteListItemEvent.DoubleClick;
              } else {
                result = HasteListItemEvent.MouseDown;
              }
              break;
            case EventType.MouseUp:
              // Select
              result = HasteListItemEvent.Click;
              break;
          }
        }

        result.Draw(isHighlighted);
      }
    }

    return result;
  }
}
