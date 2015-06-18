using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  #if IS_HASTE_PRO
  public class HasteRecommendations {

    const float THRESHOLD = 0.1f;
    const float DECAY = 0.9f;

    List<IHasteItem> items;
    Dictionary<int, float> scores;

    public HasteRecommendations() {
      items = new List<IHasteItem>();
      scores = new Dictionary<int, float>();
    }

    public float GetScore(IHasteItem item) {
      float score;
      if (scores.TryGetValue(item.GetHashCode(), out score)) {
        return score;
      } else {
        return 0.0f;
      }
    }

    public IHasteResult[] Get() {
      return items.OrderByDescending(item => {
        return scores[item.GetHashCode()];
      }).Select(item => {
        return item.GetResult(scores[item.GetHashCode()], "");
      }).ToArray();
    }

    public void Update(IHasteResult result) {
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
