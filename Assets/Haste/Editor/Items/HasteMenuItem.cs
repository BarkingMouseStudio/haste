using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Haste {

  [Serializable]
  public class HasteMenuItem : AbstractHasteItem {

    public HasteMenuItem(string path, int id, string source) : base(path, id, source) {}
    public HasteMenuItem(SerializationInfo info, StreamingContext context) : base(info, context) {}

    public override IHasteResult GetResult(float score, string queryLower) {
      return new HasteMenuItemResult(this, score, queryLower);
    }
  }
}
