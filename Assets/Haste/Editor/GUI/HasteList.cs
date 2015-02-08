using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteList : ScriptableObject {

    Vector2 scrollPosition = Vector2.zero;

    private int highlightedIndex = 0;
    public int HighlightedIndex {
      get { return highlightedIndex; }
      set { highlightedIndex = Mathf.Clamp(value, 0, Items.Length - 1); }
    }

    private IHasteResult[] items = new IHasteResult[0];
    public IHasteResult[] Items {
      get {
        return items;
      }
      set {
        items = value;
        HighlightedIndex = 0;
        ScrollTo(0);
      }
    }

    public int Size {
      get { return Items != null ? Items.Length : 0; }
    }

    public IHasteResult HighlightedItem {
      get {
        if (HighlightedIndex >= 0 && HighlightedIndex < Items.Length) {
          return Items[HighlightedIndex];
        } else {
          return null;
        }
      }
    }

    void OnEnable() {
      base.hideFlags = HideFlags.HideAndDontSave;
    }

    void ResetScroll() {
      scrollPosition = Vector2.zero;
    }

    void ScrollTo(int toIndex) {
      if (toIndex >= 0 && toIndex < Items.Length) {
        var heightOffset = 0.0f;
        for (var i = 0; i < toIndex; i++) {
          if (Items[i] != null) {
            heightOffset += Items[i].Height(i == HighlightedIndex);
          }
        }
        scrollPosition = new Vector2(scrollPosition.x, heightOffset);
      }
    }

    public void OnUpArrow() {
      if (Items != null) {
        HighlightedIndex = Mathf.Max(HighlightedIndex - 1, 0);
        ScrollTo(HighlightedIndex);
      }
    }

    public void OnDownArrow() {
      if (Items != null) {
        HighlightedIndex = Mathf.Min(HighlightedIndex + 1, Items.Length - 1);
        ScrollTo(HighlightedIndex);
      }
    }

    void OnItemDrag(Event e, int index) {
      var item = Items[index];
      if (item != null) {
        DragAndDrop.PrepareStartDrag();
        DragAndDrop.objectReferences = new UnityEngine.Object[]{
          item.Object
        };
        DragAndDrop.StartDrag(item.DragLabel);
        e.Use();
      }
    }

    void OnItemMouseDown(Event e, int index) {
      // Highlight
      HighlightedIndex = index;
      HasteWindow.Instance.Repaint();
      HasteDebug.Info("OnItemMouseDown: {0}", index);
    }

    void OnItemClick(Event e, int index) {
      // Select
      HasteDebug.Info("OnItemClick: {0}", index);
    }

    void OnItemDoubleClick(Event e, int index) {
      // Action (OnReturn)
      HasteDebug.Info("OnItemDoubleClick: {0}", index);
    }

    public void OnGUI() {
      using (var scrollView = new HasteScrollView(scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true))) {
        scrollPosition = scrollView.ScrollPosition;

        bool isHighlighted;
        for (var i = 0; i < Items.Length; i++) {
          isHighlighted = i == HighlightedIndex;
          HasteListItem.Draw(Items[i], i, isHighlighted, this.OnItemMouseDown, this.OnItemClick, this.OnItemDoubleClick, this.OnItemDrag);
        }
      }
    }
  }
}
