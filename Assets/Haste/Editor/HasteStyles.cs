using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public static class HasteStyles {

    public static readonly int WindowWidth = 500;
    public static readonly int WindowHeight = 300;
    public static readonly int ItemHeight = 46;
    public static readonly float ListHeight = 230;

    public static int PageSize {
      get {
        return Mathf.FloorToInt(ListHeight / ItemHeight);
      }
    }

    public static readonly string SelectionSymbol = "\u25cf";

    static IDictionary<string, GUIStyle> styles =
      new Dictionary<string, GUIStyle>();

    readonly static Color TRANSPARENT = new Color();

    public static readonly string BoldEnd = "</b></color>";

    private static string boldStart = "";
    public static string BoldStart {
      get {
        if (string.IsNullOrEmpty(boldStart)) {
          boldStart = EditorGUIUtility.isProSkin ? "<color=\"#aaa\"><b>" : "<color=\"#555\"><b>";
        }
        return boldStart;
      }
    }

    public static readonly string HighlightedBoldStart = "<color=\"#ddd\"><b>";

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

    public static GUIStyle GetStyle(string name) {
      GUIStyle style;
      if (!styles.TryGetValue(name, out style)) {
        Debug.LogWarning("[Haste] Failed to load GUIStyle. Make sure that your Haste installation is intact.");
      }
      return style;
    }

    public static IEnumerator Init() {
      yield return Haste.Scheduler.Start(WaitUntilReady());

      AddStyle(new Style() { name = "Query", other = EditorStyles.textField, fixedHeight = 64, font = HasteResources.Load<Font>("Fonts/FiraSans-Regular.ttf"), alignment = TextAnchor.MiddleLeft });

      AddStyle(new Style() { name = "Intro", other = EditorStyles.largeLabel, fixedHeight = 64, fontSize = 32, alignment = TextAnchor.MiddleCenter });
      AddStyle(new Style() { name = "Count", other = EditorStyles.largeLabel, fontSize = 14, alignment = TextAnchor.MiddleRight });
      AddStyle(new Style() { name = "Empty", other = EditorStyles.largeLabel, fixedHeight = 24, fontSize = 16, alignment = TextAnchor.MiddleCenter });
      AddStyle(new Style() { name = "Tip", other = EditorStyles.label, alignment = TextAnchor.MiddleCenter, fontSize = 14, richText = true, wordWrap = true, textColor = HastePalette.Current.SecondaryColor });
      AddStyle(new Style() { name = "Usage", other = EditorStyles.label, wordWrap = true });
      AddStyle(new Style() { name = "Upgrade", other = EditorStyles.label, alignment = TextAnchor.MiddleCenter, fontSize = 14, textColor = HastePalette.Current.HighlightColor });
      AddStyle(new Style() { name = "Indexing", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleCenter, fontSize = 14, textColor = HastePalette.Current.SecondaryColor });

      AddStyle(new Style() { name = "Name", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = HastePalette.Current.PrimaryColor });
      AddStyle(new Style() { name = "DisabledName", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = HastePalette.Current.DisabledColor });
      AddStyle(new Style() { name = "HighlightedName", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = HastePalette.Current.HighlightedPrimaryColor });
      AddStyle(new Style() { name = "HighlightedDisabledName", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = HastePalette.Current.HighlightedDisabledColor });

      AddStyle(new Style() { name = "PrefabName", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = HastePalette.Current.PrefabColor });
      AddStyle(new Style() { name = "BrokenPrefabName", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = HastePalette.Current.BrokenPrefabColor });
      AddStyle(new Style() { name = "DisabledPrefabName", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = HastePalette.Current.DisabledPrefabColor });
      AddStyle(new Style() { name = "DisabledBrokenPrefabName", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = HastePalette.Current.DisabledBrokenPrefabColor });

      AddStyle(new Style() { name = "HighlightedPrefabName", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = HastePalette.Current.HighlightedPrefabColor });
      AddStyle(new Style() { name = "HighlightedBrokenPrefabName", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = HastePalette.Current.HighlightedBrokenPrefabColor });
      AddStyle(new Style() { name = "HighlightedDisabledPrefabName", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = HastePalette.Current.HighlightedDisabledPrefabColor });
      AddStyle(new Style() { name = "HighlightedDisabledBrokenPrefabName", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = HastePalette.Current.HighlightedDisabledBrokenPrefabColor });

      AddStyle(new Style() { name = "Description", other = EditorStyles.label, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 12, richText = true, textColor = HastePalette.Current.SecondaryColor });
      AddStyle(new Style() { name = "DisabledDescription", other = EditorStyles.label, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 12, richText = true, textColor = HastePalette.Current.DisabledColor });
      AddStyle(new Style() { name = "HighlightedDescription", other = EditorStyles.label, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 12, richText = true, textColor = HastePalette.Current.HighlightedSecondaryColor });
      AddStyle(new Style() { name = "HighlightedDisabledDescription", other = EditorStyles.label, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 12, richText = true, textColor = HastePalette.Current.HighlightedDisabledColor });

      #if DEBUG
      AddStyle(new Style() { name = "Score", other = EditorStyles.label, alignment = TextAnchor.MiddleCenter, fontSize = 12, textColor = HastePalette.Current.SecondaryColor });
      AddStyle(new Style() { name = "HighlightedScore", other = EditorStyles.label, alignment = TextAnchor.MiddleCenter, fontSize = 12, textColor = HastePalette.Current.HighlightedSecondaryColor });
      #endif

      AddStyle(new Style() { name = "Dot", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleCenter, fontSize = 24, textColor = HastePalette.Current.DotColor });

      yield return Haste.Scheduler.Start(PreCacheDynamicFonts());
    }

    static void AddStyle(Style style) {
      if (string.IsNullOrEmpty(style.name)) {
        throw new ArgumentException("Style name is required.");
      }

      GUIStyle guiStyle;

      if (style.other != null) {
        guiStyle = new GUIStyle(style.other);
      } else {
        guiStyle = new GUIStyle();
      }

      guiStyle.name = style.name;

      if (style.fixedHeight != 0) {
        guiStyle.fixedHeight = style.fixedHeight;
      }
      if (style.fontSize != 0) {
        guiStyle.fontSize = style.fontSize;
      }
      if (style.font != null) {
        guiStyle.font = style.font;
      }
      if (style.backgroundColor != null) {
        guiStyle.normal.background = style.backgroundColor;
      }
      if (style.textColor != TRANSPARENT) {
        guiStyle.normal.textColor = style.textColor;
      }

      guiStyle.alignment = style.alignment;
      guiStyle.fontStyle = style.fontStyle;
      guiStyle.wordWrap = style.wordWrap;
      guiStyle.richText = style.richText;

      styles.Add(style.name, guiStyle);
    }

    static IEnumerator WaitUntilReady() {
      GUIStyle test = null;
      bool failing = false;
      while (failing || test == null) {
        try {
          test = EditorStyles.textField;
          failing = false;
        } catch {
          failing = true;
          test = null;
        }
        yield return null;
      }
    }

    private static readonly string glyphs = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_+=~`[]{}|\\:;\"'<>,.?/ ";
    static IEnumerator PreCacheDynamicFonts() {
      GUISkin inspectorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
      foreach (GUIStyle style in styles.Values) {
        Font font = style.font != null ? style.font : inspectorSkin.font;
        if (style.richText) {
          foreach (char glyph in glyphs) {
            font.RequestCharactersInTexture(glyph.ToString(), style.fontSize, style.fontStyle);
            font.RequestCharactersInTexture(glyph.ToString(), style.fontSize, FontStyle.Bold);
          }
          yield return null;
        }
      }
    }

    static Texture2D CreateColorSwatch(Color color) {
      Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
      texture.SetPixel(0, 0, color);
      texture.Apply();
      texture.hideFlags = HideFlags.HideAndDontSave;
      return texture;
    }

    struct Style {
      public GUIStyle other;
      public string name;
      public TextAnchor alignment;
      public int fixedHeight;
      public int fontSize;
      public Font font;
      public Texture2D backgroundColor;
      public Color textColor;
      public FontStyle fontStyle;
      public bool wordWrap;
      public bool richText;
    }
  }
}
