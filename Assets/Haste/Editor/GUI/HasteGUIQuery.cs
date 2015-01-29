using UnityEngine;
using UnityEditor;

namespace Haste {

  public delegate void QueryChangedHandler(string query);

  public class HasteGUIQuery : ScriptableObject {

    public event QueryChangedHandler Changed;

    private string query = "";
    public string Query {
      get {
        return query;
      }
      protected set {
        query = value;
      }
    }

    void OnBackspace() {
      Query = "";
    }

    void OnKeyDown(Event e) {
      switch (e.keyCode) {
        case KeyCode.Backspace:
          e.Use();
          OnBackspace();
          break;
      }
    }

    void OnEvent(Event e) {
      switch (e.type) {
        case EventType.KeyDown:
          OnKeyDown(e);
          break;
      }
    }

    void OnGUIChanged() {
      if (Changed != null) {
        Changed(Query);
      }
    }

    void OnEnable() {
      base.hideFlags = HideFlags.HideAndDontSave;
    }

    public void OnGUI() {
      OnEvent(Event.current);

      using (new HasteFocusText("query")) {
        Query = EditorGUILayout.TextField(Query, HasteStyles.QueryStyle,
          GUILayout.Height(HasteStyles.QueryStyle.fixedHeight));
        Query = Query.Trim();
      }

      if (GUI.changed) {
        OnGUIChanged();
      }
    }
  }
}
