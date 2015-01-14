using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public interface IHasteResult {
    HasteItem Item { get; }
    List<int> Indices { get; }
    float Score { get; }

    void Draw();
    bool Validate();
    void Action();
    void Select();
  }

  public abstract class AbstractHasteResult : IHasteResult {

    public HasteItem Item { get; protected set; }
    public List<int> Indices { get; protected set; }
    public float Score { get; protected set; }

    public AbstractHasteResult(HasteItem item, float score, List<int> indices) {
      Item = item;
      Score = score;
      Indices = indices;
    }

    public virtual bool Validate() {
      return true;
    }

    public virtual void Draw() {
      using (new HasteVertical()) {
        EditorGUILayout.LabelField(Path.GetFileName(Item.Path), HasteWindow.NameStyle);
        EditorGUILayout.LabelField(HasteUtils.BoldLabel(Item.Path, Indices.ToArray(), HasteWindow.BoldStart, HasteWindow.BoldEnd), HasteWindow.DescriptionStyle);
      }
    }

    public virtual void Action() {
      // NO-OP
    }

    public virtual void Select() {
      // By default, re-select the initial instance id
      HasteWindow.Instance.RestoreInitialSelection();
    }
  }

  public class HasteResult : AbstractHasteResult {
    public HasteResult(HasteItem item, float score, List<int> indices) : base(item, score, indices) {}
  }
}