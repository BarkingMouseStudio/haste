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

    // public static void SelectByProjectPath(string path) {
    //   EditorApplication.ExecuteMenuItem("Window/Project");
    //   EditorUtility.FocusProjectWindow();

    //   // Traverse project downwards, pinging each level.
    //   // HACK: This is to fix an issue with Unity where pinging
    //   // a lower-level object does not make it visible.
    //   string[] pieces = path.Split(Path.DirectorySeparatorChar);
    //   string fullPath = "";
    //   for (int i = 0; i < pieces.Length; i++) {
    //     fullPath = Path.Combine(fullPath, pieces[i]);
    //     Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(fullPath);
    //   }
    // }

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

    public static Texture2D CreateColorSwatch(Color color) {
      Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
      texture.SetPixel(0, 0, color);
      texture.Apply();
      return texture;
    }

    public static string GetRelativeAssetPath(string assetPath) {
      return "Assets/" + assetPath.TrimStart(Application.dataPath + "/");
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
