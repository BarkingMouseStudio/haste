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

    private float listHeight = 0.0f;

    public HasteList Init(float listHeight) {
      this.listHeight = listHeight;
      return this;
    }

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

    public bool IsEmpty {
      get {
        return items.Length == 0;
      }
    }

    public void SetItems(IHasteResult[] items) {
      this.items = items;

      HighlightedIndex = 0;
      scrollPosition = Vector2.zero;

      if (HighlightedItem != null) {
        HighlightedItem.Select();
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

    void ScrollTo(int to) {
      if (to > Items.Length - 1 || to < 0) {
        return;
      }

      float scrollY = 0.0f;
      float totalHeight = 0.0f;
      float itemHeight = 0.0f;

      for (int i = 0; i < Items.Length; i++) {
        if (Items[i] != null) {
          itemHeight = Items[i].Height(i == HighlightedIndex);
        } else {
          itemHeight = 0.0f;
        }

        if (i < to) {
          scrollY += itemHeight;
        }

        totalHeight += itemHeight;
      }

      // Limit scroll y to bottom-most scroll position
      scrollY = Mathf.Min(scrollY, totalHeight - listHeight);

      scrollPosition = new Vector2(scrollPosition.x, scrollY);
    }

    public void OnUpArrow() {
      HighlightedIndex = Mathf.Max(HighlightedIndex - 1, 0);
      ScrollTo(HighlightedIndex);
    }

    public void OnDownArrow() {
      HighlightedIndex = Mathf.Min(HighlightedIndex + 1, Items.Length - 1);
      ScrollTo(HighlightedIndex);
    }

    public void OnHome() {
      HighlightedIndex = 0;
      ScrollTo(HighlightedIndex);
    }

    public void OnEnd() {
      HighlightedIndex = Items.Length - 1;
      ScrollTo(HighlightedIndex);
    }

    public void OnPageUp() {
      HighlightedIndex = Mathf.Max(HighlightedIndex - HasteStyles.PageSize, 0);
      ScrollTo(HighlightedIndex);
    }

    public void OnPageDown() {
      HighlightedIndex = Mathf.Min(HighlightedIndex + HasteStyles.PageSize, Items.Length - 1);
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

        IHasteResult item;
        bool isHighlighted;
        float itemY = 0.0f;
        float itemHeight;

        // We can only change the rendered items during a layout event
        bool computeVisibility = Event.current.type == EventType.Layout;

        for (var i = 0; i < Items.Length; i++) {
          isHighlighted = i == HighlightedIndex;

          item = Items[i];
          itemHeight = item.Height(isHighlighted);

          if (computeVisibility) {
            item.IsVisible = itemY >= scrollPosition.y - itemHeight &&
              itemY < scrollPosition.y + listHeight + itemHeight;
          }

          if (item.IsVisible) {
            HasteListItem.Draw(item, i, isHighlighted, this.OnItemMouseDown, this.OnItemClick, this.OnItemDoubleClick, this.OnItemDrag);
          } else {
            using (new HasteVertical(GUILayout.Height(itemHeight))) {
              EditorGUILayout.Space();
            }
          }

          itemY += itemHeight;
        }
      }
    }
  }
}
