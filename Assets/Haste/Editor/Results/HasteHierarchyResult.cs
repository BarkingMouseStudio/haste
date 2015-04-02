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

    GUIStyle GetLabelStyle(GameObject go, bool isHighlighted) {
      if (go == null) {
        return isHighlighted ? HasteStyles.Skin.GetStyle("HighlightedDisabledName") :
          HasteStyles.Skin.GetStyle("DisabledName");
      }
      switch (PrefabUtility.GetPrefabType(go)) {
        case PrefabType.PrefabInstance:
        case PrefabType.ModelPrefabInstance:
          if (go.activeInHierarchy) {
            return isHighlighted ? HasteStyles.Skin.GetStyle("HighlightedPrefabName") :
              HasteStyles.Skin.GetStyle("PrefabName");
          } else {
            return isHighlighted ? HasteStyles.Skin.GetStyle("HighlightedDisabledPrefabName") :
              HasteStyles.Skin.GetStyle("DisabledPrefabName");
          }
        case PrefabType.MissingPrefabInstance:
          if (go.activeInHierarchy) {
            return isHighlighted ? HasteStyles.Skin.GetStyle("HighlightedBrokenPrefabName") :
              HasteStyles.Skin.GetStyle("BrokenPrefabName");
          } else {
            return isHighlighted ? HasteStyles.Skin.GetStyle("HighlightedDisabledBrokenPrefabName") :
              HasteStyles.Skin.GetStyle("DisabledBrokenPrefabName");
          }
        default:
          if (go.activeInHierarchy) {
            return isHighlighted ? HasteStyles.Skin.GetStyle("HighlightedName") :
              HasteStyles.Skin.GetStyle("Name");
          } else {
            return isHighlighted ? HasteStyles.Skin.GetStyle("HighlightedDisabledName") :
              HasteStyles.Skin.GetStyle("DisabledName");
          }
      }
    }

    public override void Draw(bool isHighlighted) {
      GameObject go = (GameObject)Object;

      var rect = EditorGUILayout.GetControlRect(GUILayout.Width(32), GUILayout.Height(32));
      rect.y += 5; // center the icon vertically
      GUI.DrawTexture(rect, GameObjectIcon);

      using (new HasteVertical()) {
        var childCount = 0;
        if (go != null && go.transform != null) {
          childCount = go.transform.childCount;
        }

        // Name
        GUIStyle nameStyle = GetLabelStyle(go, isHighlighted || IsSelected);
        string name;
        if (childCount > 0) {
          name = String.Format("{0} ({1})", Path.GetFileName(Item.Path), childCount);
        } else if (go == null) {
          name = String.Format("{0} <destroyed>", Path.GetFileName(Item.Path), childCount);
        } else {
          name = Path.GetFileName(Item.Path);
        }
        EditorGUILayout.LabelField(name, nameStyle);

        // Description
        string boldStart = isHighlighted ? HasteStyles.HighlightedBoldStart : HasteStyles.BoldStart;
        GUIStyle descriptionStyle = isHighlighted ? HasteStyles.Skin.GetStyle("HighlightedDescription") : HasteStyles.Skin.GetStyle("Description");
        EditorGUILayout.LabelField(HasteStringUtils.BoldLabel(Item.Path, Indices, boldStart, HasteStyles.BoldEnd), descriptionStyle);
      }
    }

    public override void Action() {
      EditorApplication.ExecuteMenuItem("Window/Hierarchy");
      Selection.instanceIDs = new int[]{Item.Id};
      EditorGUIUtility.PingObject(Selection.activeInstanceID);
    }
  }
}
