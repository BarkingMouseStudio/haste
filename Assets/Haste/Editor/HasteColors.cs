using UnityEngine;
using UnityEditor;

namespace Haste {

  public static class HasteColors {

    public static Color PrimaryColor;
    public static Color DisabledColor;
    public static Color SecondaryColor;
    public static Color LinkColor;
    public static Color PrefabColor;
    public static Color BrokenPrefabColor;
    public static Color HighlightColor;
    public static Color HighlightedColor;

    static void InitDarkColors() {
      PrimaryColor = new Color(250f / 255f, 251f / 255f, 252f / 255f);
      SecondaryColor = new Color(0.5f, 0.5f, 0.5f);
      DisabledColor = new Color(0.4f, 0.4f, 0.4f);
      PrefabColor = new Color(0.3f, 0.5f, 0.835f);
      BrokenPrefabColor = new Color(0.7f, 0.4f, 0.4f);
      HighlightColor = new Color(0.24f, 0.37f, 0.59f);
      HighlightedColor = PrimaryColor;
      LinkColor = HighlightColor;
    }

    static void InitLightColors() {
      PrimaryColor = new Color(250f / 255f, 251f / 255f, 254f / 255f);
      SecondaryColor = new Color(0.5f, 0.5f, 0.5f);
      DisabledColor = new Color(0.3f, 0.3f, 0.3f);
      PrefabColor = new Color(0.02f, 0.17f, 0.52f);
      BrokenPrefabColor = new Color(0.27f, 0.07f, 0.07f);
      HighlightColor = new Color(0.25f, 0.5f, 0.9f);
      HighlightedColor = PrimaryColor;
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
