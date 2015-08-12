using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Haste {

  #if IS_HASTE_PRO
  public class HasteRecommendations : ScriptableObject {

    const float THRESHOLD = 0.1f;
    const float DECAY = 0.9f;

    [SerializeField]
    List<HasteItem> recent = new List<HasteItem>();

    public static string RecommendationsPath {
      get {
        return HasteResources.InternalResourcesPath + "Recommendations.asset";
      }
    }

    public static HasteRecommendations Load() {
      if (File.Exists(RecommendationsPath)) {
        return (HasteRecommendations)AssetDatabase.LoadAssetAtPath(RecommendationsPath, typeof(HasteRecommendations));
      } else {
        var recommendations = ScriptableObject.CreateInstance<HasteRecommendations>();
        AssetDatabase.CreateAsset(recommendations, RecommendationsPath);
        return recommendations;
      }
    }

    public IHasteResult[] Get() {
      return recent.OrderByDescending(item => item.userScore)
        .Select(item => item.GetResult(item.userScore, ""))
        .Where(result => {
          if (result.Item.source == HasteHierarchySource.NAME ||
              result.Item.source == HasteProjectSource.NAME) {
            return result.Object != null;
          } else {
            return true;
          }
        })
        .ToArray();
    }

    public void Add(HasteItem newItem) {
      var index = recent.IndexOf(newItem);
      if (index != -1 && newItem.userScore == 1.0f) {
        return; // Do nothing if we just selected this item
      }

      // Decay recent
      var dead = new List<HasteItem>();
      foreach (var item in recent) {
        item.userScore *= DECAY;

        if (item.userScore < THRESHOLD) {
          dead.Add(item);
        }
      }

      // Remove dead recent
      recent.RemoveAll((item) => dead.Contains(item));

      if (index != -1) {
        recent[index] = newItem; // Replace original instance
      } else {
        recent.Add(newItem); // Add new item
      }

      // Set item score
      newItem.userScore = 1.0f;

      EditorUtility.SetDirty(this);
    }
  }
  #endif
}
