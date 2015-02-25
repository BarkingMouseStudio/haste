using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Haste {

  public static class HasteUtils {

    // This is a shortcut instead of Mathf.Approximately which is slow.
    // We don't need perfect comparisons: its called "fuzzy matching".
    public static bool Approximately(float a, float b) {
      if (a > b) {
        return (a - b) < Mathf.Epsilon;
      } else if (a < b) {
        return (b - a) < Mathf.Epsilon;
      } else {
        return true;
      }
    }

    public static string[] Layouts {
      get {
        var WindowLayout = Type.GetType("UnityEditor.WindowLayout,UnityEditor");
        var layoutsPreferencesPath = (string)WindowLayout.GetProperty("layoutsPreferencesPath", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).GetValue(WindowLayout, null);
        return Directory.GetFiles(layoutsPreferencesPath).Select((path) => {
          return Path.GetFileNameWithoutExtension(path);
        }).Where((path) => {
          return !path.Contains("LastLayout");
        }).ToArray();
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
      texture.hideFlags = HideFlags.HideAndDontSave;
      texture.SetPixels(pixels);
      texture.Apply();

      return texture;
    }

    public static string GetHomeFolder() {
      return String.Join(Path.DirectorySeparatorChar.ToString(),
        Application.dataPath.Split(Path.DirectorySeparatorChar).Slice(0, 3));
    }

    public static Texture2D CreateColorSwatch(Color color) {
      Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
      texture.SetPixel(0, 0, color);
      texture.Apply();
      return texture;
    }

    public static string GetRelativeAssetPath(string assetPath) {
      return Path.Combine("Assets", assetPath.TrimStart(Application.dataPath + Path.DirectorySeparatorChar));
    }

    public static string GetHierarchyPath(Transform transform) {
      string path;
      if (transform.parent == null) {
        path = transform.gameObject.name;
      } else {
        path = GetHierarchyPath(transform.parent) +
          Path.DirectorySeparatorChar + transform.gameObject.name;
      }
      return path;
    }
  }
}
