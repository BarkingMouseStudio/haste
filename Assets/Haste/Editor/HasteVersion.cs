using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public static class HasteVersion {

    public static bool TryParse(string str, out Version version) {
      try {
        var data = (Dictionary<string, object>)JSON.Deserialize(str);

        var major = Convert.ToInt32((long)data["major"]);
        var minor = Convert.ToInt32((long)data["minor"]);
        var patch = Convert.ToInt32((long)data["patch"]);

        version = new Version(major, minor, patch);
        return true;
      } catch {
        version = new Version();
        return false;
      }
    }
  }
}