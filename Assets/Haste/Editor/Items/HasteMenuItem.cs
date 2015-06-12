using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteMenuItem : AbstractHasteItem {

    public HasteMenuItem(string path, int id, string source) : base(path, id, source) {}

    public override IHasteResult GetResult(float score, string queryLower) {
      return new HasteMenuItemResult(this, score, queryLower);
    }
  }
}
