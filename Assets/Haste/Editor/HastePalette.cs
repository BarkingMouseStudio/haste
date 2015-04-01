using UnityEngine;
using UnityEditor;

namespace Haste {

  public class HastePalette {
    public Color PrimaryColor;
    public Color SecondaryColor;
    public Color DisabledColor;

    public Color SelectionColor;
    public Color HighlightColor;
    public Color PrimaryHighlightedColor;
    public Color SecondaryHighlightedColor;

    public Color PrefabColor;
    public Color BrokenPrefabColor;
    public Color DisabledPrefabColor;
    public Color DisabledBrokenPrefabColor;

    public Color HighlightedPrefabColor;
    public Color HighlightedBrokenPrefabColor;
    public Color HighlightedDisabledPrefabColor;
    public Color HighlightedDisabledBrokenPrefabColor;

    private static readonly float lerpFactor = 0.5f;

    private static HastePalette current;
    public static HastePalette Current {
      get {
        if (current == null) {
          current = EditorGUIUtility.isProSkin ? GetDark() : GetLight();
        }
        return current;
      }
    }

    static Color WithAlpha(Color c, float alpha) {
      return new Color(c.r, c.g, c.b, alpha);
    }

    public static HastePalette GetLight() {
      var prefabColor = new Color(0.02f, 0.17f, 0.52f);
      var brokenPrefabColor = new Color(0.27f, 0.07f, 0.07f);
      var disabledPrefabColor = WithAlpha(prefabColor, 0.5f);
      var disabledBrokenPrefabColor = WithAlpha(brokenPrefabColor, 0.5f);
      return new HastePalette() {
        PrimaryColor = new Color(18f / 255f, 18f / 255f, 18f / 255f),
        SecondaryColor = new Color(0.0f, 0.0f, 0.0f, 0.7f),
        DisabledColor = new Color(0.3f, 0.3f, 0.3f, 0.5f),

        PrefabColor = prefabColor,
        BrokenPrefabColor = brokenPrefabColor,
        DisabledPrefabColor = disabledPrefabColor,
        DisabledBrokenPrefabColor = disabledBrokenPrefabColor,

        HighlightedPrefabColor = Color.Lerp(prefabColor, Color.white, lerpFactor),
        HighlightedBrokenPrefabColor = Color.Lerp(brokenPrefabColor, Color.white, lerpFactor),
        HighlightedDisabledPrefabColor = Color.Lerp(disabledPrefabColor, Color.white, lerpFactor),
        HighlightedDisabledBrokenPrefabColor = Color.Lerp(disabledBrokenPrefabColor, Color.white, lerpFactor),

        SelectionColor = new Color(143f / 255f, 143f / 255f, 143f / 255f),
        HighlightColor = new Color(62f / 255f, 125f / 255f, 231f / 255f),
        PrimaryHighlightedColor = new Color(250f / 255f, 251f / 255f, 254f / 255f),
        SecondaryHighlightedColor = new Color(1.0f, 1.0f, 1.0f, 0.5f),
      };
    }

    public static HastePalette GetDark() {
      var prefabColor = new Color(0.3f, 0.5f, 0.835f);
      var brokenPrefabColor = new Color(0.7f, 0.4f, 0.4f);
      var disabledPrefabColor = WithAlpha(prefabColor, 0.5f);
      var disabledBrokenPrefabColor = WithAlpha(brokenPrefabColor, 0.5f);
      return new HastePalette() {
        PrimaryColor = new Color(0.705f, 0.705f, 0.705f),
        SecondaryColor = new Color(0.5f, 0.5f, 0.5f),
        DisabledColor = new Color(0.4f, 0.4f, 0.4f, 0.5f),

        PrefabColor = prefabColor,
        BrokenPrefabColor = brokenPrefabColor,
        DisabledPrefabColor = disabledPrefabColor,
        DisabledBrokenPrefabColor = disabledBrokenPrefabColor,

        HighlightedPrefabColor = Color.Lerp(prefabColor, Color.white, lerpFactor),
        HighlightedBrokenPrefabColor = Color.Lerp(brokenPrefabColor, Color.white, lerpFactor),
        HighlightedDisabledPrefabColor = Color.Lerp(disabledPrefabColor, Color.white, lerpFactor),
        HighlightedDisabledBrokenPrefabColor = Color.Lerp(disabledBrokenPrefabColor, Color.white, lerpFactor),

        SelectionColor = new Color(72f / 255f, 72f / 255f, 72f / 255f),
        HighlightColor = new Color(0.24f, 0.37f, 0.59f),
        PrimaryHighlightedColor = new Color(0.91f, 0.91f, 0.91f),
        SecondaryHighlightedColor = new Color(1.0f, 1.0f, 1.0f, 0.7f),
      };
    }
  }
}
