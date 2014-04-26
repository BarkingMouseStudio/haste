using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Haste {
  public static class HasteActions {

    public static void DisplayPreferences() {
      var asm = Assembly.GetAssembly(typeof(EditorWindow));
      var T = asm.GetType("UnityEditor.PreferencesWindow");
      var method = T.GetMethod("ShowPreferencesWindow", BindingFlags.NonPublic|BindingFlags.Static);
      method.Invoke(null, null);
    }

    public static void FrameSelected() {
      SceneView.lastActiveSceneView.FrameSelected();
    }

    public static UnityEngine.Object Clone(GameObject go) {
      UnityEngine.Object prefabRoot = PrefabUtility.GetPrefabParent(go);
      if (prefabRoot != null) {
        return PrefabUtility.InstantiatePrefab(prefabRoot);
      } else {
        return UnityEngine.Object.Instantiate(go);
      }
    }

    public static HasteAction[] MenuActions = new HasteAction[]{

      new HasteAction("Unity/Preferences...", "", (result) => {
        DisplayPreferences();
      }),

      new HasteAction("Edit/Frame Selected", "", (result) => {
        FrameSelected();
      }),
    };

    public static HasteAction[] ProjectActions = new HasteAction[]{
      new HasteAction("Open", "Open the current file...", (result) => {
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(result.Path);

        switch (Path.GetExtension(result.Path)) {
          case ".unity": // Scene
            EditorApplication.SaveCurrentSceneIfUserWantsTo();
            EditorApplication.OpenScene(result.Path);
            break;
          default:
            AssetDatabase.OpenAsset(Selection.activeObject);
            break;
        }
      }),

      new HasteAction("Copy", "Copy the current file...", (result) => {
        var copyPath = EditorUtility.SaveFilePanelInProject(String.Format("Copying {0}...", result.Path),
          result.Path,
          Path.GetExtension(result.Path),
          "Choose where to save the copy.");
        AssetDatabase.CopyAsset(result.Path, copyPath);
        AssetDatabase.Refresh();
      }),
    };

    public static HasteAction[] HierarchyActions = new HasteAction[]{
      new HasteAction("Focus", "Focus the selected object in the scene...", (result) => {
        FrameSelected();
      }),

      new HasteAction("Clone", "Clone the selected object in the scene...", (result) => {
        Selection.activeObject = Clone(Selection.activeGameObject);
        EditorGUIUtility.PingObject(Selection.activeObject);
      }),
    };

    public static void FocusByHierarchyPath(string path) {
      EditorApplication.ExecuteMenuItem("Window/Hierarchy");
      Selection.activeObject = GameObject.Find(path);
      EditorGUIUtility.PingObject(Selection.activeObject);
    }

    public static void FocusByProjectPath(string path) {
      EditorApplication.ExecuteMenuItem("Window/Project");
      EditorUtility.FocusProjectWindow();

      // Traverse project downwards, pinging each level
      string[] pieces = path.Split(Path.DirectorySeparatorChar);
      string fullPath = "";
      for (int i = 0; i < pieces.Length; i++) {
        fullPath = Path.Combine(fullPath, pieces[i]);
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(fullPath);
        EditorGUIUtility.PingObject(Selection.activeObject);
      }
    }

    public static void FocusByResult(HasteResult result) {
      switch (result.Source) {
        case HasteSource.Project:
          FocusByProjectPath(result.Path);
          break;
        case HasteSource.Hierarchy:
          FocusByHierarchyPath(result.Path);
          break;
      }
    }

    public static void SelectByHierarchyPath(string path) {
      GameObject go = GameObject.Find(path);
      if (go.transform == null) {
        Selection.activeGameObject = go;
      } else {
        Selection.activeTransform = go.transform;
      }
    }

    public static void SelectByProjectPath(string path) {
      Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
    }

    public static HasteAction[] GetActionsForSource(HasteResult result) {
      switch (result.Source) {
        case HasteSource.Project:
          return ProjectActions;
        case HasteSource.Hierarchy:
          return HierarchyActions;
        default:
          return new HasteAction[0];
      }
    }

    public static void SelectByResult(HasteResult result) {
      switch (result.Source) {
        case HasteSource.Project:
          SelectByProjectPath(result.Path);
          break;
        case HasteSource.Hierarchy:
          SelectByHierarchyPath(result.Path);
          break;
        case HasteSource.Editor:
          EditorApplication.ExecuteMenuItem(result.Path);
          break;
      }
    }
  }
}