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

        var instantiatedPrefabs = new List<UnityEngine.Object>(Selection.objects.Length);

        using (new HasteUndoStack("Instantiate Prefabs")) {
          foreach (var selectedObject in Selection.objects) {
            PrefabType prefabType = PrefabUtility.GetPrefabType(selectedObject);
            if (prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab) {
              var instantiatedPrefab = PrefabUtility.InstantiatePrefab(selectedObject);
              instantiatedPrefabs.Add(instantiatedPrefab);
              Undo.RegisterCreatedObjectUndo(instantiatedPrefab, "Instantiate Prefab");
            }
          }
        }

        Selection.objects = instantiatedPrefabs.ToArray();
      } },

      { "GameObject/Lock", () => {
        if (Selection.gameObjects.Length == 0) {
          return;
        }

        foreach (var selectedObject in Selection.gameObjects) {
          selectedObject.hideFlags |= HideFlags.NotEditable;
          EditorUtility.SetDirty(selectedObject);
        }
      } },

      { "GameObject/Unlock", () => {
        if (Selection.gameObjects.Length == 0) {
          return;
        }

        foreach (var selectedObject in Selection.gameObjects) {
          selectedObject.hideFlags &= ~HideFlags.NotEditable;
          EditorUtility.SetDirty(selectedObject);
        }
      } },

      { "GameObject/Activate", () => {
        if (Selection.gameObjects.Length == 0) {
          return;
        }

        Undo.RecordObjects(Selection.gameObjects, "Activate GameObjects");
        foreach (var selectedObject in Selection.gameObjects) {
          EditorUtility.SetObjectEnabled(selectedObject, true);
        }
      } },

      { "GameObject/Deactivate", () => {
        if (Selection.gameObjects.Length == 0) {
          return;
        }

        Undo.RecordObjects(Selection.gameObjects, "Deactivate GameObjects");
        foreach (var selectedObject in Selection.gameObjects) {
          EditorUtility.SetObjectEnabled(selectedObject, false);
        }
      } },

      { "GameObject/Reset Transform", () => {
        if (Selection.transforms.Length == 0) {
          return;
        }

        Undo.RecordObjects(Selection.transforms, "Reset Transforms");
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

        using (new HasteUndoStack("Revert to Prefabs")) {
          foreach (var selectedObject in Selection.gameObjects) {
            Undo.RegisterFullObjectHierarchyUndo(selectedObject, "Revert to Prefab");
            PrefabUtility.RevertPrefabInstance(selectedObject);
          }
        }
      } },

      { "GameObject/Reconnect to Prefab", () => {
        if (Selection.gameObjects.Length == 0) {
          return;
        }

        using (new HasteUndoStack("Reconnect to Prefabs")) {
          foreach (var selectedObject in Selection.gameObjects) {
            Undo.RegisterFullObjectHierarchyUndo(selectedObject, "Reconnect to Prefab");
            PrefabUtility.ReconnectToLastPrefab(selectedObject);
          }
        }
      } },
      #endregion

      { "Unity/About Unity...", () => { // OS X
        var AboutWindow = typeof(EditorWindow).Assembly.GetType("UnityEditor.AboutWindow");
        EditorWindow.GetWindow(AboutWindow, true, "About Unity");
      } },

      { "Help/About Unity...", () => { // Windows
        var AboutWindow = typeof(EditorWindow).Assembly.GetType("UnityEditor.AboutWindow");
        EditorWindow.GetWindow(AboutWindow, true, "About Unity");
      } },

      { "Unity/Preferences...", () => { // OS X
        HasteReflection.Invoke(HasteReflection.EditorAssembly, "UnityEditor.PreferencesWindow", "ShowPreferencesWindow");
      } },

      { "File/Preferences...", () => { // Windows
        HasteReflection.Invoke(HasteReflection.EditorAssembly, "UnityEditor.PreferencesWindow", "ShowPreferencesWindow");
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
        HasteReflection.Invoke(HasteReflection.EditorAssembly, "UnityEditor.BuildPlayerWindow", "ShowBuildPlayerWindow");
      } },

      { "File/Build & Run", () => {
        HasteReflection.Invoke(HasteReflection.EditorAssembly, "UnityEditor.BuildPlayerWindow", "BuildPlayerAndRun");
      } },

      { "File/Build in Cloud...", () => {
        Application.OpenURL("http://build.connect.unity3d.com/");
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
      } },

      { "Edit/Play", () => {
        EditorApplication.isPlaying = !EditorApplication.isPlaying;
      } },

      { "Edit/Pause", () => {
        EditorApplication.isPaused = !EditorApplication.isPaused;
      } },

      { "Edit/Step", () => {
        EditorApplication.Step();
      } },

      // { "Edit/Project Settings/Input", () => {
      //   TODO: throw new NotImplementedException();
      // } },

      { "Edit/Project Settings/Tags and Layers", () => {
        Type type = HasteReflection.EditorAssembly.GetType("UnityEditor.TagManager");
        Selection.activeObject = Resources.FindObjectsOfTypeAll(type).First();
      } },

      // { "Edit/Project Settings/Audio", () => {
      //   TODO: throw new NotImplementedException();
      // } },

      // { "Edit/Project Settings/Time", () => {
      //   TODO: throw new NotImplementedException();
      // } },

      { "Edit/Project Settings/Player", () => {
        Selection.activeObject = Resources.FindObjectsOfTypeAll<PlayerSettings>().First();
        EditorApplication.ExecuteMenuItem("Window/Inspector");
      } },

      { "Edit/Project Settings/Physics", () => {
        Type type = HasteReflection.EditorAssembly.GetType("UnityEditor.PhysicsManager");
        Selection.activeObject = Resources.FindObjectsOfTypeAll(type).First();
        EditorApplication.ExecuteMenuItem("Window/Inspector");
      } },

      { "Edit/Project Settings/Physics 2D", () => {
        Type type = HasteReflection.EditorAssembly.GetType("UnityEditor.Physics2DSettings");
        Selection.activeObject = Resources.FindObjectsOfTypeAll(type).First();
        EditorApplication.ExecuteMenuItem("Window/Inspector");
      } },

      { "Edit/Project Settings/Quality", () => {
        Selection.activeObject = Resources.FindObjectsOfTypeAll<QualitySettings>().First();
        EditorApplication.ExecuteMenuItem("Window/Inspector");
      } },

      // { "Edit/Project Settings/Graphics", () => {
      //   TODO: throw new NotImplementedException();
      // } },

      // { "Edit/Project Settings/Network", () => {
      //   TODO: throw new NotImplementedException();
      // } },

      { "Edit/Project Settings/Editor", () => {
        Selection.activeObject = Resources.FindObjectsOfTypeAll<EditorSettings>().First();
        EditorApplication.ExecuteMenuItem("Window/Inspector");
      } },

      { "Edit/Project Settings/Script Execution Order", () => {
        Type type = HasteReflection.EditorAssembly.GetType("UnityEditor.MonoManager");
        Selection.activeObject = Resources.FindObjectsOfTypeAll(type).First();
        EditorApplication.ExecuteMenuItem("Window/Inspector");
      } },

      { "Edit/Render Settings", () => {
        Selection.activeObject = UnityEngine.Object.FindObjectOfType<RenderSettings>();
        EditorApplication.ExecuteMenuItem("Window/Inspector");
      } }
    };
  }
  #endif
}