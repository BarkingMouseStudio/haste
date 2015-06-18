using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  #if IS_HASTE_PRO
  public class HasteRecommendations {

    const float THRESHOLD = 0.1f;
    const float DECAY = 0.9f;

    List<IHasteItem> recent;

    public HasteRecommendations() {
      recent = new List<IHasteItem>();
    }

    public IHasteResult[] Get() {
      return recent.OrderByDescending(item => {
        return item.UserScore;
      }).Select(item => {
        return item.GetResult(item.UserScore, "");
      }).ToArray();
    }

    public void Add(IHasteItem newItem) {
      var isContained = recent.Contains(newItem);
      if (isContained && newItem.UserScore == 1.0f) {
        return; // Do nothing if we just selected this item
      }

      // Decay recent
      var dead = new List<IHasteItem>();
      foreach (var item in recent) {
        item.UserScore *= DECAY;
        if (item.UserScore < THRESHOLD) {
          dead.Add(item);
        }
      }

      // Remove dead recent
      recent.RemoveAll((item) => dead.Contains(item));

      if (!isContained) {
        // Add new item
        recent.Add(newItem);
      }

      // Set item score
      newItem.UserScore = 1.0f;
    }
  }
  #endif
}
