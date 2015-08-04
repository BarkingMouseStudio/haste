using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Haste {

  [Serializable]
  public class HasteItem {

    public string path;
    public string pathLower;

    public string name;
    public string nameLower;

    public int id;
    public string source;

    public string boundariesLower;
    public int bitset;

    public string extensionLower;

    public float userScore;

    public HasteItem(string path, int id, string source) {
      this.path = path;
      this.pathLower = path.ToLowerInvariant();

      this.name = HasteStringUtils.GetFileNameWithoutExtension(path);
      this.nameLower = name.ToLowerInvariant();

      this.id = id;
      this.source = source;

      this.boundariesLower = HasteStringUtils.GetBoundaries(path);
      this.bitset = HasteStringUtils.LetterBitsetFromString(pathLower);

      this.extensionLower = HasteStringUtils.GetExtension(path).ToLowerInvariant();

      this.userScore = 0.0f;
    }

    public IHasteResult GetResult(float score, string queryLower) {
      switch (source) {
        case HasteHierarchySource.NAME:
          return new HasteHierarchyResult(this, score, queryLower);
        case HasteProjectSource.NAME:
          return new HasteProjectResult(this, score, queryLower);
        case HasteMenuItemSource.NAME:
          return new HasteMenuItemResult(this, score, queryLower);
        case HasteLayoutSource.NAME:
          return new HasteMenuItemResult(this, score, queryLower);
        default:
          return new HasteResult(this, score, queryLower);
      }
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
      hash = hash * 23 ^ id.GetHashCode();
      hash = hash * 23 ^ path.GetHashCode();
      return hash;
    }
  }
}
