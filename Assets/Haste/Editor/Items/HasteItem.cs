using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteItem : AbstractHasteItem {

    public HasteItem(string path, int id, string source) : base(path, id, source) {}

    public override IHasteResult GetResult(string queryLower, int queryLen) {
      return new HasteResult(this, queryLower, queryLen);
    }
  }
}