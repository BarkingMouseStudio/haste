using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Haste {
  public static class HasteActions {

    public static IDictionary<string, Action> MenuActions = new Dictionary<string, Action>(){
      { "Unity/Preferences...", () => {
        var asm = Assembly.GetAssembly(typeof(EditorWindow));
        var T = asm.GetType("UnityEditor.PreferencesWindow");
        var method = T.GetMethod("ShowPreferencesWindow", BindingFlags.NonPublic|BindingFlags.Static);
        method.Invoke(null, null);
      } },

      { "Edit/Frame Selected", () => { SceneView.lastActiveSceneView.FrameSelected(); } },
    };


    public static void DefaultAction(HasteResult result) {
      switch (result.Source) {
        case HasteSource.Project:
          EditorApplication.ExecuteMenuItem("Window/Project");
          Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(result.Path);
          EditorGUIUtility.PingObject(Selection.activeObject);
          break;
        case HasteSource.Hierarchy:
          EditorApplication.ExecuteMenuItem("Window/Hierarchy");
          Selection.activeObject = GameObject.Find(result.Path);
          EditorGUIUtility.PingObject(Selection.activeObject);
          break;
        case HasteSource.Editor:
          if (HasteActions.MenuActions.ContainsKey(result.Path)) {
            HasteActions.MenuActions[result.Path]();
          } else {
            EditorApplication.ExecuteMenuItem(result.Path);
          }
          break;
      }
    }
  }
}