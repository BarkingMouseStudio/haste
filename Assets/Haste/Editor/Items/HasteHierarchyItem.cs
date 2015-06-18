using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Haste {

  [Serializable]
  public class HasteHierarchyItem : AbstractHasteItem {

    public HasteHierarchyItem(string path, int id, string source) : base(path, id, source) {}
    public HasteHierarchyItem(SerializationInfo info, StreamingContext context) : base(info, context) {}

    public override IHasteResult GetResult(float score, string queryLower) {
      return new HasteHierarchyResult(this, score, queryLower);
    }
  }
}
