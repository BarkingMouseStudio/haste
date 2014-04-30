using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public class HasteProjectWatcher : HasteFileWatcher {

    public HasteProjectWatcher() : base(Application.dataPath) {
      EditorApplication.projectWindowChanged += ProjectWindowChanged;
    }

    void ProjectWindowChanged() {
      ResetAndRestart();
    }
  }
}