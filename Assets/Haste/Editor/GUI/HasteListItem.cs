using UnityEditor;
using UnityEngine;
using System;

namespace Haste {

  // Wraps results to handle some of the common drawing and interaction tasks.
  public static class HasteListItem {

    public static void Draw(IHasteResult result, int index, bool isHighlighted, Action<Event, int> mouseDown, Action<Event, int> click, Action<Event, int> doubleClick, Action<Event, int> mouseDrag) {
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
              mouseDrag(e, index);
              break;
            case EventType.MouseDown:
              if (e.clickCount == 2) {
                doubleClick(e, index);
              } else {
                mouseDown(e, index);
              }
              break;
            case EventType.MouseUp:
              click(e, index);
              break;
          }
        }

        #if DEBUG
        EditorGUILayout.LabelField(result.Score.ToString("f2"),
          isHighlighted ? HasteStyles.GetStyle("HighlightedScore") : HasteStyles.GetStyle("Score"),
          GUILayout.Width(64), GUILayout.ExpandHeight(true));
        #endif

        result.Draw(isHighlighted);

        if (result.IsSelected) {
          var rect = new Rect(horizontal.Rect.x + horizontal.Rect.width - 28, horizontal.Rect.y + 10, 20, 20);
          EditorGUI.LabelField(rect, HasteStyles.SelectionSymbol, HasteStyles.GetStyle("Dot"));
        }
      }
    }
  }
}
