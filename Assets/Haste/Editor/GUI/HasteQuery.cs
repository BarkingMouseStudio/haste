using UnityEngine;
using UnityEditor;

namespace Haste {

  public delegate void QueryChangedHandler(string query);

  public class HasteQuery : ScriptableObject {

    static readonly string NAME = "query";

    public event QueryChangedHandler Changed;

    private string query = "";
    public string Query {
      get { return query; }
      protected set { query = value; }
    }

    void OnBackspace() {
      Query = "";
      OnGUIChanged();
    }

    void OnKeyDown(Event e) {
      switch (e.keyCode) {
        case KeyCode.Backspace:
          if (HasteSettings.BackspaceClears) {
            e.Use();
            OnBackspace();
          }
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

    // public void Focus() {
    //   EditorGUI.FocusTextInControl(NAME);
    // }

    // public void Blur() {
    //   EditorGUI.FocusTextInControl("");
    // }

    public void OnGUI() {
      OnEvent(Event.current);

      // Dummy element for blurring
      // GUI.SetNextControlName("");
      // GUI.Button(new Rect(0, 0, 0, 0), "", GUIStyle.none);

      using (new HasteFocusText(NAME)) {
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
