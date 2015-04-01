using UnityEngine;
using UnityEditor;

namespace Haste {

  public static class HasteStyles {

    public static readonly int WindowWidth = 500;
    public static readonly int WindowHeight = 300;
    public static readonly int ItemHeight = 46;

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

    private static GUISkin skin;
    public static GUISkin Skin {
      get {
        if (skin == null) {
          if (EditorGUIUtility.isProSkin) {
            skin = HasteResources.Load<GUISkin>("Skins/Pro.guiskin");
            skin.hideFlags = HideFlags.HideAndDontSave;
          } else {
            skin = HasteResources.Load<GUISkin>("Skins/Personal.guiskin");
            skin.hideFlags = HideFlags.HideAndDontSave;
          }
        }
        return skin;
      }
    }
  }
}
