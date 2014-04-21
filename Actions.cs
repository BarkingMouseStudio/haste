using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Haste {
  public static class Actions {

    static Actions() {
      var asm = Assembly.GetAssembly(typeof(EditorWindow));
      var T = asm.GetTypes();
      Logger.Info(T);
    }

    public static IDictionary<string, Action> MenuActions = new Dictionary<string, Action>(){
      { "Unity/Preferences...", () => {
        var asm = Assembly.GetAssembly(typeof(EditorWindow));
        var T = asm.GetType("UnityEditor.PreferencesWindow");
        var method = T.GetMethod("ShowPreferencesWindow", BindingFlags.NonPublic|BindingFlags.Static);
        method.Invoke(null, null);
      } },

      { "Edit/Frame Selected", () => { SceneView.lastActiveSceneView.FrameSelected(); } },
    };


    public static void DefaultAction(Item item) {
      switch (item.Source) {
        case Source.Project:
          EditorApplication.ExecuteMenuItem("Window/Project");
          Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(item.Path);
          EditorGUIUtility.PingObject(Selection.activeObject);
          break;
        case Source.Hierarchy:
          EditorApplication.ExecuteMenuItem("Window/Hierarchy");
          Selection.activeObject = GameObject.Find(item.Path);
          EditorGUIUtility.PingObject(Selection.activeObject);
          break;
        case Source.Editor:
          if (Actions.MenuActions.ContainsKey(item.Path)) {
            Actions.MenuActions[item.Path]();
          } else {
            EditorApplication.ExecuteMenuItem(item.Path);
          }
          break;
      }
    }
  }
}