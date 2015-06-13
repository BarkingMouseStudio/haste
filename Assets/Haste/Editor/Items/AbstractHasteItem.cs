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
    public string Name { get; private set; }
    public string NameLower { get; private set; }
    public string ExtensionLower { get; private set; }
    public string BoundariesLower { get; private set; }

    protected AbstractHasteItem(string path, int id, string source) {
      Id = id;
      Source = source;

      Path = path;
      PathLower = path.ToLowerInvariant();

      BoundariesLower = HasteStringUtils.GetBoundaries(Path);

      Bitset = HasteStringUtils.LetterBitsetFromString(PathLower);

      Name = HasteStringUtils.GetFileNameWithoutExtension(Path);
      NameLower = Name.ToLowerInvariant();

      ExtensionLower = HasteStringUtils.GetExtension(Path).ToLowerInvariant();
    }

    public virtual IHasteResult GetResult(float score, string queryLower) {
      return new HasteResult(this, score, queryLower);
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
