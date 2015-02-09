using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public delegate void HasteListEvent(IHasteResult item);

  public class HasteList : ScriptableObject {

    private static readonly IHasteResult[] emptyResults = new IHasteResult[0];

    Vector2 scrollPosition = Vector2.zero;

    public event HasteListEvent ItemDrag;
    public event HasteListEvent ItemMouseDown;
    public event HasteListEvent ItemClick;
    public event HasteListEvent ItemDoubleClick;

    private int highlightedIndex = 0;
    int HighlightedIndex {
      get { return highlightedIndex; }
      set { highlightedIndex = Mathf.Clamp(value, 0, Items.Length - 1); }
    }

    private IHasteResult[] items = new IHasteResult[0];
    public IHasteResult[] Items {
      get {
        return items;
      }
    }

    public void SetItems(IHasteResult[] items) {
      this.items = items;

      HighlightedIndex = 0;
      ResetScroll();
      if (HighlightedItem != null) {
        ItemClick(HighlightedItem);
      }
    }

    public void ClearItems() {
      SetItems(emptyResults);
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
      HighlightedIndex = Mathf.Max(HighlightedIndex - 1, 0);
      ScrollTo(HighlightedIndex);
    }

    public void OnDownArrow() {
      HighlightedIndex = Mathf.Min(HighlightedIndex + 1, Items.Length - 1);
      ScrollTo(HighlightedIndex);
    }

    void OnItemDrag(Event e, int index) {
      HighlightedIndex = index;
      if (HighlightedItem != null && ItemDrag != null) {
        ItemDrag(HighlightedItem);
      }
    }

    void OnItemMouseDown(Event e, int index) {
      HighlightedIndex = index;
      if (HighlightedItem != null && ItemMouseDown != null) {
        ItemMouseDown(HighlightedItem);
      }
    }

    void OnItemClick(Event e, int index) {
      HighlightedIndex = index;
      if (HighlightedItem != null && ItemClick != null) {
        ItemClick(HighlightedItem);
      }
    }

    void OnItemDoubleClick(Event e, int index) {
      HighlightedIndex = index;
      if (HighlightedItem != null && ItemDoubleClick != null) {
        ItemDoubleClick(HighlightedItem);
      }
    }

    public void OnGUI() {
      using (var scrollView = new HasteScrollView(scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true))) {
        scrollPosition = scrollView.ScrollPosition;

        bool isHighlighted;
        for (var i = 0; i < Items.Length; i++) {
          isHighlighted = i == HighlightedIndex;
          // TODO: This is rediculous...
          // if (Array.IndexOf(Selection.objects, Items[i].Object) != -1) {
          //   isHighlighted = true;
          // }
          // Better:
          // Items[i].IsHighlighted
          HasteListItem.Draw(Items[i], i, isHighlighted, this.OnItemMouseDown, this.OnItemClick, this.OnItemDoubleClick, this.OnItemDrag);
        }
      }
    }
  }
}
