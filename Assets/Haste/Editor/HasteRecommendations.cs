using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  #if IS_HASTE_PRO
  public static class HasteRecommendations {

    // Intelligent "recent":
    // - Each time an item is selected, at its uid to a List<Tuple<int, float>> with a score of 1.
    // - If the item already exists, increment by 1
    // - Each time Haste is opened, decay the scores of all items by some amount.
    // - Limit list to 100 MRU or so.
    // Needs persistence

    static readonly float THRESHOLD = 0.1f;
    static readonly float DECAY = 0.9f;

    static List<IHasteItem> items;
    static Dictionary<int, float> scores;

    static HasteRecommendations() {
      items = new List<IHasteItem>();
      scores = new Dictionary<int, float>();
    }

    public static float GetScore(IHasteItem item) {
      float score;
      if (scores.TryGetValue(item.GetHashCode(), out score)) {
        return score;
      } else {
        return 0.0f;
      }
    }

    public static IHasteResult[] Get() {
      return items.OrderByDescending(item => {
        return scores[item.GetHashCode()];
      }).Select(item => {
        return item.GetResult(scores[item.GetHashCode()], "");
      }).ToArray();
    }

    public static void Update(IHasteResult result) {
      // Get original score
      var item = result.Item;
      var hashCode = item.GetHashCode();

      float score;
      if (scores.TryGetValue(hashCode, out score) && score == 1.0f) {
        return; // Do nothing
      }

      var keys = scores.Keys.ToList();
      var dead = new List<int>();

      // Decay items and accumulate dead items
      foreach (var key in keys) {
        scores[key] *= DECAY;
        if (scores[key] < THRESHOLD) {
          dead.Add(key);
        }
      }

      // Remove dead items
      foreach (var key in dead) {
        scores.Remove(key);
      }

      items.RemoveAll((it) => {
        return dead.Contains(it.GetHashCode());
      });

      // Increment item scores
      if (!scores.ContainsKey(hashCode)) {
        items.Add(item);
      }
      scores[hashCode] = 1.0f;
    }
  }
  #endif
}
