using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class AbstractHasteItem : IHasteItem {

    public string Path { get; private set; }
    public string PathLower { get; private set; }
    public int Id { get; private set; }
    public string Source { get; private set; }
    public int Bitset { get; private set; }
    public string BoundariesLower { get; private set; }

    public AbstractHasteItem(string path, int id, string source) {
      Path = path;
      Id = id;
      Source = source;
      PathLower = path.ToLowerInvariant();
      Bitset = HasteStringUtils.LetterBitsetFromString(PathLower);
      BoundariesLower = HasteStringUtils.GetBoundaries(Path);
    }

    public virtual IHasteResult GetResult(string queryLower, int queryLen) {
      return new HasteResult(this, queryLower, queryLen);
    }

    public bool Equals(IHasteItem other) {
      if (other == null) {
        return false;
      }

      // Reference
      if (other == this) {
        return true;
      }

      return GetHashCode() == other.GetHashCode();
    }

    public override bool Equals(object obj) {
      return Equals(obj as HasteItem);
    }

    public override int GetHashCode() {
      int hash = (int)17;
      hash = hash * 23 ^ Id.GetHashCode();
      hash = hash * 23 ^ Path.GetHashCode();
      return hash;
    }
  }
}