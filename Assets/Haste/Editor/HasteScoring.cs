using System;

namespace Haste {

  public static class HasteScoring {

    public static float Score(IHasteItem item, string queryLower, int queryLen) {
      float score = 0.0f;

      #if !IS_HASTE_PRO
      // Force menu item matches to the bottom in free version
      if (item.Source == HasteMenuItemSource.NAME) {
        return score;
      }
      #endif

      var userScore = 1.0f + HasteRecommendations.GetScore(item);

      var boundaryMatchCount = HasteStringUtils.LongestCommonSubsequenceLength(queryLower, item.BoundariesLower);
      var boundaryQueryRatio = boundaryMatchCount / (float)queryLen;
      var boundaryLen = item.BoundariesLower.Length;
      var boundaryUtilization = boundaryLen > 0 ? boundaryMatchCount / (float)boundaryLen : 0.0f;

      score += 40.0f * boundaryQueryRatio;
      score += 40.0f * boundaryUtilization;

      // Favor exact name matches
      if (item.NameLower == queryLower) {
        score += 60.0f;
        return score * userScore;
      }

      // Favor exact path matches
      if (item.PathLower == queryLower) {
        score += 50.0f;
        return score * userScore;
      }

      // Favor prefix name matches
      if (queryLen >= 3 && item.NameLower.IndexOf(queryLower, StringComparison.InvariantCulture) == 0) {
        score += 40.0f;
        return score * userScore;
      }

      // Favor prefix path matches
      if (queryLen >= 3 && item.PathLower.IndexOf(queryLower, StringComparison.InvariantCulture) == 0) {
        score += 30.0f;
        return score * userScore;
      }

      // Favor first char name matches
      if (item.NameLower[0] == queryLower[0]) {
        score += 20.0f;
        return score * userScore;
      }

      // Favor first char path matches
      if (item.PathLower[0] == queryLower[0]) {
        score += 10.0f;
        return score * userScore;
      }

      return score * userScore;
    }
  }
}
