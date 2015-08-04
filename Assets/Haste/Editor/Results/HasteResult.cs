using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteResult : AbstractHasteResult {

    public HasteResult(HasteItem item, float score, string queryLower) : base(item, score, queryLower) {}
  }
}
