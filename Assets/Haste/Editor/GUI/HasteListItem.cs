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

      var height = result.Height(isHighlighted);
      using (var horizontal = new HasteHorizontal(resultStyle, GUILayout.Height(height), GUILayout.ExpandWidth(true))) {
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

        if (result.IsSelected) {
          var rect = new Rect(horizontal.Rect.x + horizontal.Rect.width - 28, horizontal.Rect.y + 10, 20, 20);
          EditorGUI.LabelField(rect, HasteStyles.SelectionSymbol, HasteStyles.Skin.GetStyle("Dot"));
        }
      }
    }
  }
}
