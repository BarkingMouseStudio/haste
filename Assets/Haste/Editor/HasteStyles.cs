using UnityEngine;
using UnityEditor;

namespace Haste {

  public static class HasteStyles {

    public static readonly int WindowWidth = 500;
    public static readonly int WindowHeight = 300;
    public static readonly int ItemHeight = 46;

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

    public static readonly string BoldEnd = "</b></color>";

    private static Font queryStyleFont;
    public static Font QueryStyleFont {
      get {
        if (queryStyleFont == null) {
          queryStyleFont = HasteResources.Load<Font>("Fonts/FiraSans-Regular.ttf");
          queryStyleFont.hideFlags = HideFlags.HideAndDontSave;
        }
        return queryStyleFont;
      }
    }

    private static GUIStyle queryStyle;
    public static GUIStyle QueryStyle {
      get {
        if (queryStyle == null) {
          queryStyle = new GUIStyle(EditorStyles.textField);
          queryStyle.font = QueryStyleFont;
          queryStyle.fixedHeight = 64;
          queryStyle.alignment = TextAnchor.MiddleLeft;
        }
        return queryStyle;
      }
    }

    public static GUIStyle IntroStyle;
    public static GUIStyle IndexingStyle;
    public static GUIStyle TipStyle;
    public static GUIStyle EmptyStyle;
    public static GUIStyle UpgradeStyle;

    public static GUIStyle NameStyle;
    public static GUIStyle DisabledNameStyle;
    public static GUIStyle HighlightedNameStyle;

    public static GUIStyle PrefabStyle;
    public static GUIStyle BrokenPrefabStyle;

    public static GUIStyle PrefixStyle;
    public static GUIStyle DisabledPrefixStyle;

    public static GUIStyle DescriptionStyle;
    public static GUIStyle DisabledDescriptionStyle;
    public static GUIStyle HighlightedDescriptionStyle;

    private static GUIStyle highlightStyle;
    public static GUIStyle HighlightStyle {
      get {
        if (highlightStyle == null) {
          highlightStyle = new GUIStyle();
          highlightStyle.normal.background = HasteUtils.CreateColorSwatch(HasteColors.HighlightColor);
          highlightStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;
        }
        return highlightStyle;
      }
    }

    // We use a separate non-highlight style since GUIStyle.none has some extra padding...
    private static GUIStyle nonHighlightStyle;
    public static GUIStyle NonHighlightStyle {
      get {
        if (nonHighlightStyle == null) {
          nonHighlightStyle = new GUIStyle();
        }
        return nonHighlightStyle;
      }
    }

    static HasteStyles() {
      IntroStyle = new GUIStyle(EditorStyles.largeLabel);
      IntroStyle.fixedHeight = 64;
      IntroStyle.alignment = TextAnchor.MiddleCenter;
      IntroStyle.fontSize = 32;

      IndexingStyle = new GUIStyle(EditorStyles.largeLabel);
      IndexingStyle.alignment = TextAnchor.MiddleCenter;
      IndexingStyle.fontSize = 14;
      IndexingStyle.normal.textColor = HasteColors.SecondaryColor;

      TipStyle = new GUIStyle(EditorStyles.label);
      TipStyle.alignment = TextAnchor.MiddleCenter;
      TipStyle.fontSize = 14;
      TipStyle.normal.textColor = HasteColors.SecondaryColor;
      TipStyle.wordWrap = true;

      PrefabStyle = new GUIStyle(EditorStyles.largeLabel);
      PrefabStyle.alignment = TextAnchor.MiddleLeft;
      PrefabStyle.fixedHeight = 24;
      PrefabStyle.fontSize = 16;
      PrefabStyle.normal.textColor = HasteColors.PrefabColor;

      BrokenPrefabStyle = new GUIStyle(EditorStyles.largeLabel);
      BrokenPrefabStyle.alignment = TextAnchor.MiddleLeft;
      BrokenPrefabStyle.fixedHeight = 24;
      BrokenPrefabStyle.fontSize = 16;
      BrokenPrefabStyle.normal.textColor = HasteColors.BrokenPrefabColor;

      EmptyStyle = new GUIStyle(EditorStyles.largeLabel);
      EmptyStyle.alignment = TextAnchor.MiddleCenter;
      EmptyStyle.fixedHeight = 24;
      EmptyStyle.fontSize = 16;

      UpgradeStyle = new GUIStyle(EditorStyles.label);
      UpgradeStyle.alignment = TextAnchor.MiddleCenter;
      UpgradeStyle.fontSize = 14;
      UpgradeStyle.normal.textColor = HasteColors.LinkColor;

      NameStyle = new GUIStyle(EditorStyles.largeLabel);
      NameStyle.alignment = TextAnchor.MiddleLeft;
      NameStyle.fixedHeight = 24;
      NameStyle.fontSize = 16;
      NameStyle.normal.textColor = HasteColors.PrimaryColor;

      HighlightedNameStyle = new GUIStyle(EditorStyles.largeLabel);
      HighlightedNameStyle.alignment = TextAnchor.MiddleLeft;
      HighlightedNameStyle.fixedHeight = 24;
      HighlightedNameStyle.fontSize = 16;
      HighlightedNameStyle.normal.textColor = HasteColors.PrimaryHighlightedColor;

      DisabledNameStyle = new GUIStyle(EditorStyles.largeLabel);
      DisabledNameStyle.alignment = TextAnchor.MiddleLeft;
      DisabledNameStyle.fixedHeight = 24;
      DisabledNameStyle.fontSize = 16;
      DisabledNameStyle.normal.textColor = HasteColors.DisabledColor;

      DisabledDescriptionStyle = new GUIStyle(EditorStyles.label);
      DisabledDescriptionStyle.alignment = TextAnchor.MiddleLeft;
      DisabledDescriptionStyle.fixedHeight = 24;
      DisabledDescriptionStyle.fontSize = 12;
      DisabledDescriptionStyle.normal.textColor = HasteColors.DisabledColor;
      DisabledDescriptionStyle.richText = true;

      DescriptionStyle = new GUIStyle(EditorStyles.label);
      DescriptionStyle.alignment = TextAnchor.MiddleLeft;
      DescriptionStyle.fixedHeight = 24;
      DescriptionStyle.fontSize = 12;
      DescriptionStyle.richText = true;
      DescriptionStyle.normal.textColor = HasteColors.SecondaryColor;

      HighlightedDescriptionStyle = new GUIStyle(EditorStyles.label);
      HighlightedDescriptionStyle.alignment = TextAnchor.MiddleLeft;
      HighlightedDescriptionStyle.fixedHeight = 24;
      HighlightedDescriptionStyle.fontSize = 12;
      HighlightedDescriptionStyle.richText = true;
      HighlightedDescriptionStyle.normal.textColor = HasteColors.SecondaryHighlightedColor;
    }
  }
}
