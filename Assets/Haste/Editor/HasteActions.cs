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

    public delegate void MenuItemFallbackDelegate();

    public static IDictionary<string, MenuItemFallbackDelegate> MenuItemFallbacks = new Dictionary<string, MenuItemFallbackDelegate>() {
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
}