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
  [Serializable]
  public class HasteRecommendations {

    const float THRESHOLD = 0.1f;
    const float DECAY = 0.9f;

    // [SerializeField]
    List<SerializableHasteItem> recent;

    public HasteRecommendations() {
      recent = new List<SerializableHasteItem>();
    }

    public static string RecommendationsPath {
      get {
        return HasteResources.InternalResourcesPath + "Recommendations";
      }
    }

    public static HasteRecommendations Load() {
      if (File.Exists(RecommendationsPath)) {
        var bf = new BinaryFormatter();
        var file = File.Open(RecommendationsPath, FileMode.Open);
        var rec = (HasteRecommendations)bf.Deserialize(file);
        file.Close();
        return rec;
      } else {
        var rec = new HasteRecommendations();
        rec.Save();
        return rec;
      }
    }

    public void Save() {
      // EditorUtility.SetDirty(targetPlayer);

      var bf = new BinaryFormatter();
      var file = File.Create(RecommendationsPath);
      bf.Serialize(file, this);
      file.Close();
    }

    public IHasteResult[] Get() {
      return recent.OrderByDescending(item => {
        return item.Item.UserScore;
      }).Select(item => {
        return item.Item.GetResult(item.Item.UserScore, "");
      }).ToArray();
    }

    public void Add(IHasteItem newItem) {
      var newSerializableItem = new SerializableHasteItem(newItem);
      var isContained = recent.Contains(newSerializableItem);
      if (isContained && newItem.UserScore == 1.0f) {
        return; // Do nothing if we just selected this item
      }

      // Decay recent
      var dead = new List<SerializableHasteItem>();
      foreach (var item in recent) {
        item.Item.UserScore *= DECAY;
        if (item.Item.UserScore < THRESHOLD) {
          dead.Add(item);
        }
      }

      // Remove dead recent
      recent.RemoveAll((item) => dead.Contains(item));

      // Add new item
      if (!isContained) {
        recent.Add(newSerializableItem);
      }

      // Set item score
      newItem.UserScore = 1.0f;

      Save();
    }
  }
  #endif
}
