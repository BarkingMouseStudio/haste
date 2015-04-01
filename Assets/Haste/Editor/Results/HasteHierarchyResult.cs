using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteHierarchyResult : AbstractHasteResult {

    public static void LoadGameObjectIcon() {
      gameObjectIcon = EditorGUIUtility.ObjectContent(null, typeof(GameObject)).image;
    }

    private static Texture gameObjectIcon;
    public static Texture GameObjectIcon {
      get {
        if (gameObjectIcon == null) {
          LoadGameObjectIcon();
        }
        return gameObjectIcon;
      }
    }

    private UnityEngine.Object object_;
    public override UnityEngine.Object Object {
      get {
        if (object_ == null) {
          object_ = EditorUtility.InstanceIDToObject(Item.Id);
        }
        return object_;
      }
    }

    public override bool IsDraggable {
      get { return true; }
    }

    public override string DragLabel {
      get {
        if (Object == null) {
          return "<destroyed>";
        }
        return Object.name;
      }
    }

    public HasteHierarchyResult(IHasteItem item, string query, int queryLen) : base(item, query, queryLen) {}

    GUIStyle GetLabelStyle(GameObject go) {
      if (go == null) {
        return HasteStyles.Skin.GetStyle("DisabledName");
      }
      switch (PrefabUtility.GetPrefabType(go)) {
        case PrefabType.PrefabInstance:
        case PrefabType.ModelPrefabInstance:
          if (go.activeInHierarchy) {
            return HasteStyles.Skin.GetStyle("Prefab");
          } else {
            return HasteStyles.Skin.GetStyle("DisabledPrefab");
          }
        case PrefabType.MissingPrefabInstance:
          if (go.activeInHierarchy) {
            return HasteStyles.Skin.GetStyle("BrokenPrefab");
          } else {
            return HasteStyles.Skin.GetStyle("DisabledBrokenPrefab");
          }
        default:
          if (go.activeInHierarchy) {
            return HasteStyles.Skin.GetStyle("Name");
          } else {
            return HasteStyles.Skin.GetStyle("DisabledName");
          }
      }
    }

    public override void Draw(bool isHighlighted, bool highlightMatches) {
      GameObject go = (GameObject)Object;

      var rect = EditorGUILayout.GetControlRect(GUILayout.Width(32), GUILayout.Height(32));
      rect.y += 5; // center the icon vertically
      GUI.DrawTexture(rect, GameObjectIcon);

      using (new HasteVertical()) {
        var childCount = 0;
        if (go != null && go.transform != null) {
          childCount = go.transform.childCount;
        }

        if (childCount > 0) {
          EditorGUILayout.LabelField(String.Format("{0} ({1})", Path.GetFileName(Item.Path), childCount), isHighlighted ? HasteStyles.Skin.GetStyle("HighlightedName") : GetLabelStyle(go));
        } else {
          if (go == null) {
            EditorGUILayout.LabelField(String.Format("{0} <destroyed>", Path.GetFileName(Item.Path), childCount), isHighlighted ? HasteStyles.Skin.GetStyle("HighlightedName") : GetLabelStyle(go));
          } else {
            EditorGUILayout.LabelField(Path.GetFileName(Item.Path), isHighlighted ? HasteStyles.Skin.GetStyle("HighlightedName") : GetLabelStyle(go));
          }
        }
        if (highlightMatches) {
          EditorGUILayout.LabelField(HasteStringUtils.BoldLabel(Item.Path, Indices, isHighlighted ? HasteStyles.HighlightedBoldStart : HasteStyles.BoldStart, HasteStyles.BoldEnd), isHighlighted ? HasteStyles.Skin.GetStyle("HighlightedDescription") : HasteStyles.Skin.GetStyle("Description"));
        } else {
          EditorGUILayout.LabelField(Item.Path, isHighlighted ? HasteStyles.Skin.GetStyle("HighlightedDescription") : HasteStyles.Skin.GetStyle("Description"));
        }
      }
    }

    public override void Action() {
      EditorApplication.ExecuteMenuItem("Window/Hierarchy");
      Selection.instanceIDs = new int[]{Item.Id};
      EditorGUIUtility.PingObject(Selection.activeInstanceID);
    }
  }
}
