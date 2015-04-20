using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Haste {

  class IgnorePathsProxy : ScriptableObject {

    [HideInInspector]
    public List<string> paths;

    public int Length {
      get {
        return paths.Count;
      }
    }

    public void Add(string path) {
      paths.Add(path);
    }

    public void Remove(string path) {
      paths.Remove(path);
    }

    public void RemoveAt(int index) {
      paths.RemoveAt(index);
    }

    public void Save() {
      HasteSettings.IgnorePaths = string.Join(",", paths.ToArray());
      Haste.Rebuild();
    }

    public string this[int index] {
      get {
        return paths[index];
      }
      set {
        paths[index] = value;
      }
    }

    public bool Contains(string path) {
      return paths.Contains(path);
    }

    public IgnorePathsProxy Init() {
      paths = HasteSettings.IgnorePaths.Trim().Split(',')
        .Select((path) => path.Trim())
        .Where((path) => !string.IsNullOrEmpty(path))
        .ToList();
      return this;
    }
  }

  public static class HasteIgnore {

    static IgnorePathsProxy proxy;
    static IgnorePathsProxy Proxy {
      get {
        if (proxy == null) {
          proxy = ScriptableObject.CreateInstance<IgnorePathsProxy>().Init();
          proxy.hideFlags = HideFlags.HideAndDontSave;
        }
        return proxy;
      }
    }

    static ReorderableList reorderableList;
    static ReorderableList List {
      get {
        if (reorderableList == null) {
          reorderableList = new ReorderableList(Proxy.paths, typeof(IgnorePathsProxy), true, true, true, true);
          reorderableList.drawHeaderCallback += (rect) => {
            EditorGUI.LabelField(rect, "Ignore Paths");
          };
          reorderableList.drawElementCallback += (rect, index, active, focused) => {
            // Check for weird extra list items
            if (index >= 0 && index < Proxy.Length) {
              EditorGUI.BeginChangeCheck();
              rect = new Rect(rect.x, rect.y + 1, rect.width, rect.height - 4);
              Proxy[index] = EditorGUI.TextField(rect, Proxy[index]);
              if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(Proxy);
              }
            }
          };
          reorderableList.onAddCallback += (list) => {
            Proxy.Add("");
            EditorUtility.SetDirty(Proxy);
          };
          reorderableList.onRemoveCallback += (list) => {
            Proxy.RemoveAt(list.index);
            EditorUtility.SetDirty(Proxy);
          };
        }
        return reorderableList;
      }
    }

    public static void DrawPreferences() {
      List.DoLayoutList();

      if (GUILayout.Button("Save Ignore Paths", GUILayout.Width(128))) {
        Proxy.Save();
      }

      EditorGUILayout.Space();
      EditorGUILayout.HelpBox("Add paths to ignore when indexing assets. Useful for excluding folders you do not want to see in results. You can also right-click on folders and select Haste > Ignore to exclude them.", MessageType.Info);
    }

    [MenuItem("Assets/Haste/Ignore")]
    public static void Ignore() {
      var path = AssetDatabase.GetAssetPath(Selection.activeObject);
      Proxy.Add(path);
      Proxy.Save();
    }

    [MenuItem("Assets/Haste/Ignore", true)]
    public static bool CanIgnore() {
      var selection = Selection.activeObject;
      if (selection == null) {
        return false;
      }
      if (!AssetDatabase.Contains(selection)) {
        return false; // invalid asset
      }
      var path = AssetDatabase.GetAssetPath(selection);
      if (!Directory.Exists(path)) {
        return false; // invalid directory
      }
      if (Proxy.Contains(path)) {
        return false; // already ignored
      }
      return true;
    }

    [MenuItem("Assets/Haste/Unignore")]
    public static void Unignore() {
      var path = AssetDatabase.GetAssetPath(Selection.activeObject);
      Proxy.Remove(path);
      Proxy.Save();
    }

    [MenuItem("Assets/Haste/Unignore", true)]
    public static bool CanUnignore() {
      var selection = Selection.activeObject;
      if (selection == null) {
        return false;
      }
      if (!AssetDatabase.Contains(selection)) {
        return false; // invalid asset
      }
      var path = AssetDatabase.GetAssetPath(selection);
      if (!Directory.Exists(path)) {
        return false; // invalid directory
      }
      if (!Proxy.Contains(path)) {
        return false; // not ignored
      }
      return true;
    }
  }
}
