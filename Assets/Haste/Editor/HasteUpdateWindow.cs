using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Haste {

  public class HasteUpdateWindow : EditorWindow {

    public static HasteUpdateWindow Instance { get; protected set; }

    HasteUpdateChecker updateChecker;
    HasteUpdateStatus status;
    float progress;

    [MenuItem("Window/Haste/Check for updates")]
    public static void Open() {
      if (HasteUpdateWindow.Instance == null) {
        HasteUpdateWindow.Init();
      } else {
        // Return the existing window if it exists
        EditorWindow.GetWindow<HasteUpdateWindow>();
      }
    }

    // Creates a new window instance and initializes it
    static void Init() {
      HasteUpdateWindow.Instance = EditorWindow.CreateInstance<HasteUpdateWindow>();
      HasteUpdateWindow.Instance.InitializeInstance();
      HasteUpdateWindow.Instance.ShowAuxWindow();
    }

    void InitializeInstance() {
      this.title = "Haste Update";

      this.position = new Rect(
        (Screen.currentResolution.width - HasteStyles.WindowWidth) / 2,
        (Screen.currentResolution.height - HasteStyles.WindowHeight) / 2,
        300, 200
      );

      // Disable the resize handle on the window
      this.minSize = this.maxSize =
        new Vector2(this.position.width, this.position.height);

      this.updateChecker = new HasteUpdateChecker();
      this.progress = 0.0f;

      Haste.Scheduler.Start(this.updateChecker.Check());
    }

    void Update() {
      bool repaint = false;
      if (status != updateChecker.Status) {
        status = updateChecker.Status;
        repaint = true;
      }

      if (status == HasteUpdateStatus.InProgress) {
        if (progress != updateChecker.Progress) {
          progress = updateChecker.Progress;
          repaint = true;
        }
      }

      if (repaint) {
        Repaint();
      }
    }

    void OnGUI() {
      switch (updateChecker.Status) {
        case HasteUpdateStatus.Available:
          if (GUILayout.Button("An update is available. Open the Asset Store to update.", HasteStyles.UpgradeStyle)) {
            #if IS_HASTE_PRO
            UnityEditorInternal.AssetStore.Open(Haste.ASSET_STORE_PRO_URL);
            #else
            UnityEditorInternal.AssetStore.Open(Haste.ASSET_STORE_FREE_URL);
            #endif
          }
          break;
        case HasteUpdateStatus.UpToDate:
          EditorGUILayout.LabelField("You're all up to date!");
          break;
        case HasteUpdateStatus.InProgress:
          EditorGUILayout.LabelField("Checking for updates...");
          break;
        case HasteUpdateStatus.Failed:
          EditorGUILayout.LabelField("Failed to check for updates.");
          break;
      }
    }
  }
}
