using UnityEngine;
using UnityEditor;

namespace Haste {

  public static class HasteColors {

    public static Color BlurColor;

    public static Color PrimaryColor;
    public static Color DisabledColor;
    public static Color SecondaryColor;
    public static Color LinkColor;
    public static Color PrefabColor;
    public static Color BrokenPrefabColor;
    public static Color HighlightColor;
    public static Color PrimaryHighlightedColor;
    public static Color SecondaryHighlightedColor;
    public static Color DisabledBrokenPrefabColor;
    public static Color DisabledPrefabColor;

    static void InitDarkColors() {
      BlurColor = new Color(0.0f, 0.0f, 0.0f, 0.4f);
      PrimaryColor = new Color(0.705f, 0.705f, 0.705f);
      SecondaryColor = new Color(0.5f, 0.5f, 0.5f);
      DisabledColor = new Color(0.4f, 0.4f, 0.4f);
      PrefabColor = new Color(0.3f, 0.5f, 0.835f);
      DisabledPrefabColor = new Color(0.3f, 0.5f, 0.835f, 0.5f);
      BrokenPrefabColor = new Color(0.7f, 0.4f, 0.4f);
      DisabledBrokenPrefabColor = new Color(0.7f, 0.4f, 0.4f, 0.5f);
      HighlightColor = new Color(0.24f, 0.37f, 0.59f);
      PrimaryHighlightedColor = new Color(0.91f, 0.91f, 0.91f);
      SecondaryHighlightedColor = new Color(1.0f, 1.0f, 1.0f, 0.7f);
      LinkColor = HighlightColor;
    }

    static void InitLightColors() {
      BlurColor = new Color(1.0f, 1.0f, 1.0f, 0.6f);
      PrimaryColor = new Color(18f / 255f, 18f / 255f, 18f / 255f);
      SecondaryColor = new Color(0.0f, 0.0f, 0.0f, 0.7f);
      DisabledColor = new Color(0.3f, 0.3f, 0.3f);
      PrefabColor = new Color(0.02f, 0.17f, 0.52f);
      DisabledPrefabColor = new Color(0.02f, 0.17f, 0.52f, 0.5f);
      BrokenPrefabColor = new Color(0.27f, 0.07f, 0.07f);
      DisabledBrokenPrefabColor = new Color(0.27f, 0.07f, 0.07f, 0.5f);
      HighlightColor = new Color(62f / 255f, 125f / 255f, 231f / 255f);
      PrimaryHighlightedColor = new Color(250f / 255f, 251f / 255f, 254f / 255f);
      SecondaryHighlightedColor = new Color(1.0f, 1.0f, 1.0f, 0.7f);
      LinkColor = HighlightColor;
    }

    static HasteColors() {
      if (EditorGUIUtility.isProSkin) {
        InitDarkColors();
      } else {
        InitLightColors();
      }
    }
  }
}
