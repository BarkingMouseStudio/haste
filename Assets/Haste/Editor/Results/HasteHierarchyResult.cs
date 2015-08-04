using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteHierarchyResult : AbstractHasteResult {

    private static Texture gameObjectIcon;

    public static void LoadGameObjectIcon() {
      gameObjectIcon = EditorGUIUtility.ObjectContent(null, typeof(GameObject)).image;
    }

    public static Texture GameObjectIcon {
      get {
        if (gameObjectIcon == null) {
          LoadGameObjectIcon();
        }
        return gameObjectIcon;
      }
    }

    private UnityEngine.Object obj;
    public override UnityEngine.Object Object {
      get {
        if (obj == null) {
          HasteHierarchySource.Scene.TryGetValue(this.Item.GetHashCode(), out obj); // EditorUtility.InstanceIDToObject(Item.id);
        }
        return obj;
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

    public HasteHierarchyResult(HasteItem item, float score, string queryLower) : base(item, score, queryLower) {}

    GUIStyle GetLabelStyle(GameObject go, bool isHighlighted) {
      if (go == null) {
        return isHighlighted ? HasteStyles.GetStyle("HighlightedDisabledName") :
          HasteStyles.GetStyle("DisabledName");
      }
      switch (PrefabUtility.GetPrefabType(go)) {
        case PrefabType.PrefabInstance:
        case PrefabType.ModelPrefabInstance:
          if (go.activeInHierarchy) {
            return isHighlighted ? HasteStyles.GetStyle("HighlightedPrefabName") :
              HasteStyles.GetStyle("PrefabName");
          } else {
            return isHighlighted ? HasteStyles.GetStyle("HighlightedDisabledPrefabName") :
              HasteStyles.GetStyle("DisabledPrefabName");
          }
        case PrefabType.MissingPrefabInstance:
          if (go.activeInHierarchy) {
            return isHighlighted ? HasteStyles.GetStyle("HighlightedBrokenPrefabName") :
              HasteStyles.GetStyle("BrokenPrefabName");
          } else {
            return isHighlighted ? HasteStyles.GetStyle("HighlightedDisabledBrokenPrefabName") :
              HasteStyles.GetStyle("DisabledBrokenPrefabName");
          }
        default:
          if (go.activeInHierarchy) {
            return isHighlighted ? HasteStyles.GetStyle("HighlightedName") :
              HasteStyles.GetStyle("Name");
          } else {
            return isHighlighted ? HasteStyles.GetStyle("HighlightedDisabledName") :
              HasteStyles.GetStyle("DisabledName");
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
          name = String.Format("{0} ({1})", HasteStringUtils.GetFileName(Item.path), childCount);
        } else if (go == null) {
          name = String.Format("{0} <destroyed>", HasteStringUtils.GetFileName(Item.path), childCount);
        } else {
          name = HasteStringUtils.GetFileName(Item.path);
        }
        EditorGUILayout.LabelField(name, nameStyle);

        // Description
        string boldStart = isHighlighted ? HasteStyles.HighlightedBoldStart : HasteStyles.BoldStart;
        GUIStyle descriptionStyle = isHighlighted ? HasteStyles.GetStyle("HighlightedDescription") : HasteStyles.GetStyle("Description");
        EditorGUILayout.LabelField(HasteStringUtils.BoldLabel(Item.path, Indices, boldStart, HasteStyles.BoldEnd), descriptionStyle);
      }
    }

    public override void Action() {
      EditorApplication.ExecuteMenuItem("Window/Hierarchy");
      Selection.instanceIDs = new int[]{Object.GetInstanceID()};
      EditorGUIUtility.PingObject(Selection.activeInstanceID);
    }
  }
}
