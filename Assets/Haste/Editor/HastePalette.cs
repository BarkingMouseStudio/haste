using UnityEngine;
using UnityEditor;

namespace Haste {

  public class HastePalette {
    public Color PrimaryColor;
    public Color SecondaryColor;
    public Color DisabledColor;

    public Color HighlightedPrimaryColor;
    public Color HighlightedSecondaryColor;
    public Color HighlightedDisabledColor;

    public Color PrefabColor;
    public Color BrokenPrefabColor;
    public Color DisabledPrefabColor;
    public Color DisabledBrokenPrefabColor;

    public Color HighlightedPrefabColor;
    public Color HighlightedBrokenPrefabColor;
    public Color HighlightedDisabledPrefabColor;
    public Color HighlightedDisabledBrokenPrefabColor;

    public Color SelectionColor;
    public Color HighlightColor;
    public Color DotColor;

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
      var primaryColor = new Color(18f / 255f, 18f / 255f, 18f / 255f);
      var secondaryColor = new Color(0.3f, 0.3f, 0.3f);
      var disabledColor = WithAlpha(primaryColor, 0.4f);

      var highlightedPrimaryColor = Color.Lerp(primaryColor, Color.white, 0.9f);
      var highlightedSecondaryColor = Color.Lerp(secondaryColor, Color.white, 0.9f);
      var highlightedDisabledColor = Color.Lerp(disabledColor, Color.white, 0.8f);

      var prefabColor = new Color(0.02f, 0.17f, 0.52f);
      var brokenPrefabColor = new Color(0.27f, 0.07f, 0.07f);
      var disabledPrefabColor = WithAlpha(prefabColor, 0.4f);
      var disabledBrokenPrefabColor = WithAlpha(brokenPrefabColor, 0.4f);

      return new HastePalette() {
        PrimaryColor = primaryColor,
        SecondaryColor = secondaryColor,
        DisabledColor = disabledColor,

        HighlightedPrimaryColor = highlightedPrimaryColor,
        HighlightedSecondaryColor = highlightedSecondaryColor,
        HighlightedDisabledColor = highlightedDisabledColor,

        PrefabColor = prefabColor,
        BrokenPrefabColor = brokenPrefabColor,
        DisabledPrefabColor = disabledPrefabColor,
        DisabledBrokenPrefabColor = disabledBrokenPrefabColor,

        HighlightedPrefabColor = Color.Lerp(prefabColor, Color.white, 0.7f),
        HighlightedBrokenPrefabColor = Color.Lerp(brokenPrefabColor, Color.white, 0.7f),
        HighlightedDisabledPrefabColor = Color.Lerp(disabledPrefabColor, Color.white, 0.7f),
        HighlightedDisabledBrokenPrefabColor = Color.Lerp(disabledBrokenPrefabColor, Color.white, 0.7f),

        SelectionColor = new Color(143f / 255f, 143f / 255f, 143f / 255f),
        HighlightColor = new Color(62f / 255f, 125f / 255f, 231f / 255f),
        DotColor = new Color(1, 1, 1, 0.5f),
      };
    }

    public static HastePalette GetDark() {
      float lerpFactor = 0.5f;
      float disabledAlpha = 0.5f;

      var primaryColor = new Color(0.705f, 0.705f, 0.705f);
      var secondaryColor = new Color(0.5f, 0.5f, 0.5f);
      var disabledColor = WithAlpha(primaryColor, disabledAlpha);

      var highlightedPrimaryColor = Color.Lerp(primaryColor, Color.white, lerpFactor);
      var highlightedSecondaryColor = Color.Lerp(secondaryColor, Color.white, lerpFactor);
      var highlightedDisabledColor = Color.Lerp(disabledColor, Color.white, lerpFactor);

      var prefabColor = new Color(0.3f, 0.5f, 0.835f);
      var brokenPrefabColor = new Color(0.7f, 0.4f, 0.4f);
      var disabledPrefabColor = WithAlpha(prefabColor, disabledAlpha);
      var disabledBrokenPrefabColor = WithAlpha(brokenPrefabColor, disabledAlpha);

      return new HastePalette() {
        PrimaryColor = primaryColor,
        SecondaryColor = secondaryColor,
        DisabledColor = disabledColor,

        HighlightedPrimaryColor = highlightedPrimaryColor,
        HighlightedSecondaryColor = highlightedSecondaryColor,
        HighlightedDisabledColor = highlightedDisabledColor,

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
        DotColor = new Color(1, 1, 1, 0.5f),
      };
    }
  }
}
