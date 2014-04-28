using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Haste {
  public static class HasteActions {

    public static void FocusByHierarchyPath(string path) {
      EditorApplication.ExecuteMenuItem("Window/Hierarchy");
      Selection.activeObject = GameObject.Find(path);
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

    public static HasteAction[] GetActionsForSource(HasteSource source) {
      switch (source) {
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

    public static HasteAction[] ProjectActions = new HasteAction[]{
      new HasteAction("Open", "Open the current file...", result => {
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

      new HasteAction("Copy", "Copy the current file...", result => {
        var copyPath = EditorUtility.SaveFilePanelInProject(String.Format("Copying {0}", result.Path),
          Path.GetFileNameWithoutExtension(result.Path),
          Path.GetExtension(result.Path).Substring(1),
          "Choose where to save the copy.");
        if (copyPath.Length != 0) {
          AssetDatabase.CopyAsset(result.Path, copyPath);
          AssetDatabase.Refresh();
        }
      }),

      new HasteAction("Rename", "Rename the current file...", result => {
        var renamePath = EditorUtility.SaveFilePanelInProject(String.Format("Renaming {0}", result.Path),
          Path.GetFileNameWithoutExtension(result.Path),
          Path.GetExtension(result.Path).Substring(1),
          "Choose where to save the copy.");
        if (renamePath.Length != 0) {
          AssetDatabase.RenameAsset(result.Path, renamePath);
          AssetDatabase.Refresh();
        }
      }),

      new HasteAction("Delete", "Delete the current file...", result => {
        bool cont = EditorUtility.DisplayDialog(
          String.Format("Delete {0}",
          Path.GetFileName(result.Path)),
          String.Format("Are you sure you want to delete \"{0}\"?", result.Path), "Delete", "Cancel");
        if (cont) {
          AssetDatabase.MoveAssetToTrash(result.Path);
          AssetDatabase.Refresh();
        }
      }),

      new HasteAction("Instantiate Prefab", "Instantiate the currently selected prefab...", result => {
        PrefabType prefabType = PrefabUtility.GetPrefabType(Selection.activeObject);
        if (prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab) {
          Selection.activeObject = PrefabUtility.InstantiatePrefab(Selection.activeObject);
          Undo.RegisterCreatedObjectUndo(Selection.activeObject, "Instantiated prefab");
        }
      }),
    };

    public static HasteAction[] HierarchyActions = new HasteAction[]{
      new HasteAction("Focus", "Focus the selected object", result => {
        FrameSelected();
      }),

      new HasteAction("Clone", "Clone the selected object", result => {
        Selection.activeObject = Clone(Selection.activeGameObject);
        Undo.RegisterCreatedObjectUndo(Selection.activeObject, "Cloned game object");
      }),

      new HasteAction("Select Prefab", "Select the prefab of the selected object", result => {
        Selection.activeObject = PrefabUtility.GetPrefabParent(Selection.activeGameObject);
      }),

      new HasteAction("Break Prefab", "Break the prefab connection of the selected object", result => {
        Undo.RecordObject(Selection.activeGameObject, "Break prefab");
        PrefabUtility.DisconnectPrefabInstance(Selection.activeGameObject);
      }),

      new HasteAction("Revert to Prefab", "Revert the selected object to its prefab", result => {
        Undo.RecordObject(Selection.activeGameObject, "Reset prefab state");
        PrefabUtility.ResetToPrefabState(Selection.activeGameObject);
      }),

      new HasteAction("Reconnect to Prefab", "Reconnect the selected object to its last prefab", result => {
        Undo.RecordObject(Selection.activeGameObject, "Reconnect prefab");
        PrefabUtility.ReconnectToLastPrefab(Selection.activeGameObject);
      }),

      new HasteAction("Select Parent", "Select the parent of the selected object", result => {
        GameObject go = Selection.activeGameObject;
        if (go.transform != null && go.transform.parent != null) {
          Selection.activeGameObject = go.transform.parent.gameObject;
        }
      }),

      new HasteAction("Select Children", "Select the children of the selected object", result => {
        GameObject go = Selection.activeGameObject;
        Transform parent = go.transform;
        if (parent != null && parent.childCount > 0) {
          IList<GameObject> children = new List<GameObject>(parent.childCount); 
          foreach (Transform transform in parent) {
            children.Add(transform.gameObject);
          }
          Selection.objects = children.ToArray();
        }
      }),

      new HasteAction("Delete", "Delete the selected object", result => {
        Undo.DestroyObjectImmediate(Selection.activeGameObject);
      }),
    };
  }
}