using UnityEditor;
using UnityEngine;

namespace Haste {

  public enum ButtonEvent {
    None,
    SingleClick,
    DoubleClick
  }

  public static class HasteGUI {

    // Utility for double-click buttons:
    //
    // var button = HasteGUILayout.Button(EditorGUIUtility.GetControlRect());
    // if (button == ButtonEvent.DoubleClicked) {
    // } else if (button == ButtonEvent.SingleClicked) {
    // }
    public static ButtonEvent Button(Rect rect) {
      if (!Event.current.isMouse) {
        return ButtonEvent.None;
      }

      if (Event.current.type != EventType.MouseDown) {
        return ButtonEvent.None;
      }

      if (!rect.Contains(Event.current.mousePosition)) {
        return ButtonEvent.None;
      }

      Debug.Log(Event.current.clickCount);

      if (Event.current.clickCount == 2) {
        return ButtonEvent.DoubleClick;
      } else {
        return ButtonEvent.SingleClick;
      }
    }
  }
}
