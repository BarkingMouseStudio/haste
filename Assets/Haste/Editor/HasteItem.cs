using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  // Unique object to exist in Haste's index.
  // Contains everything necessary to index
  // the item and filter them.
  public class HasteItem : IEquatable<HasteItem> {

    public string Path { get; private set; }
    public string PathLower { get; private set; }
    public int Id { get; private set; }
    public string Source { get; private set; }
    public int Bitset { get; private set; }
    public string Boundaries { get; private set; }

    public HasteItem(string path, int id, string source) {
      Path = path;
      Id = id;
      Source = source;
      Boundaries = Path.GetBoundaries();
      PathLower = path.ToLower();
      Bitset = HasteStringUtils.LetterBitsetFromString(PathLower);
    }

    public bool Equals(HasteItem other) {
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
      return Source.GetHashCode() ^ Id.GetHashCode() ^ Path.GetHashCode();
    }
  }
}