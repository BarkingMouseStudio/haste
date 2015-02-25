using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Haste {

  internal static class ReflectionHelpers {

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
