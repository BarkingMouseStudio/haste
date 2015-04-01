using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  // Unique object to exist in Haste's index.
  // Contains everything necessary to index
  // the item and filter them. Features required
  // during sorting are omitted.
  public interface IHasteItem : IEquatable<IHasteItem> {

    string Path { get; }
    string PathLower { get; }
    int Id { get; }
    string Source { get; }
    int Bitset { get; }
    string BoundariesLower { get; }

    IHasteResult GetResult(string queryLower, int queryLen);
    bool Equals(object obj);
    int GetHashCode();
  }
}
