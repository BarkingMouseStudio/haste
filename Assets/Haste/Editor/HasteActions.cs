using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Haste {

  #if IS_HASTE_PRO
  public static class HasteActions {

    public delegate void MenuItemFallbackDelegate();

    public static IDictionary<string, MenuItemFallbackDelegate> MenuItemFallbacks = new Dictionary<string, MenuItemFallbackDelegate>() {

      #region CUSTOM
      { "Assets/Instantiate Prefab", () => {
        if (Selection.objects.Length == 0) {
          return;
        }

        using (new HasteUndoStack("Instantiate Prefab")) {
          var objects = new List<UnityEngine.Object>(Selection.objects.Length);
          foreach (var selectedObject in Selection.objects) {
            PrefabType prefabType = PrefabUtility.GetPrefabType(selectedObject);
            if (prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab) {
              var instantiatedPrefab = PrefabUtility.InstantiatePrefab(selectedObject);
              objects.Add(instantiatedPrefab);
              Undo.RegisterCreatedObjectUndo(selectedObject, "Instantiate Prefab");
            }
          }
          Selection.objects = objects.ToArray();
        }
      } },

      { "GameObject/Lock", () => {
        if (Selection.gameObjects.Length == 0) {
          return;
        }

        Undo.RecordObjects(Selection.gameObjects, "Lock GameObject");
        foreach (var selectedObject in Selection.gameObjects) {
          selectedObject.hideFlags = HideFlags.NotEditable;
        }
      } },

      { "GameObject/Unlock", () => {
        if (Selection.gameObjects.Length == 0) {
          return;
        }

        Undo.RecordObjects(Selection.gameObjects, "Unlock GameObject");
        foreach (var selectedObject in Selection.gameObjects) {
          selectedObject.hideFlags = HideFlags.None;
        }
      } },

      { "GameObject/Activate", () => {
        if (Selection.gameObjects.Length == 0) {
          return;
        }

        Undo.RecordObjects(Selection.gameObjects, "Activate GameObject");
        foreach (var selectedObject in Selection.gameObjects) {
          EditorUtility.SetObjectEnabled(selectedObject, true);
        }
      } },

      { "GameObject/Deactivate", () => {
        if (Selection.gameObjects.Length == 0) {
          return;
        }

        Undo.RecordObjects(Selection.gameObjects, "Deactivate GameObject");
        foreach (var selectedObject in Selection.gameObjects) {
          EditorUtility.SetObjectEnabled(selectedObject, false);
        }
      } },

      { "GameObject/Reset Transform", () => {
        if (Selection.transforms.Length == 0) {
          return;
        }

        Undo.RecordObjects(Selection.transforms, "Reset Transform");
        foreach (var selectedTransform in Selection.transforms) {
          selectedTransform.localPosition = Vector3.zero;
          selectedTransform.localScale = Vector3.one;
          selectedTransform.localRotation = Quaternion.identity;
        }
      } },

      { "GameObject/Select Parent", () => {
        if (Selection.transforms.Length == 0) {
          return;
        }

        var transforms = new List<Transform>(Selection.transforms.Length);
        foreach (var selectedTransform in Selection.transforms) {
          if (selectedTransform.parent != null) {
            transforms.Add(selectedTransform.parent);
          }
        }

        Selection.objects = transforms.ToArray();
      } },

      { "GameObject/Select Children", () => {
        if (Selection.transforms.Length == 0) {
          return;
        }

        IList<GameObject> children = new List<GameObject>();
        foreach (var selectedTransform in Selection.transforms) {
          if (selectedTransform != null && selectedTransform.childCount > 0) {
            foreach (Transform transform in selectedTransform) {
              children.Add(transform.gameObject);
            }
          }
        }

        Selection.objects = children.ToArray();
      } },

      // Prefab
      { "GameObject/Select Prefab", () => {
        if (Selection.gameObjects.Length == 0) {
          return;
        }

        var objects = new List<UnityEngine.Object>(Selection.objects.Length);
        foreach (var selectedObject in Selection.gameObjects) {
          var parentObject = PrefabUtility.GetPrefabParent(selectedObject);
          if (parentObject != null) {
            objects.Add(parentObject);
          }
        }

        Selection.objects = objects.ToArray();
      } },

      { "GameObject/Revert to Prefab", () => {
        if (Selection.gameObjects.Length == 0) {
          return;
        }

        using (new HasteUndoStack("Revert to Prefab")) {
          foreach (var selectedObject in Selection.gameObjects) {
            Undo.RegisterFullObjectHierarchyUndo(selectedObject);
            PrefabUtility.RevertPrefabInstance(selectedObject);
          }
        }
      } },

      { "GameObject/Reconnect to Prefab", () => {
        if (Selection.gameObjects.Length == 0) {
          return;
        }

        using (new HasteUndoStack("Reconnect to Prefab")) {
          foreach (var selectedObject in Selection.gameObjects) {
            Undo.RegisterFullObjectHierarchyUndo(selectedObject);
            PrefabUtility.ReconnectToLastPrefab(selectedObject);
          }
        }
      } },
      #endregion

      { "Unity/About Unity...", () => {
        var AboutWindow = typeof(EditorWindow).Assembly.GetType("UnityEditor.AboutWindow");
        EditorWindow.GetWindow(AboutWindow, true, "About Unity");
      } },

      { "Unity/Preferences...", () => {
        HasteUtils.UnityEditorInvoke("UnityEditor.PreferencesWindow", "ShowPreferencesWindow");
      } },

      { "File/New Scene", () => {
        EditorApplication.SaveCurrentSceneIfUserWantsTo();
        EditorApplication.NewScene();
      } },

      { "File/Open Scene...", () => {
        var scenePath = EditorUtility.OpenFilePanel("Load Scene", Application.dataPath, "unity");
        if (scenePath.Length != 0) {
          EditorApplication.SaveCurrentSceneIfUserWantsTo();
          EditorApplication.OpenScene(scenePath);
        }
      } },

      { "File/Save Scene", () => {
        EditorApplication.SaveScene();
      } },

      { "File/Save Scene as...", () => {
        EditorApplication.SaveScene(String.Empty, true);
      } },

      // { "File/New Project...", () => {
      //   // TODO: Project wizard
      //   throw new NotImplementedException();
      // } },

      // { "File/Open Project...", () => {
      //   // TODO: Project wizard
      //   throw new NotImplementedException();
      // } },

      { "File/Save Project", () => {
        AssetDatabase.SaveAssets();
      } },

      { "File/Build Settings...", () => {
        HasteUtils.UnityEditorInvoke("UnityEditor.BuildPlayerWindow", "ShowBuildPlayerWindow");
      } },

      { "File/Build & Run", () => {
        HasteUtils.UnityEditorInvoke("UnityEditor.BuildPlayerWindow", "BuildPlayerAndRun");
      } },

      { "Edit/Undo", () => {
        Undo.PerformUndo();
      } },

      { "Edit/Redo", () => {
        Undo.PerformRedo();
      } },

      { "Edit/Cut", () => {
        EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Cut"));
      } },

      { "Edit/Copy", () => {
        EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Copy"));
      } },

      { "Edit/Paste", () => {
        EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Paste"));
      } },

      { "Edit/Duplicate", () => {
        EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Duplicate"));
      } },

      { "Edit/Delete", () => {
        EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Delete"));
      } },

      { "Edit/Frame Selected", () => {
        SceneView.lastActiveSceneView.FrameSelected();
      } },

      { "Edit/Lock View To Selected", () => {
        SceneView.lastActiveSceneView.FrameSelected(true);
      } },

      { "Edit/Select All", () => {
        EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("SelectAll"));
      } }

      // { "Edit/Project Settings/Input", () => {
      //   throw new NotImplementedException();
      // } },

      // { "Edit/Project Settings/Tags and Layers", () => {
      //   throw new NotImplementedException();
      // } },

      // { "Edit/Project Settings/Audio", () => {
      //   throw new NotImplementedException();
      // } },

      // { "Edit/Project Settings/Time", () => {
      //   throw new NotImplementedException();
      // } },

      // { "Edit/Project Settings/Player", () => {
      //   throw new NotImplementedException();
      // } },

      // { "Edit/Project Settings/Physics", () => {
      //   throw new NotImplementedException();
      // } },

      // { "Edit/Project Settings/Physics 2D", () => {
      //   throw new NotImplementedException();
      // } },

      // { "Edit/Project Settings/Quality", () => {
      //   throw new NotImplementedException();
      // } },

      // { "Edit/Project Settings/Graphics", () => {
      //   throw new NotImplementedException();
      // } },

      // { "Edit/Project Settings/Network", () => {
      //   throw new NotImplementedException();
      // } },

      // { "Edit/Project Settings/Editor", () => {
      //   throw new NotImplementedException();
      // } },

      // { "Edit/Project Settings/Script Execution Order", () => {
      //   throw new NotImplementedException();
      // } },

      // { "Edit/Render Settings", () => {
      //   throw new NotImplementedException();
      // } }
    };
  }
  #endif
}