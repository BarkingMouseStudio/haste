namespace Haste {

  public class HasteTips {

    public static string[] Tips = new string[]{
      "You can add your own commands to Haste using the MenuItem attribute.",
      "You can use fuzzy matching to search without typing the full name.",
      "You can access Haste's preferences in Unity's preferences window."
    };

    public static string Random {
      get {
        return Tips[HasteSettings.UsageCount % Tips.Length];
      }
    }
  }
}