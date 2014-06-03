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
      // TODO: These should just be implemented as MenuItems so they get validation functions and are read like everything else
      // TODO: All should operate on "Selection" arrays, not one-offs
      { "Assets/Instantiate Prefab", () => {
        var selectedObject = Selection.activeObject;
        PrefabType prefabType = PrefabUtility.GetPrefabType(Selection.activeObject);
        if (prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab) {
          Selection.activeObject = PrefabUtility.InstantiatePrefab(Selection.activeObject);
          Undo.RegisterCreatedObjectUndo(Selection.activeObject, "Instantiated prefab");
        }
      } },

      { "GameObject/Lock", () => {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null) {
          selectedObject.hideFlags = HideFlags.NotEditable;
        }
      } },

      { "GameObject/Unlock", () => {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null) {
          selectedObject.hideFlags = HideFlags.None;
        }
      } },

      { "GameObject/Activate", () => {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null) {
          EditorUtility.SetObjectEnabled(selectedObject, true);
        }
      } },

      { "GameObject/Deactivate", () => {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null) {
          EditorUtility.SetObjectEnabled(selectedObject, false);
        }
      } },

      { "GameObject/Reset Transform", () => {
        Transform selectedTransform = Selection.activeTransform;
        if (selectedTransform != null) {
          selectedTransform.localPosition = Vector3.zero;
          selectedTransform.localScale = Vector3.one;
          selectedTransform.localRotation = Quaternion.identity;
        }
      } },

      { "GameObject/Select Parent", () => {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null) {
          if (selectedObject.transform != null) {
            if (selectedObject.transform.parent != null) {
              Selection.activeGameObject = selectedObject.transform.parent.gameObject;
            }
          }
        }
      } },

      { "GameObject/Select Children", () => {
        GameObject selectedObject = Selection.activeGameObject;
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
      } },

      // Prefab
      { "GameObject/Select Prefab", () => {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null) {
          var parentObject = PrefabUtility.GetPrefabParent(selectedObject);
          if (parentObject != null) {
            HasteUtils.SelectByProjectPath(AssetDatabase.GetAssetPath(parentObject));
          }
        }
      } },

      { "GameObject/Revert to Prefab", () => {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null) {
          Undo.RegisterFullObjectHierarchyUndo(selectedObject);
          PrefabUtility.RevertPrefabInstance(selectedObject);
        }
      } },

      { "GameObject/Reconnect to Prefab", () => {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null) {
          Undo.RegisterFullObjectHierarchyUndo(selectedObject);
          PrefabUtility.ReconnectToLastPrefab(selectedObject);
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