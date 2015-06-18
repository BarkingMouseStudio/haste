using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Haste {

  [Serializable]
  public class HasteItem : AbstractHasteItem {

    public HasteItem(string path, int id, string source) : base(path, id, source) {}
    public HasteItem(SerializationInfo info, StreamingContext context) : base(info, context) {}

    public override IHasteResult GetResult(float score, string queryLower) {
      return new HasteResult(this, score, queryLower);
    }
  }
}
