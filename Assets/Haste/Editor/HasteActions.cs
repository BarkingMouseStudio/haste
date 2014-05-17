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
          Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(result.Path);
          break;
        case HasteSource.Hierarchy:
          Selection.activeInstanceID = result.InstanceId;
          EditorGUIUtility.PingObject(result.InstanceId);
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
      var prefabRoot = PrefabUtility.GetPrefabParent(go);
      if (prefabRoot != null) {
        return PrefabUtility.InstantiatePrefab(prefabRoot);
      } else {
        return UnityEngine.Object.Instantiate(go);
      }
    }

    public static HasteAction[] ProjectActions = new HasteAction[]{
      new HasteAction("Open", "Open the current file...", result => {
        var selectedObject = AssetDatabase.LoadMainAssetAtPath(result.Path);
        switch (Path.GetExtension(result.Path)) {
          case ".unity": // Scene
            EditorApplication.SaveCurrentSceneIfUserWantsTo();
            EditorApplication.OpenScene(result.Path);
            break;
          default:
            AssetDatabase.OpenAsset(selectedObject);
            break;
        }
        Selection.activeObject = selectedObject;
      }),

      new HasteAction("Copy", "Copy the current file...", result => {
        var copyPath = EditorUtility.SaveFilePanelInProject(String.Format("Copying {0}", result.Path),
          Path.GetFileNameWithoutExtension(result.Path),
          Path.GetExtension(result.Path).Substring(1),
          "Choose where to save the copy.");
        if (copyPath.Length != 0 && AssetDatabase.CopyAsset(result.Path, copyPath)) {
          AssetDatabase.Refresh();
          Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(copyPath);
        }
      }),

      new HasteAction("Rename", "Rename the current file...", result => {
        var renamePath = EditorUtility.SaveFilePanelInProject(String.Format("Renaming {0}", result.Path),
          Path.GetFileNameWithoutExtension(result.Path),
          Path.GetExtension(result.Path).Substring(1),
          "Choose where to save the copy.");
        if (renamePath.Length != 0 && AssetDatabase.RenameAsset(result.Path, renamePath) == "") {
          AssetDatabase.Refresh();
          Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(renamePath);
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
        var selectedObject = AssetDatabase.LoadMainAssetAtPath(result.Path);
        PrefabType prefabType = PrefabUtility.GetPrefabType(Selection.activeObject);
        if (prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab) {
          Selection.activeObject = PrefabUtility.InstantiatePrefab(Selection.activeObject);
          Undo.RegisterCreatedObjectUndo(Selection.activeObject, "Instantiated prefab");
        }
        Selection.activeObject = selectedObject;
      }),
    };

    public static HasteAction[] HierarchyActions = new HasteAction[]{
      new HasteAction("Focus", "Focus the selected object", result => {
        FrameSelected();
      }),

      new HasteAction("Create Empty GameObject", "Create an GameObject under the selected object", result => {
        GameObject selectedObject = GameObject.Find(result.Path);
        GameObject newEmptyGameObject = new GameObject("GameObject");
        newEmptyGameObject.transform.parent = selectedObject.transform;
        Undo.RegisterCreatedObjectUndo(newEmptyGameObject, "Create Empty GameObject");
        Selection.activeObject = newEmptyGameObject;
      }),

      new HasteAction("Reset Transform", "Reset transform of the selected object", result => {
        GameObject selectedObject = GameObject.Find(result.Path);

        Undo.RecordObject(selectedObject.transform, "Reset Transform");
        selectedObject.transform.localRotation = Quaternion.identity;
        selectedObject.transform.localPosition = Vector3.zero;
        selectedObject.transform.localScale = Vector3.one;

        Selection.activeObject = selectedObject;
      }),

      new HasteAction("Clone", "Clone the selected object", result => {
        GameObject selectedObject = GameObject.Find(result.Path);
        if (selectedObject != null) {
          var clonedObject = Clone(selectedObject);
          Undo.RegisterCreatedObjectUndo(clonedObject, "Clone GameObject");
          Selection.activeObject = clonedObject;
        }
      }),

      new HasteAction("Select Prefab", "Select the prefab of the selected object", result => {
        GameObject selectedObject = GameObject.Find(result.Path);
        if (selectedObject != null) {
          var parentObject = PrefabUtility.GetPrefabParent(selectedObject);
          if (parentObject != null) {
            FocusByProjectPath(AssetDatabase.GetAssetPath(parentObject));
          }
        }
      }),

      new HasteAction("Break Prefab", "Break the prefab connection of the selected object", result => {
        GameObject selectedObject = GameObject.Find(result.Path);
        if (selectedObject != null) {
          Undo.RegisterFullObjectHierarchyUndo(selectedObject);
          PrefabUtility.DisconnectPrefabInstance(selectedObject);
          Selection.activeGameObject = selectedObject;
        }
      }),

      new HasteAction("Revert to Prefab", "Revert the selected object to its prefab", result => {
        GameObject selectedObject = GameObject.Find(result.Path);
        if (selectedObject != null) {
          Undo.RegisterFullObjectHierarchyUndo(Selection.activeGameObject);
          PrefabUtility.RevertPrefabInstance(Selection.activeGameObject);
          Selection.activeGameObject = selectedObject;
        }
      }),

      new HasteAction("Reconnect to Prefab", "Reconnect the selected object to its last prefab", result => {
        GameObject selectedObject = GameObject.Find(result.Path);
        if (selectedObject != null) {
          Undo.RegisterFullObjectHierarchyUndo(selectedObject);
          PrefabUtility.ReconnectToLastPrefab(selectedObject);
          Selection.activeGameObject = selectedObject;
        }
      }),

      new HasteAction("Select Parent", "Select the parent of the selected object", result => {
        GameObject selectedObject = GameObject.Find(result.Path);
        if (selectedObject != null) {
          if (selectedObject.transform != null) {
            if (selectedObject.transform.parent != null) {
              Selection.activeGameObject = selectedObject.transform.parent.gameObject;
            }
          }
        }
      }),

      new HasteAction("Select Children", "Select the children of the selected object", result => {
        GameObject selectedObject = GameObject.Find(result.Path);
        if (selectedObject != null) {
          Transform parent = selectedObject.transform;
          if (parent != null && parent.childCount > 0) {
            IList<GameObject> children = new List<GameObject>(parent.childCount); 
            foreach (Transform transform in parent) {
              children.Add(transform.gameObject);
            }
            Selection.objects = children.ToArray();
          }
        }
      }),

      new HasteAction("Delete", "Delete the selected object", result => {
        GameObject selectedObject = GameObject.Find(result.Path);
        if (selectedObject != null) {
          Undo.DestroyObjectImmediate(selectedObject);
        }
      }),
    };
  }
}