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

    public static Assembly EditorAssembly {
      get {
        return typeof(EditorWindow).Assembly;
      }
    }

    // public static T GetPropValue<T>(object obj, string propName) {
    //   return (T)obj.GetType().GetProperty(propName).GetValue(obj, null);
    // }

    // public static System.Object Instantiate(Assembly assembly, string typeName, params object[] args) {
    //   return Activator.CreateInstance(assembly.GetType(typeName), args);
    // }

    public static System.Object Invoke(Assembly assembly, string className, string methodName, System.Object obj = null, params System.Object[] parameters) {
      var T = assembly.GetType(className);
      var method = T.GetMethod(methodName, BindingFlags.NonPublic|BindingFlags.Static);
      return method.Invoke(obj, parameters);
    }

    public static IEnumerable<HasteTuple<T, MethodInfo>> GetAttributesInAssembly<T>(Assembly assembly) {
      var flags = BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static|BindingFlags.DeclaredOnly;
      foreach (var type in assembly.GetTypes()) {
        foreach (var methodInfo in type.GetMethods(flags)) {
          foreach (var attribute in methodInfo.GetCustomAttributes(typeof(T), true)) {
            yield return HasteTuple.Create((T)attribute, methodInfo);
          }
        }
      }
    }
  }
}
