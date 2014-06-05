using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

namespace Haste {

  public class HasteUndoStack : IDisposable {

    private static GameObject s_TempGO;
    private static GameObject TempGO {
      get {
        if (s_TempGO == null) {
          s_TempGO = new GameObject("{GameObject}");
          s_TempGO.hideFlags = HideFlags.HideAndDontSave;
        }
        return s_TempGO;
      }
    }

    public int GroupId { get; protected set; }

    public HasteUndoStack(string label) {
      Undo.IncrementCurrentGroup();
      GroupId = Undo.GetCurrentGroup();

      Undo.RecordObject(TempGO, label);
      TempGO.transform.localPosition += Vector3.one;
    }

    public void Dispose() {
      Undo.CollapseUndoOperations(GroupId);
    }
  }
}