using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteHierarchyItem : AbstractHasteItem {

    public HasteHierarchyItem(string path, int id, string source) : base(path, id, source) {}

    public override IHasteResult GetResult(float score, string queryLower) {
      return new HasteHierarchyResult(this, score, queryLower);
    }
  }
}
