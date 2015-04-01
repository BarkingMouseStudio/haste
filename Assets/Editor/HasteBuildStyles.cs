using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  internal static class HasteBuildStyles {

    readonly static string SKINS_PATH = "Assets/Haste/Editor/InternalResources/Skins";
    readonly static Color TRANSPARENT = new Color();

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

    class Builder {

      IList<GUIStyle> styles = new List<GUIStyle>();
      string name;

      public Builder(string name) {
        this.name = name;
      }

      public GUIStyle AddStyle(Style style) {
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

        styles.Add(guiStyle);
        return guiStyle;
      }

      public GUISkin Build() {
        var skin = ScriptableObject.CreateInstance<GUISkin>();
        skin.name = name;
        skin.customStyles = styles.ToArray();
        return skin;
      }
    }

    static void CreateSkin(string name, HastePalette palette) {
      var font = HasteResources.Load<Font>("Fonts/FiraSans-Regular.ttf");
      font.hideFlags = HideFlags.HideAndDontSave;

      var builder = new Builder(name);

      builder.AddStyle(new Style() { name = "Query", other = EditorStyles.textField, fixedHeight = 64, font = font, alignment = TextAnchor.MiddleLeft });

      builder.AddStyle(new Style() { name = "Intro", other = EditorStyles.largeLabel, fixedHeight = 64, fontSize = 32, alignment = TextAnchor.MiddleCenter });
      builder.AddStyle(new Style() { name = "Count", other = EditorStyles.largeLabel, fontSize = 14, alignment = TextAnchor.MiddleLeft });
      builder.AddStyle(new Style() { name = "Empty", other = EditorStyles.largeLabel, fixedHeight = 24, fontSize = 16, alignment = TextAnchor.MiddleCenter });
      builder.AddStyle(new Style() { name = "Tip", other = EditorStyles.label, alignment = TextAnchor.MiddleCenter, fontSize = 14, wordWrap = true, textColor = palette.SecondaryColor });
      builder.AddStyle(new Style() { name = "Usage", other = EditorStyles.label, wordWrap = true });
      builder.AddStyle(new Style() { name = "Upgrade", other = EditorStyles.label, alignment = TextAnchor.MiddleCenter, fontSize = 14, textColor = palette.SelectionColor });
      builder.AddStyle(new Style() { name = "Indexing", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleCenter, fontSize = 14, textColor = palette.SecondaryColor });

      builder.AddStyle(new Style() { name = "Name", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = palette.PrimaryColor });
      builder.AddStyle(new Style() { name = "DisabledName", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = palette.DisabledColor });
      builder.AddStyle(new Style() { name = "HighlightedName", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = palette.PrimaryHighlightedColor });
      builder.AddStyle(new Style() { name = "HighlightedDisabledName", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = palette.SecondaryHighlightedColor });

      builder.AddStyle(new Style() { name = "Prefab", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = palette.PrefabColor });
      builder.AddStyle(new Style() { name = "BrokenPrefab", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = palette.BrokenPrefabColor });
      builder.AddStyle(new Style() { name = "DisabledPrefab", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = palette.DisabledPrefabColor });
      builder.AddStyle(new Style() { name = "DisabledBrokenPrefab", other = EditorStyles.largeLabel, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 16, textColor = palette.DisabledBrokenPrefabColor });

      builder.AddStyle(new Style() { name = "Description", other = EditorStyles.label, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 12, richText = true, textColor = palette.SecondaryColor });
      builder.AddStyle(new Style() { name = "DisabledDescription", other = EditorStyles.label, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 12, richText = true, textColor = palette.DisabledColor });
      builder.AddStyle(new Style() { name = "HighlightedDescription", other = EditorStyles.label, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 12, richText = true, textColor = palette.SecondaryHighlightedColor });
      builder.AddStyle(new Style() { name = "HighlightedDisabledDescription", other = EditorStyles.label, alignment = TextAnchor.MiddleLeft, fixedHeight = 24, fontSize = 12, richText = true, textColor = palette.SecondaryHighlightedColor });

      var skin = builder.Build();
      AssetDatabase.CreateAsset(skin, String.Format("{0}/{1}.guiskin", SKINS_PATH, skin.name));
    }

    [MenuItem("Window/Haste/Create Skins")]
    public static void CreateSkins() {
      CreateSkin("Pro", HastePalette.GetDark());
      CreateSkin("Personal", HastePalette.GetLight());
    }
  }
}