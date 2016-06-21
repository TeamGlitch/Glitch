using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;


public class BlackAndWhiteShaderScript2 : ImageEffectBase
{
    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);

    }

}
