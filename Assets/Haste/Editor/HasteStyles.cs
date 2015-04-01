using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Haste {

  public static class HasteStyles {

    static Texture2D CreateColorSwatch(Color color) {
      Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
      texture.SetPixel(0, 0, color);
      texture.Apply();
      texture.hideFlags = HideFlags.HideAndDontSave;
      return texture;
    }

    public static readonly int WindowWidth = 500;
    public static readonly int WindowHeight = 300;
    public static readonly int ItemHeight = 46;

    public static readonly string SelectionSymbol = "\u25cf";

    public static readonly string BoldEnd = "</b></color>";

    private static string boldStart = "";
    public static string BoldStart {
      get {
        if (string.IsNullOrEmpty(boldStart)) {
          boldStart = EditorGUIUtility.isProSkin ? "<color=\"#aaa\"><b>" : "<color=\"#222\"><b>";
        }
        return boldStart;
      }
    }

    private static string highlightedBoldStart = "";
    public static string HighlightedBoldStart {
      get {
        if (string.IsNullOrEmpty(highlightedBoldStart)) {
          highlightedBoldStart = EditorGUIUtility.isProSkin ? "<color=\"#ddd\"><b>" : "<color=\"#eee\"><b>";
        }
        return highlightedBoldStart;
      }
    }

    private static GUIStyle selectionStyle;
    public static GUIStyle SelectionStyle {
      get {
        if (selectionStyle == null) {
          selectionStyle = new GUIStyle();
          selectionStyle.normal.background = CreateColorSwatch(HastePalette.Current.SelectionColor);
        }
        return selectionStyle;
      }
    }

    private static GUIStyle highlightStyle;
    public static GUIStyle HighlightStyle {
      get {
        if (highlightStyle == null) {
          highlightStyle = new GUIStyle();
          highlightStyle.normal.background = CreateColorSwatch(HastePalette.Current.HighlightColor);
        }
        return highlightStyle;
      }
    }

    // We use a separate non-highlight style since GUIStyle.none has some extra padding...
    private static GUIStyle emptyStyle;
    public static GUIStyle EmptyStyle {
      get {
        if (emptyStyle == null) {
          emptyStyle = new GUIStyle();
        }
        return emptyStyle;
      }
    }

    public static void LoadSkin() {
      if (EditorGUIUtility.isProSkin) {
        skin = HasteResources.Load<GUISkin>("Skins/Pro.guiskin");
        skin.hideFlags = HideFlags.HideAndDontSave;
      } else {
        skin = HasteResources.Load<GUISkin>("Skins/Personal.guiskin");
        skin.hideFlags = HideFlags.HideAndDontSave;
      }
    }

    private static GUISkin skin;
    public static GUISkin Skin {
      get {
        if (skin == null) {
          LoadSkin();
        }
        return skin;
      }
    }

    private static readonly string glyphs = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_+=~`[]{}|\\:;\"'<>,.?/ ";
    public static IEnumerator PreCacheDynamicFonts() {
      GUISkin inspectorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
      foreach (GUIStyle style in Skin.customStyles) {
        Font font = style.font != null ? style.font : inspectorSkin.font;
        if (style.richText) {
          foreach (char glyph in glyphs) {
            font.RequestCharactersInTexture(glyph.ToString(), style.fontSize, FontStyle.Bold);
          }
          yield return null;
        }
      }
    }
  }
}
