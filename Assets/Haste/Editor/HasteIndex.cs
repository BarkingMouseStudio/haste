using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public class HasteIndex {

    readonly IDictionary<char, HashSet<HasteItem>> index =
      new Dictionary<char, HashSet<HasteItem>>();

    // The number of unique items in the index
    public int Count { get; protected set; }

    // The total size of the index including each indexed reference
    public int Size { get; protected set; }

    public bool TryGetValue(char key, out HashSet<HasteItem> bucket) {
      return index.TryGetValue(key, out bucket);
    }

    public void Add(HasteItem item) {
      Count++;

      foreach (char c in item.boundariesLower) {
        if (!index.ContainsKey(c)) {
          index.Add(c, new HashSet<HasteItem>());
        }

        index[c].Add(item);
        Size++;
      }
    }

    public void Remove(HasteItem item) {
      Count--;

      foreach (char c in item.boundariesLower) {
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
