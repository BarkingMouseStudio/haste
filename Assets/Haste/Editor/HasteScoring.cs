namespace Haste {

  public static class HasteScoring {

    public static float Score(IHasteResult r) {
      float score = 0.0f;

      #if !IS_HASTE_PRO
      // Force menu item matches to the bottom in free version
      if (r.Item.Source == HasteMenuItemSource.NAME) {
        return score;
      }
      #endif

      // Favor exact name matches
      if (r.IsExactNameMatch) {
        score += 4.0f;

      // Favor exact path matches
      } else if (r.IsExactMatch) {
        score += 50.0f;

      // Favor prefix name matches
      } else if (r.IsNamePrefixMatch) {
        score += 40.0f;

      // Favor prefix path matches
      } else if (r.IsPrefixMatch) {
        score += 30.0f;

      // Favor first char name matches
      } else if (r.IsFirstCharNameMatch) {
        score += 20.0f;

      // Favor first char path matches
      } else if (r.IsFirstCharMatch) {
        score += 10.0f;
      }

      // boundary matches : query length
      score += 100.0f * r.BoundaryQueryRatio;

      // boundary matches : boundaries length
      score += 100.0f * r.BoundaryUtilization;

      return score;
    }
  }
}
