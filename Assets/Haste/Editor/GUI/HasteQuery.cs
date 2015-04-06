using UnityEngine;
using UnityEditor;

namespace Haste {

  public delegate void QueryChangedHandler(string query);

  public class HasteQuery : ScriptableObject {

    static readonly string NAME = "query";

    static readonly float BACKSPACE_DELAY = 0.2f;
    double backspaceTime = 0.0f;

    public event QueryChangedHandler Changed;

    private string query = "";
    public string Query {
      get { return query; }
      protected set { query = value; }
    }

    void OnKeyDown(Event e) {
      switch (e.keyCode) {
        case KeyCode.Backspace:
          if (backspaceTime == 0) {
            backspaceTime = EditorApplication.timeSinceStartup;
          }

          var delta = EditorApplication.timeSinceStartup - backspaceTime;
          if (delta >= BACKSPACE_DELAY) {
            // Consume backspace events while we're holding the key down.
            e.Use();
            query = ""; // ?
            OnGUIChanged(); // ?
          }
          break;
      }
    }

    void OnKeyUp(Event e) {
      switch (e.keyCode) {
        case KeyCode.Backspace:
          // var delta = EditorApplication.timeSinceStartup - backspaceTime;
          // if (delta >= BACKSPACE_DELAY) {
          //   // Consume backspace events when we release the key after a delay.
          //   e.Use();
          // }

          // Always clear backspace time.
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
        Changed(query);
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

    // public void UpdateHandler(EditorWindow window) {
    //   if (backspaceTime > 0) { // Quick check that backspace key is down
    //     var delta = EditorApplication.timeSinceStartup - backspaceTime;
    //
    //     // If we have a query and our backspace has been held long enough,
    //     // clear the query:
    //     if (delta >= BACKSPACE_DELAY && query.Length > 0) {
    //       query = "";
    //       OnGUIChanged();
    //     }
    //   }
    // }

    public void OnGUI() {
      OnEvent(Event.current);

      // Dummy element for blurring
      // GUI.SetNextControlName("");
      // GUI.Button(new Rect(0, 0, 0, 0), "", GUIStyle.none);

      using (new HasteFocusText(NAME)) {
        var queryStyle = HasteStyles.GetStyle("Query");
        query = EditorGUILayout.TextField(query, queryStyle,
          GUILayout.Height(queryStyle.fixedHeight));
        query = query.Trim();
      }

      if (GUI.changed) {
        OnGUIChanged();
      }
    }
  }
}
