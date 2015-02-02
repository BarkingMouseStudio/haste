using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

namespace Haste {

  [Serializable]
  public class HasteBackground : ScriptableObject {

    [SerializeField]
    Rect position;

    [SerializeField]
    HasteBlur blur;

    [SerializeField]
    RenderTexture backgroundTexture;

    public HasteBackground Init(Rect position) {
      this.position = position;

      if (Application.HasProLicense()) {
        blur = ScriptableObject.CreateInstance<HasteBlur>().Init(HasteColors.BlurColor);

        backgroundTexture = new RenderTexture((int)position.width, (int)position.height, 0);
        backgroundTexture.hideFlags = HideFlags.HideAndDontSave;
        backgroundTexture.Create();
      }

      return this;
    }

    public void Capture(Rect position) {
      // Must grab texture before Haste is visible
      using (var texture = new HasteTexture(HasteUtils.GrabScreenSwatch(position))) {
        blur.Apply(texture.Texture, backgroundTexture);
      }
    }

    public void OnGUI() {
      if (backgroundTexture != null) {
        UnityEngine.GUI.DrawTexture(position, backgroundTexture);
      }
    }
  }
}
