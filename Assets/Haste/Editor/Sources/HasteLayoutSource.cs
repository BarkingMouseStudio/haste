using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;

using System.Diagnostics;

namespace Haste {

  public class HasteLayoutSource : IEnumerable<HasteItem> {

    public const string NAME = "Layout";

    static Type windowLayout;
    static Type WindowLayout {
      get {
        if (windowLayout == null) {
          windowLayout = Type.GetType("UnityEditor.WindowLayout,UnityEditor");
        }
        return windowLayout;
      }
    }

    static string layoutsPreferencesPath = "";
    static string LayoutPreferencesPath {
      get {
        if (layoutsPreferencesPath == "") {
          layoutsPreferencesPath = (string)WindowLayout.GetProperty("layoutsPreferencesPath", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
            .GetValue(windowLayout, null);
        }
        return layoutsPreferencesPath;
      }
    }

    public IEnumerator<HasteItem> GetEnumerator() {
      string layoutName;
      foreach (string layoutPath in Directory.GetFiles(LayoutPreferencesPath)) {
        if (!layoutPath.Contains("LastLayout")) {
          layoutName = Path.GetFileNameWithoutExtension(layoutPath);
          yield return new HasteItem(String.Format("Window/Layouts/{0}", layoutName), 0, NAME);
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}
