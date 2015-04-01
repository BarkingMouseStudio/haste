using UnityEditor;
using UnityEngine;
using System;

namespace Haste {

  // Wraps results to handle some of the common drawing and interaction tasks.
  public static class HasteListItem {

    public static void Draw(IHasteResult result, int index, bool isHighlighted, Action<Event, int> MouseDown, Action<Event, int> Click, Action<Event, int> DoubleClick, Action<Event, int> MouseDrag) {
      GUIStyle resultStyle;
      if (isHighlighted) {
        resultStyle = HasteStyles.HighlightStyle;
      } else if (result.IsSelected) {
        resultStyle = HasteStyles.SelectionStyle;
      } else {
        resultStyle = HasteStyles.EmptyStyle;
      }

      using (var horizontal = new HasteHorizontal(resultStyle, GUILayout.Height(result.Height(isHighlighted)))) {
        var e = Event.current;
        var isMouseContained = horizontal.Rect.Contains(e.mousePosition);
        if (isMouseContained) {
          switch (e.type) {
            case EventType.MouseDrag:
              MouseDrag(e, index);
              break;
            case EventType.MouseDown:
              if (e.clickCount == 2) {
                DoubleClick(e, index);
              } else {
                MouseDown(e, index);
              }
              break;
            case EventType.MouseUp:
              Click(e, index);
              break;
          }
        }

        result.Draw(isHighlighted);
      }
    }
  }
}
