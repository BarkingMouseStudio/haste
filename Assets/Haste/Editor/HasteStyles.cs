using UnityEngine;
using UnityEditor;

namespace Haste {

  public static class HasteStyles {

    public static readonly string BoldStart = "<color=\"#ddd\"><b>";
    public static readonly string BoldEnd = "</b></color>";

    public static GUIStyle QueryStyle;
    public static GUIStyle NameStyle;
    public static GUIStyle DescriptionStyle;
    public static GUIStyle PrefixStyle;
    public static GUIStyle DisabledPrefixStyle;
    public static GUIStyle IntroStyle;
    public static GUIStyle IndexingStyle;
    public static GUIStyle TipStyle;
    public static GUIStyle HighlightStyle;
    public static GUIStyle NonHighlightStyle;
    public static GUIStyle EmptyStyle;
    public static GUIStyle UpgradeStyle;
    public static GUIStyle DisabledNameStyle;
    public static GUIStyle DisabledDescriptionStyle;

    static HasteStyles() {
      QueryStyle = new GUIStyle(EditorStyles.textField);
      QueryStyle.fixedHeight = 64;
      QueryStyle.alignment = TextAnchor.MiddleLeft;
      QueryStyle.fontSize = 32;

      IntroStyle = new GUIStyle(EditorStyles.largeLabel);
      IntroStyle.fixedHeight = 64;
      IntroStyle.alignment = TextAnchor.MiddleCenter;
      IntroStyle.fontSize = 32;

      IndexingStyle = new GUIStyle(EditorStyles.largeLabel);
      IndexingStyle.fixedHeight = 24;
      IndexingStyle.alignment = TextAnchor.MiddleCenter;
      IndexingStyle.fontSize = 16;
      IndexingStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f);

      TipStyle = new GUIStyle(EditorStyles.largeLabel);
      TipStyle.fixedHeight = 0;
      TipStyle.alignment = TextAnchor.LowerCenter;
      TipStyle.fontSize = 14;
      TipStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
      TipStyle.wordWrap = true;
      TipStyle.stretchHeight = true;

      NameStyle = new GUIStyle(EditorStyles.largeLabel);
      NameStyle.alignment = TextAnchor.MiddleLeft;
      NameStyle.fixedHeight = 24;
      NameStyle.fontSize = 16;

      EmptyStyle = new GUIStyle(EditorStyles.largeLabel);
      EmptyStyle.alignment = TextAnchor.MiddleCenter;
      EmptyStyle.fixedHeight = 24;
      EmptyStyle.fontSize = 16;

      UpgradeStyle = new GUIStyle(EditorStyles.largeLabel);
      UpgradeStyle.alignment = TextAnchor.MiddleCenter;
      UpgradeStyle.fixedHeight = 24;
      UpgradeStyle.fontSize = 16;
      UpgradeStyle.normal.textColor = new Color(0.2f, 0.30f, 0.82f);

      DisabledNameStyle = new GUIStyle(EditorStyles.largeLabel);
      DisabledNameStyle.alignment = TextAnchor.MiddleLeft;
      DisabledNameStyle.fixedHeight = 24;
      DisabledNameStyle.fontSize = 16;
      DisabledNameStyle.normal.textColor = new Color(0.4f, 0.4f, 0.4f);

      DisabledDescriptionStyle = new GUIStyle(EditorStyles.largeLabel);
      DisabledDescriptionStyle.alignment = TextAnchor.MiddleLeft;
      DisabledDescriptionStyle.fixedHeight = 24;
      DisabledDescriptionStyle.fontSize = 12;
      DisabledDescriptionStyle.normal.textColor = new Color(0.4f, 0.4f, 0.4f);
      DisabledDescriptionStyle.richText = true;

      DescriptionStyle = new GUIStyle(EditorStyles.largeLabel);
      DescriptionStyle.alignment = TextAnchor.MiddleLeft;
      DescriptionStyle.fixedHeight = 24;
      DescriptionStyle.fontSize = 12;
      DescriptionStyle.richText = true;

      PrefixStyle = new GUIStyle(EditorStyles.largeLabel);
      PrefixStyle.alignment = TextAnchor.MiddleRight;
      PrefixStyle.fixedHeight = 18;
      PrefixStyle.fontSize = 12;

      DisabledPrefixStyle = new GUIStyle(EditorStyles.largeLabel);
      DisabledPrefixStyle.alignment = TextAnchor.MiddleRight;
      DisabledPrefixStyle.fixedHeight = 18;
      DisabledPrefixStyle.fontSize = 12;
      DisabledPrefixStyle.normal.textColor = new Color(0.4f, 0.4f, 0.4f);

      // We use a separate non-highlight style since GUIStyle.none has some extra padding...
      NonHighlightStyle = new GUIStyle();

      HighlightStyle = new GUIStyle();
      if (EditorGUIUtility.isProSkin) {
        HighlightStyle.normal.background = HasteUtils.CreateColorSwatch(new Color(0.275f, 0.475f, 0.95f, 0.2f));
      } else {
        HighlightStyle.normal.background = HasteUtils.CreateColorSwatch(new Color(0.045f, 0.22f, 0.895f, 0.2f));
      }
    }
  }
}
