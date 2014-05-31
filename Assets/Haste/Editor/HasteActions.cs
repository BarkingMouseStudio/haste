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
      { "Unity/Preferences", () => {
        HasteUtils.UnityEditorInvoke("UnityEditor.PreferencesWindow", "ShowPreferencesWindow");
      } },

      { "File/New Scene", () => {
      } },
      { "File/Open Scene...", () => {
      } },
      { "File/Save Scene", () => {
      } },
      { "File/Save Scene as...", () => {
      } },

      { "File/New Project...", () => {
      } },
      { "File/Open Project...", () => {
      } },
      { "File/Save Project", () => {
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
      } },

      { "Edit/Project Settings/Input", () => {
      } },
      { "Edit/Project Settings/Tags and Layers", () => {
      } },
      { "Edit/Project Settings/Audio", () => {
      } },
      { "Edit/Project Settings/Time", () => {
      } },
      { "Edit/Project Settings/Player", () => {
      } },
      { "Edit/Project Settings/Physics", () => {
      } },
      { "Edit/Project Settings/Physics 2D", () => {
      } },
      { "Edit/Project Settings/Quality", () => {
      } },
      { "Edit/Project Settings/Graphics", () => {
      } },
      { "Edit/Project Settings/Network", () => {
      } },
      { "Edit/Project Settings/Editor", () => {
      } },
      { "Edit/Project Settings/Script Execution Order", () => {
      } },

      { "Edit/Render Settings", () => {
      } }
    };
  }
}