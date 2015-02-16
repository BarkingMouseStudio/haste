using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Haste {

  [Serializable]
  public class HasteBlur : ScriptableObject {

    [SerializeField]
    Material blurMaterial;

    public HasteBlur Init(Color tint) {
      blurMaterial = new Material(Shader.Find("Hidden/Haste/Blur"));
      blurMaterial.hideFlags = HideFlags.HideAndDontSave;
      blurMaterial.SetColor("_Tint", tint);
      blurMaterial.SetFloat("_BlurSize", 2.0f);
      return this;
    }

    void OnEnable() {
      base.hideFlags = HideFlags.HideAndDontSave;
    }

    public void Apply(Texture sourceTexture, RenderTexture destTexture, int passes = 10) {
      RenderTexture active = RenderTexture.active; // Save original RenderTexture

      try {
        RenderTexture tempA = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height);
        RenderTexture tempB = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height);

        for (int i = 0; i < passes; i++) {
          if (i == 0) {
            Graphics.Blit(sourceTexture, tempA, blurMaterial, 0);
          } else {
            Graphics.Blit(tempB, tempA, blurMaterial, 0);
          }
          Graphics.Blit(tempA, tempB, blurMaterial, 1);
        }

        Graphics.Blit(tempB, destTexture, blurMaterial, 2);

        RenderTexture.ReleaseTemporary(tempA);
        RenderTexture.ReleaseTemporary(tempB);
      } catch (Exception e) {
        Debug.LogException(e);
      } finally {
        RenderTexture.active = active; // Restore
      }
    }
  }
}
