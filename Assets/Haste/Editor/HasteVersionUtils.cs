using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Haste {

  public static class HasteVersionUtils {

    public static bool TryParse(string str, out Version version) {
      try {
        var data = (Dictionary<string, object>)JSON.Deserialize(str);
        version = new Version((string)data["version"]);
        return true;
      } catch {
        version = new Version();
        return false;
      }
    }

    private static Version unityVersion;
    public static Version UnityVersion {
      get {
        if (unityVersion == null) {
          string unityVersionStr = Application.unityVersion;
          int versionPostfix = unityVersionStr.IndexOfAny("abcdefghijklmnopqrstuvwxyz".ToCharArray());
          if (versionPostfix != -1) {
            unityVersionStr = unityVersionStr.Remove(versionPostfix);
          }
          unityVersion = new Version(unityVersionStr);
        }
        return unityVersion;
      }
    }

    private static Version unity5Version;
    public static Version Unity5Version {
      get {
        if (unity5Version == null) {
          unity5Version = new Version(5, 0, 0);
        }
        return unity5Version;
      }
    }

    public static bool IsUnity5 {
      get {
        return UnityVersion >= Unity5Version;
      }
    }
  }
}
