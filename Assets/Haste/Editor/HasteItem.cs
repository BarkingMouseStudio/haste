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
    public string BoundariesLower { get; private set; }

    // TODO: HasteItem is a little too large now, consider recaculating indices
    public HasteItem(string path, int id, string source) {
      Path = path;
      Id = id;
      Source = source;

      // TODO: This is slow and allocs (thread culture lookup)
      // TODO: Try String.ToLowerInvariant or String.ToLower(Haste.CultureInfo)
      PathLower = path.ToLower();
      Bitset = HasteStringUtils.LetterBitsetFromString(PathLower);

      // TODO: This allocs and is slow but is probably better of here
      // than at search.
      // TODO: Benchmark recalculating boundary indices during sort since we
      // have to iterate again anyway.
      // TODO: ToLower here is slow
      BoundariesLower = HasteStringUtils.GetBoundaries(Path).ToLower();
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
      int hash = (int)17;
      hash = hash * 23 ^ Id.GetHashCode();
      hash = hash * 23 ^ Path.GetHashCode();
      return hash;
    }
  }
}