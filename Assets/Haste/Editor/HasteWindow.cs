using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Haste {

  public enum HasteWindowState {
    Intro,
    Empty,
    Results,
    Loading
  }

  [Serializable]
  public class HasteWindow : EditorWindow {

    const int RESULT_COUNT = 100;

    HasteWindowState windowState = HasteWindowState.Intro;

    [SerializeField]
    UnityEngine.Object[] prevSelection;

    [SerializeField]
    HashSet<UnityEngine.Object> nextSelection;

    [SerializeField]
    HasteQuery queryInput;

    [SerializeField]
    HasteEmpty empty;

    [SerializeField]
    HasteLoading loading;

    [SerializeField]
    HasteIntro intro;

    [SerializeField]
    HasteList resultList;

    Rect selectionPosition;

    HasteUpdateStatus prevUpdateStatus = HasteUpdateStatus.UpToDate;
    bool wasIndexing = false;
    bool wasSearching = false;

    HasteSchedulerNode searching;

    public static HasteWindow Instance { get; protected set; }

    public static bool IsOpen {
      get {
        return Instance != null;
      }
    }

    public static Rect GetPosition() {
      var position = HasteSettings.WindowPosition;
      if (position == Vector2.zero) {
        position = new Vector2(
          (Screen.currentResolution.width - HasteStyles.WindowWidth) / 2,
          (Screen.currentResolution.height - HasteStyles.WindowHeight) / 2
        );
      }
      return new Rect(position.x, position.y,
        HasteStyles.WindowWidth, HasteStyles.WindowHeight);
    }

    public static void Open() {
      EditorApplication.LockReloadAssemblies();

      if (HasteWindow.Instance == null) {
        HasteWindow.Init();
      } else {
        // Return the existing window if it exists
        EditorWindow.GetWindow<HasteWindow>();
      }
    }

    // Creates a new window instance and initializes it
    static void Init() {
      // Increment open count
      HasteSettings.UsageCount++;

      if (HasteSettings.ShowHandle) {
        if (HasteWindow.IsOpen) {
          // Close any old instances before opening the window
          HasteWindow.Instance.Close();
        }
        HasteWindow.Instance = EditorWindow.GetWindowWithRect<HasteWindow>(GetPosition(), true);
        HasteWindow.Instance.InitializeInstance();
        return;
      }

      HasteWindow.Instance = EditorWindow.CreateInstance<HasteWindow>();
      HasteWindow.Instance.InitializeInstance();

      HasteWindow.Instance.ShowPopup();
      HasteWindow.Instance.Focus();
    }

    void InitializeInstance() {
      this.title = "Haste";
      this.position = GetPosition();

      // Disable the resize handle on the window
      this.minSize = this.maxSize =
        new Vector2(this.position.width, this.position.height);

      this.selectionPosition = new Rect(HasteStyles.WindowWidth - 90, 24, 80, 80);

      if (Selection.objects != null) {
        this.prevSelection = new UnityEngine.Object[Selection.objects.Length];
        Array.Copy(Selection.objects, this.prevSelection, Selection.objects.Length);
      }

      this.nextSelection = new HashSet<UnityEngine.Object>();

      this.queryInput = ScriptableObject.CreateInstance<HasteQuery>();
      this.queryInput.Changed += OnQueryChanged;

      this.resultList = ScriptableObject.CreateInstance<HasteList>().Init(HasteStyles.ListHeight);
      this.resultList.ItemDrag += OnItemDrag;
      this.resultList.ItemMouseDown += OnItemHighlight;
      this.resultList.ItemClick += OnItemSelect;
      this.resultList.ItemDoubleClick += OnItemAction;

      #if IS_HASTE_PRO
      RestoreRecommendations();
      #endif

      var tip = HasteTips.Random;
      this.intro = ScriptableObject.CreateInstance<HasteIntro>().Init(tip);
      this.empty = ScriptableObject.CreateInstance<HasteEmpty>().Init(tip);
      this.loading = ScriptableObject.CreateInstance<HasteLoading>();
    }

    public bool IsSelected(UnityEngine.Object obj) {
      return nextSelection.Contains(obj);
    }

    void OnEscape(Event e) {
      Selection.objects = prevSelection;
      Close();
    }

    void OnReturn(Event e) {
      if (windowState != HasteWindowState.Results) {
        return;
      }

      // Add to multi-selection
      if (EditorGUI.actionKey) {
        var obj = this.resultList.HighlightedItem.Object;
        if (obj != null) {
          if (nextSelection.Contains(obj)) {
            nextSelection.Remove(obj);
          } else {
            nextSelection.Add(obj);
          }
          Selection.objects = nextSelection.ToArray();
        }
        return;
      }

      // Perform multi-selection
      if (nextSelection.Count > 0) {
        if (nextSelection.Any(x => x is GameObject)) {
          EditorApplication.ExecuteMenuItem("Window/Hierarchy");
        } else {
          EditorApplication.ExecuteMenuItem("Window/Project");
          EditorUtility.FocusProjectWindow();
        }
        Selection.objects = nextSelection.ToArray();
        Close();
        return;
      }

      if (prevSelection.Any(x => x is GameObject)) {
        // Do this again so the window (with selection)
        // actually gets focus.
        EditorApplication.ExecuteMenuItem("Window/Hierarchy");
      }

      // Restore selection in case the action affects
      // the original selection.
      Selection.objects = prevSelection;

      if (this.resultList.HighlightedItem != null) {
        #if IS_HASTE_PRO
          Haste.Recommendations.Add(this.resultList.HighlightedItem.Item);
        #endif

        // Register action to occur after the window is closed and destroyed.
        // This is done to prevent errors when modifying window layouts and
        // other Unity state while Haste is open.
        Haste.WindowAction += this.resultList.HighlightedItem.Action;
      }

      Close();
    }

    void OnItemDrag(IHasteResult item) {
      Selection.objects = prevSelection;

      DragAndDrop.PrepareStartDrag();

      if (nextSelection.Contains(item.Object)) {
        if (nextSelection.Count == 1) {
          DragAndDrop.objectReferences = nextSelection.ToArray();
          DragAndDrop.StartDrag(item.Object.name);
        } else {
          DragAndDrop.objectReferences = nextSelection.ToArray();
          DragAndDrop.StartDrag("<Multiple>");
        }
      } else {
        DragAndDrop.objectReferences = new UnityEngine.Object[]{item.Object};
        DragAndDrop.StartDrag(item.DragLabel);
      }
    }

    void OnItemHighlight(IHasteResult item) {
      Repaint();
    }

    void OnItemSelect(IHasteResult item) {
      if (EditorGUI.actionKey) {
        var obj = item.Object;
        if (obj != null) {
          if (nextSelection.Contains(obj)) {
            nextSelection.Remove(obj);
          } else {
            nextSelection.Add(obj);
          }
          Selection.objects = nextSelection.ToArray();
        }

        Repaint();
        return;
      }

      item.Select();
    }

    void OnItemAction(IHasteResult item) {
      #if IS_HASTE_PRO
        Haste.Recommendations.Add(item.Item);
      #endif

      Selection.objects = prevSelection;
      Haste.WindowAction += item.Action;
      Close();
    }

    new void Close() {
      if (searching != null && searching.IsRunning) {
        searching.Stop();
      }

      EditorApplication.UnlockReloadAssemblies();

      base.Close();
    }

    void OnHome(Event e) {
      this.resultList.OnHome();
      if (this.resultList.HighlightedItem != null) {
        this.resultList.HighlightedItem.Select();
      }
    }

    void OnEnd(Event e) {
      this.resultList.OnEnd();
      if (this.resultList.HighlightedItem != null) {
        this.resultList.HighlightedItem.Select();
      }
    }

    void OnPageUp(Event e) {
      this.resultList.OnPageUp();
      if (this.resultList.HighlightedItem != null) {
        this.resultList.HighlightedItem.Select();
      }
    }

    void OnPageDown(Event e) {
      this.resultList.OnPageDown();
      if (this.resultList.HighlightedItem != null) {
        this.resultList.HighlightedItem.Select();
      }
    }

    void OnUpArrow(Event e) {
      this.resultList.OnUpArrow();
      if (this.resultList.HighlightedItem != null) {
        this.resultList.HighlightedItem.Select();
      }
    }

    void OnDownArrow(Event e) {
      this.resultList.OnDownArrow();
      if (this.resultList.HighlightedItem != null) {
        this.resultList.HighlightedItem.Select();
      }
    }

    void OnKeyDown(Event e) {
      switch (e.keyCode) {
        case KeyCode.Escape:
          e.Use();
          OnEscape(e);
          break;
        case KeyCode.Return:
          e.Use();
          OnReturn(e);
          break;
        case KeyCode.Home:
          e.Use();
          OnHome(e);
          break;
        case KeyCode.End:
          e.Use();
          OnEnd(e);
          break;
        case KeyCode.PageUp:
          e.Use();
          OnPageUp(e);
          break;
        case KeyCode.PageDown:
          e.Use();
          OnPageDown(e);
          break;
        case KeyCode.UpArrow:
          e.Use();
          OnUpArrow(e);
          break;
        case KeyCode.DownArrow:
          e.Use();
          OnDownArrow(e);
          break;
      }
    }

    void OnEvent(Event e) {
      switch (e.type) {
        case EventType.KeyDown:
          OnKeyDown(e);
          break;
      }
    }

    void Update() {
      bool needsRepaint = false;
      bool isSearching = searching != null && searching.IsRunning;

      // Repaint window if search state changes
      if (isSearching) {
        wasSearching = isSearching;
        needsRepaint = true;
      } else if (wasSearching) {
        wasSearching = false;
        needsRepaint = true;
      }

      // Repaint window if indexing state changes
      if (Haste.IsIndexing) {
        wasIndexing = Haste.IsIndexing;
        needsRepaint = true;

      } else if (wasIndexing) {
        wasIndexing = false;
        needsRepaint = true;
      }

      // Repaint window if update state changes
      if (Haste.UpdateChecker.Status != prevUpdateStatus) {
        needsRepaint = true;
      }

      prevUpdateStatus = Haste.UpdateChecker.Status;

      if (needsRepaint) {
        Repaint();
      }

      // this.queryInput.UpdateHandler(this);

      if (!HasteSettings.ShowHandle && this != EditorWindow.focusedWindow) {
        // Check if we lost focus and close:
        // Cannot use OnLostFocus due to render bug in Unity
        Selection.objects = prevSelection;
        Close();
      }
    }

    double searchStart;
    const double loadingDelay = 0.25;

    IEnumerator BeginSearch(string query) {
      searchStart = EditorApplication.timeSinceStartup;

      var searchResults = new Promise<IHasteResult[]>();
      yield return Haste.Scheduler.Start(Haste.Search.Search(query, RESULT_COUNT, searchResults)); // wait on search

      #if DEBUG
      if (searchResults.Reason != null) {
        Debug.LogException(searchResults.Reason);
        yield break;
      }
      #endif

      if (searchResults.Value != null) {
        this.resultList.SetItems(searchResults.Value);
      }
    }

    #if IS_HASTE_PRO
    void RestoreRecommendations() {
      var recommendations = Haste.Recommendations.Get();
      if (recommendations.Length > 0) {
        this.resultList.SetItems(recommendations);
      } else {
        this.resultList.ClearItems();
      }
    }
    #endif

    void OnQueryChanged(string query) {
      if (searching != null) {
        searching.Stop();
      }

      if (query == "") {
        #if IS_HASTE_PRO
        RestoreRecommendations();
        #endif
      } else {
        searching = Haste.Scheduler.Start(BeginSearch(query));
      }
    }

    // This function should have no side effects except updating `windowState`
    void UpdateWindowState() {
      var duration = TimeSpan.FromSeconds(EditorApplication.timeSinceStartup - searchStart);
      var isSearching = searching != null && searching.IsRunning;
      var isLong = duration.TotalSeconds >= loadingDelay;

      if (!isSearching || isLong) { // Don't update right away if we're searching
        if (this.queryInput.Query == "") {
          #if IS_HASTE_PRO
            if (this.resultList.IsEmpty) {
              this.windowState = HasteWindowState.Intro;
            } else {
              this.windowState = HasteWindowState.Results;
            }
          #else
            this.windowState = HasteWindowState.Intro;
          #endif
        } else if (isSearching) {
          this.windowState = HasteWindowState.Loading;
        } else if (this.resultList.Size > 0) {
          this.windowState = HasteWindowState.Results;
        } else {
          this.windowState = HasteWindowState.Empty;
        }
      }
    }

    void OnGUI() {
      OnEvent(Event.current);

      this.queryInput.OnGUI();

      HasteSelection.Draw(selectionPosition, nextSelection);

      UpdateWindowState();

      switch (this.windowState) {
        case HasteWindowState.Intro:
          this.intro.OnGUI();
          break;
        case HasteWindowState.Loading:
          this.loading.OnGUI();
          break;
        case HasteWindowState.Results:
          this.resultList.OnGUI();
          break;
        case HasteWindowState.Empty:
          this.empty.OnGUI();
          break;
      }
    }
  }
}
