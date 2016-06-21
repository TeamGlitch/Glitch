using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;


public class InvertColorsSceneShader : ImageEffectBase
{

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }

}
