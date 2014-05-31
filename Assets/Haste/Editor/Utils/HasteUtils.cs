using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Haste {

  public static class HasteUtils {

    public static string GetHomeFolder() {
      return String.Join(Path.DirectorySeparatorChar.ToString(),
        Application.dataPath.Split(Path.DirectorySeparatorChar).Slice(0, 3));
    }

    public static System.Object Invoke(Assembly assembly, string className, string methodName, System.Object obj = null, params System.Object[] parameters) {
      var T = assembly.GetType(className);
      var method = T.GetMethod(methodName, BindingFlags.NonPublic|BindingFlags.Static);
      return method.Invoke(obj, parameters);
    }

    public static System.Object UnityEngineInvoke(string className, string method, System.Object obj = null, params System.Object[] parameters) {
      var assembly = Assembly.GetAssembly(typeof(ScriptableObject));
      return Invoke(assembly, className, method, obj, parameters);
    }

    public static System.Object UnityEditorInvoke(string className, string method, System.Object obj = null, params System.Object[] parameters) {
      var assembly = Assembly.GetAssembly(typeof(EditorWindow));
      return Invoke(assembly, className, method, obj, parameters);
    }

    public static UnityEngine.Object Clone(GameObject go) {
      var prefabRoot = PrefabUtility.GetPrefabParent(go);
      if (prefabRoot != null) {
        return PrefabUtility.InstantiatePrefab(prefabRoot);
      } else {
        return UnityEngine.Object.Instantiate(go);
      }
    }

    public static UnityEngine.Object GetLightmapSettings() {
      return (UnityEngine.Object)UnityEditorInvoke("UnityEditor.LightmapEditorSettings", "GetLightmapSettings");
    }

    public static void FindType(string typeToFind) {
      var assembly = Assembly.GetAssembly(typeof(EditorWindow));
      foreach (var type in assembly.GetTypes()) {
        foreach (var info in type.GetMethods(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static|BindingFlags.DeclaredOnly)) {
          if (info.Name.IndexOf(typeToFind) != -1) {
            HasteLogger.Info(info.Name, type.FullName);
          }
        }
      }
    }

    public static string BoldLabel(string str, int[] indices) {
      string bolded = "";

      // TODO: This could iterate indices and use substring instead
      int index = 0;
      for (int i = 0; i < str.Length; i++) {
        if (index < indices.Length && i == indices[index]) {
          bolded += "<color=\"white\">" + str[i] + "</color>";
          index++;
        } else {
          bolded += str[i];
        }
      }

      return bolded;
    }

    public static Texture2D CreateTexture(Color color) {
      Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
      texture.SetPixel(0, 0, color);
      texture.Apply();
      return texture;
    }

    // public static HasteResult GetResultFromObject(UnityEngine.Object obj) {
    //   if (AssetDatabase.Contains(obj)) {
    //     return new HasteResult(AssetDatabase.GetAssetPath(obj), obj.GetInstanceID(), HasteSource.Project);
    //   } else if (obj.GetType() == typeof(Transform)) {
    //     Transform transform = (Transform)obj;
    //     return new HasteResult(HasteUtils.GetHierarchyPath(transform), obj.GetInstanceID(), HasteSource.Hierarchy);
    //   } else if (obj.GetType() == typeof(GameObject)) {
    //     GameObject go = (GameObject)obj;
    //     return new HasteResult(HasteUtils.GetHierarchyPath(go.transform), obj.GetInstanceID(), HasteSource.Hierarchy);
    //   } else {
    //     return new HasteResult(obj.name, obj.GetInstanceID(), HasteSource.Unknown);
    //   }
    // }

    // public static HasteResult[] GetResultsFromObjects(UnityEngine.Object[] objects) {
    //   return objects.Select(obj => GetResultFromObject(obj)).ToArray();
    // }

    public static Regex GetFuzzyFilterRegex(string query) {
      return new Regex(String.Join(".*", query.ToLower().ToCharArray().Select(c => c.ToString()).ToArray()));
    }

    public static string GetRelativeAssetPath(string assetPath) {
      return "Assets/" + assetPath.TrimStart(Application.dataPath + "/");
    }

    public static string GetHierarchyPath(Transform transform) {
      string path;

      if (transform.parent == null) {
        path = transform.gameObject.name;
      } else {
        path = HasteUtils.GetHierarchyPath(transform.parent) +
          Path.DirectorySeparatorChar + transform.gameObject.name;
      }

      return path;
    }
  }
}
