using UnityEngine;
using UnityEditor;

namespace Haste {

  public static class HasteStyles {

    public static readonly int WindowWidth = 500;
    public static readonly int WindowHeight = 300;
    public static readonly int ItemHeight = 46;

    public static GUIStyle IntroStyle;
    public static GUIStyle UsageStyle;
    public static GUIStyle IndexingStyle;
    public static GUIStyle CountStyle;
    public static GUIStyle TipStyle;
    public static GUIStyle EmptyStyle;
    public static GUIStyle UpgradeStyle;

    public static GUIStyle NameStyle;
    public static GUIStyle DisabledNameStyle;
    public static GUIStyle HighlightedNameStyle;
    public static GUIStyle HighlightedDisabledNameStyle;

    public static GUIStyle PrefabStyle;
    public static GUIStyle BrokenPrefabStyle;
    public static GUIStyle DisabledPrefabStyle;
    public static GUIStyle DisabledBrokenPrefabStyle;

    public static GUIStyle PrefixStyle;
    public static GUIStyle DisabledPrefixStyle;

    public static GUIStyle DescriptionStyle;
    public static GUIStyle DisabledDescriptionStyle;
    public static GUIStyle HighlightedDescriptionStyle;
    public static GUIStyle HighlightedDisabledDescriptionStyle;

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

    private static GUIStyle selectionStyle;
    public static GUIStyle SelectionStyle {
      get {
        if (selectionStyle == null) {
          selectionStyle = new GUIStyle();
          selectionStyle.normal.background = HasteUtils.CreateColorSwatch(HasteColors.SelectionColor);
          selectionStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;
        }
        return selectionStyle;
      }
    }

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

      CountStyle = new GUIStyle(EditorStyles.largeLabel);
      CountStyle.alignment = TextAnchor.MiddleRight;
      CountStyle.fontSize = 14;

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

      DisabledPrefabStyle = new GUIStyle(EditorStyles.largeLabel);
      DisabledPrefabStyle.alignment = TextAnchor.MiddleLeft;
      DisabledPrefabStyle.fixedHeight = 24;
      DisabledPrefabStyle.fontSize = 16;
      DisabledPrefabStyle.normal.textColor = HasteColors.DisabledPrefabColor;

      BrokenPrefabStyle = new GUIStyle(EditorStyles.largeLabel);
      BrokenPrefabStyle.alignment = TextAnchor.MiddleLeft;
      BrokenPrefabStyle.fixedHeight = 24;
      BrokenPrefabStyle.fontSize = 16;
      BrokenPrefabStyle.normal.textColor = HasteColors.BrokenPrefabColor;

      DisabledBrokenPrefabStyle = new GUIStyle(EditorStyles.largeLabel);
      DisabledBrokenPrefabStyle.alignment = TextAnchor.MiddleLeft;
      DisabledBrokenPrefabStyle.fixedHeight = 24;
      DisabledBrokenPrefabStyle.fontSize = 16;
      DisabledBrokenPrefabStyle.normal.textColor = HasteColors.DisabledBrokenPrefabColor;

      EmptyStyle = new GUIStyle(EditorStyles.largeLabel);
      EmptyStyle.alignment = TextAnchor.MiddleCenter;
      EmptyStyle.fixedHeight = 24;
      EmptyStyle.fontSize = 16;

      UsageStyle = new GUIStyle(EditorStyles.label);
      UsageStyle.wordWrap = true;

      UpgradeStyle = new GUIStyle(EditorStyles.label);
      UpgradeStyle.alignment = TextAnchor.MiddleCenter;
      UpgradeStyle.fontSize = 14;
      UpgradeStyle.fontStyle = FontStyle.Bold;
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

      HighlightedDisabledNameStyle = new GUIStyle(EditorStyles.largeLabel);
      HighlightedDisabledNameStyle.alignment = TextAnchor.MiddleLeft;
      HighlightedDisabledNameStyle.fixedHeight = 24;
      HighlightedDisabledNameStyle.fontSize = 16;
      HighlightedDisabledNameStyle.normal.textColor = HasteColors.SecondaryHighlightedColor;

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

      HighlightedDisabledDescriptionStyle = new GUIStyle(EditorStyles.label);
      HighlightedDisabledDescriptionStyle.alignment = TextAnchor.MiddleLeft;
      HighlightedDisabledDescriptionStyle.fixedHeight = 24;
      HighlightedDisabledDescriptionStyle.fontSize = 12;
      HighlightedDisabledDescriptionStyle.richText = true;
      HighlightedDisabledDescriptionStyle.normal.textColor = HasteColors.SecondaryHighlightedColor;
    }
  }
}
