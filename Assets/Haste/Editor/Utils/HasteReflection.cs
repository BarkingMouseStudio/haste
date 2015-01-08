using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Haste {

  public static class HasteReflection {

    public static string[] Layouts {
      get {
        var WindowLayout = Type.GetType("UnityEditor.WindowLayout,UnityEditor");
        var layoutsPreferencesPath = (string)WindowLayout.GetProperty("layoutsPreferencesPath", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).GetValue(WindowLayout, null);
        return Directory.GetFiles(layoutsPreferencesPath).Select((path) => {
          return Path.GetFileNameWithoutExtension(path);
        }).ToArray();
      }
    }

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
  }
}
