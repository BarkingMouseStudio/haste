namespace Haste {

  public class HasteTips {

    #if IS_HASTE_PRO
      public static string[] Tips = new string[]{
        "Add your own commands to Haste using the MenuItem attribute.",
        "Use fuzzy matching to search without typing the full name.",
        "Access Haste's preferences in Unity's preferences window.",
        "Try dragging an asset or object directly out of Haste.",
        "You can select multiple results in Haste using Cmd/Ctrl+Click."
      };
    #else
      public static string[] Tips = new string[]{
        "You can use fuzzy matching to search without typing the full name.",
        "You can access Haste's preferences in Unity's preferences window.",
        "Try dragging an asset or object directly out of Haste.",
        "You can select multiple results in Haste using Cmd/Ctrl+Click."
      };
    #endif

    public static string Random {
      get {
        return Tips[HasteSettings.UsageCount % Tips.Length];
      }
    }
  }
}