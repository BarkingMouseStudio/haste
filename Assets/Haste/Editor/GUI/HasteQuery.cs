using UnityEngine;
using UnityEditor;

namespace Haste {

  public delegate void QueryChangedHandler(string query);

  public class HasteQuery : ScriptableObject {

    static readonly string NAME = "query";

    static readonly float BACKSPACE_DELAY = 0.1f;
    double backspaceTime = 0.0f;

    public event QueryChangedHandler Changed;

    private string query = "";
    public string Query {
      get { return query; }
      protected set { query = value; }
    }

    void OnBackspace(Event e) {
      e.Use();
      Query = "";
      OnGUIChanged();
    }

    void OnKeyDown(Event e) {
      switch (e.keyCode) {
        case KeyCode.Backspace:
          if (Query.Length > 0) {
            if (backspaceTime == 0) {
              backspaceTime = EditorApplication.timeSinceStartup;
            }

            var delta = EditorApplication.timeSinceStartup - backspaceTime;
            if (delta >= BACKSPACE_DELAY) {
              backspaceTime = 0;
              OnBackspace(e);
            }
          }
          break;
      }
    }

    void OnKeyUp(Event e) {
      switch (e.keyCode) {
        case KeyCode.Backspace:
          backspaceTime = 0;
          break;
      }
    }

    void OnEvent(Event e) {
      switch (e.type) {
        case EventType.KeyDown:
          OnKeyDown(e);
          break;
        case EventType.KeyUp:
          OnKeyUp(e);
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
        var queryStyle = HasteStyles.Skin.GetStyle("Query");
        Query = EditorGUILayout.TextField(Query, queryStyle,
          GUILayout.Height(queryStyle.fixedHeight));
        Query = Query.Trim();
      }

      if (GUI.changed) {
        OnGUIChanged();
      }
    }
  }
}
