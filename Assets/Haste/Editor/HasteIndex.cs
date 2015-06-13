using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public class HasteIndex {

    readonly IDictionary<char, HashSet<IHasteItem>> index =
      new Dictionary<char, HashSet<IHasteItem>>();

    // The number of unique items in the index
    public int Count { get; protected set; }

    // The total size of the index including each indexed reference
    public int Size { get; protected set; }

    public bool TryGetValue(char key, out HashSet<IHasteItem> bucket) {
      return index.TryGetValue(key, out bucket);
    }

    string GetIndexBoundaries(IHasteItem m) {
      var indexBoundaries = new StringBuilder(4);
      if (m.NameLower.Length > 0) {
        indexBoundaries.Append(m.NameLower[0]);
      }
      if (m.PathLower.Length > 0) {
        indexBoundaries.Append(m.PathLower[0]);
      }
      if (m.ExtensionLower.Length > 0) {
        indexBoundaries.Append('.');
        indexBoundaries.Append(m.ExtensionLower[0]);
      }
      return indexBoundaries.ToString();
    }

    public void Add(IHasteItem item) {
      Count++;

      foreach (char c in GetIndexBoundaries(item)) {
        if (!index.ContainsKey(c)) {
          index.Add(c, new HashSet<IHasteItem>());
        }

        index[c].Add(item);
        Size++;
      }
    }

    public void Remove(IHasteItem item) {
      Count--;

      foreach (char c in GetIndexBoundaries(item)) {
        if (index.ContainsKey(c)) {
          index[c].Remove(item);
          Size--;
        }
      }
    }

    public void Clear() {
      index.Clear();
      Count = 0;
      Size = 0;
    }
  }
}
