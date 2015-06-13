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

    int Id { get; }
    string Source { get; }

    string Path { get; }
    string PathLower { get; }

    string BoundariesLower { get; }

    string Name { get; }
    string NameLower { get; }

    string ExtensionLower { get; }

    int Bitset { get; }

    IHasteResult GetResult(float score, string queryLower);
    bool Equals(object obj);
    int GetHashCode();
  }
}
