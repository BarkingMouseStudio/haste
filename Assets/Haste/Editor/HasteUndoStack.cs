using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

namespace Haste {

  // Hack to name multiple collapsed undo operations
  public class HasteUndoStack : IDisposable {

    private static GameObject _TempGO;
    private static GameObject TempGO {
      get {
        if (_TempGO == null) {
          _TempGO = new GameObject("{GameObject}");
          _TempGO.hideFlags = HideFlags.HideAndDontSave;
        }
        return _TempGO;
      }
    }

    public int GroupId { get; protected set; }

    public HasteUndoStack(string label) {
      GroupId = Undo.GetCurrentGroup();
      Undo.IncrementCurrentGroup();

      Undo.RecordObject(TempGO, label);
      TempGO.transform.localPosition += Vector3.one;
    }

    public void Dispose() {
      Undo.CollapseUndoOperations(GroupId);
    }
  }
}