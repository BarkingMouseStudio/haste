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

    public static Assembly EditorAssembly {
      get {
        return typeof(EditorWindow).Assembly;
      }
    }

    public static Assembly EngineAssembly {
      get {
        return typeof(ScriptableObject).Assembly;
      }
    }

    public static System.Object Invoke(Assembly assembly, string className, string methodName, System.Object obj = null, params System.Object[] parameters) {
      var T = assembly.GetType(className);
      var method = T.GetMethod(methodName, BindingFlags.NonPublic|BindingFlags.Static);
      return method.Invoke(obj, parameters);
    }

    public static System.Object Instantiate(Assembly assembly, string typeName) {
      Type type = assembly.GetType(typeName);
      return Activator.CreateInstance(type);
    }

    public static IEnumerable<System.Object> GetAttributesInAssembly(Assembly assembly, string attributeTypeName) {
      Type attributeType = assembly.GetType(attributeTypeName, true);
      return GetAttributesInAssembly(assembly, attributeType);
    }

    public static IEnumerable<System.Object> GetAttributesInAssembly(Assembly assembly, Type attributeType) {
      var flags = BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static|BindingFlags.DeclaredOnly;
      foreach (var type in assembly.GetTypes()) {
        foreach (var info in type.GetMethods(flags)) {
          foreach (var attribute in info.GetCustomAttributes(attributeType, true)) {
            yield return attribute;
          }
        }
      }
    }

    public static IEnumerable<Type> FindDerivedTypesInAssembly(Assembly assembly, string typeName) {
      Type derivedType = assembly.GetType(typeName, true);
      foreach (Type type in assembly.GetTypes()) {
        if (type != derivedType && derivedType.IsAssignableFrom(type)) {
          yield return type;
        }
      }
    }

    public static IEnumerable<Type> FindTypesInAssembly(Assembly assembly, string typeName) {
      foreach (Type type in assembly.GetTypes()) {
        if (type.FullName.IndexOf(typeName) != -1) {
          yield return type;
        }
      }
    }

    public static IEnumerable<MethodInfo> FindMethodsInAssembly(Assembly assembly, string methodName) {
      foreach (Type type in assembly.GetTypes()) {
        BindingFlags flags = BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static|BindingFlags.DeclaredOnly;
        foreach (MethodInfo info in type.GetMethods(flags)) {
          if (info.Name.IndexOf(methodName) != -1) {
            yield return info;
          }
        }
      }
    }

    public static Texture GrabScreenSwatch(Rect rect) {
      int width = (int)rect.width;
      int height = (int)rect.height;
      int x = (int)rect.x;
      int y = (int)rect.y;
      Vector2 position = new Vector2(x, y);

      Color[] pixels = UnityEditorInternal.InternalEditorUtility.ReadScreenPixel(position, width, height);

      Texture2D texture = new Texture2D(width, height);
      texture.SetPixels(pixels);
      texture.Apply();

      return texture;
    }

    public static void SelectByProjectPath(string path) {
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

    public static string GetHomeFolder() {
      return String.Join(Path.DirectorySeparatorChar.ToString(),
        Application.dataPath.Split(Path.DirectorySeparatorChar).Slice(0, 3));
    }

    public static string BoldLabel(string str, int[] indices, string boldStart = "<color=\"white\">", string boldEnd = "</color>") {
      string bolded = "";

      // TODO: This could iterate indices and use substring instead
      int index = 0;
      for (int i = 0; i < str.Length; i++) {
        if (index < indices.Length && i == indices[index]) {
          bolded += boldStart + str[i] + boldEnd;
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
