using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

namespace Haste {

  public class HasteGUIBackground : ScriptableObject {

    Rect position;

    [SerializeField]
    HasteBlur blur;

    [SerializeField]
    RenderTexture backgroundTexture;

    public HasteGUIBackground(Rect position) {
      this.position = position;

      if (Application.HasProLicense()) {
        blur = new HasteBlur(HasteColors.BlurColor);

        backgroundTexture = new RenderTexture((int)position.width, (int)position.height, 0);
        backgroundTexture.hideFlags = HideFlags.HideAndDontSave;
        backgroundTexture.Create();
      }
    }

    public void Capture(Rect position) {
      if (blur != null) {
        // Must grab texture before Haste is visible
        using (var texture = new HasteTexture(HasteUtils.GrabScreenSwatch(position))) {
          blur.Apply(texture.Tex, backgroundTexture);
        }
      }
    }

    public void OnGUI() {
      if (backgroundTexture != null) {
        UnityEngine.GUI.DrawTexture(position, backgroundTexture);
      }
    }
  }
}
