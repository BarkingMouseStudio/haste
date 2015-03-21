using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteResult : AbstractHasteResult {

    public HasteResult(HasteItem item, string query, int queryLen) : base(item, query, queryLen) {}
  }
}