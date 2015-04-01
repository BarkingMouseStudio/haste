using UnityEngine;
using UnityEditor;

namespace Haste {

  public class HastePalette {
    public Color PrimaryColor;
    public Color DisabledColor;
    public Color SecondaryColor;
    public Color PrefabColor;
    public Color BrokenPrefabColor;
    public Color SelectionColor;
    public Color HighlightColor;
    public Color PrimaryHighlightedColor;
    public Color SecondaryHighlightedColor;
    public Color DisabledBrokenPrefabColor;
    public Color DisabledPrefabColor;

    private static HastePalette current;
    public static HastePalette Current {
      get {
        if (current == null) {
          current = EditorGUIUtility.isProSkin ? GetDark() : GetLight();
        }
        return current;
      }
    }

    public static HastePalette GetLight() {
      return new HastePalette() {
        PrimaryColor = new Color(18f / 255f, 18f / 255f, 18f / 255f),
        SecondaryColor = new Color(0.0f, 0.0f, 0.0f, 0.7f),
        DisabledColor = new Color(0.3f, 0.3f, 0.3f),
        PrefabColor = new Color(0.02f, 0.17f, 0.52f),
        DisabledPrefabColor = new Color(0.02f, 0.17f, 0.52f, 0.5f),
        BrokenPrefabColor = new Color(0.27f, 0.07f, 0.07f),
        DisabledBrokenPrefabColor = new Color(0.27f, 0.07f, 0.07f, 0.5f),
        SelectionColor = new Color(62f / 255f, 125f / 255f, 231f / 255f),
        HighlightColor = new Color(62f / 255f, 125f / 255f, 231f / 255f, 0.5f),
        PrimaryHighlightedColor = new Color(250f / 255f, 251f / 255f, 254f / 255f),
        SecondaryHighlightedColor = new Color(1.0f, 1.0f, 1.0f, 0.7f),
      };
    }

    public static HastePalette GetDark() {
      return new HastePalette() {
        PrimaryColor = new Color(0.705f, 0.705f, 0.705f),
        SecondaryColor = new Color(0.5f, 0.5f, 0.5f),
        DisabledColor = new Color(0.4f, 0.4f, 0.4f),
        PrefabColor = new Color(0.3f, 0.5f, 0.835f),
        DisabledPrefabColor = new Color(0.3f, 0.5f, 0.835f, 0.5f),
        BrokenPrefabColor = new Color(0.7f, 0.4f, 0.4f),
        DisabledBrokenPrefabColor = new Color(0.7f, 0.4f, 0.4f, 0.5f),
        SelectionColor = new Color(0.24f, 0.37f, 0.59f),
        HighlightColor = new Color(0.24f, 0.37f, 0.59f, 0.5f),
        PrimaryHighlightedColor = new Color(0.91f, 0.91f, 0.91f),
        SecondaryHighlightedColor = new Color(1.0f, 1.0f, 1.0f, 0.7f),
      };
    }
  }
}
