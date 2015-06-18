using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Haste {

  [Serializable]
  public class SerializableHasteItem : ISerializable, IEquatable<SerializableHasteItem> {

    IHasteItem item;

    public IHasteItem Item {
      get {
        return item;
      }
    }

    public SerializableHasteItem(IHasteItem item) {
      this.item = item;
    }

    public SerializableHasteItem(SerializationInfo info, StreamingContext context) {
      var source = (string)info.GetValue("source", typeof(string));
      switch (source) {
        case "Hierarchy":
          this.item = new HasteHierarchyItem(info, context);
          break;
        case "Project":
          this.item = new HasteProjectItem(info, context);
          break;
        case "Menu Item":
          this.item = new HasteMenuItem(info, context);
          break;
        case "Layout":
          this.item = new HasteMenuItem(info, context);
          break;
        default:
          this.item = new HasteItem(info, context);
          break;
      }
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context) {
      item.GetObjectData(info, context);
    }

    public override bool Equals(object obj) {
      if (obj == null) {
        return false;
      }

      SerializableHasteItem objAsPart = obj as SerializableHasteItem;
      if (objAsPart == null) {
        return false;
      } else {
        return Equals(objAsPart);
      }
    }

    public override int GetHashCode() {
      return item.GetHashCode();
    }

    public bool Equals(SerializableHasteItem other) {
      if (other == null) {
        return false;
      }

      return this.GetHashCode().Equals(other.GetHashCode());
    }
  }
}
