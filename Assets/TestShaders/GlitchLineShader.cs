using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;


public class GlitchLineShader : ImageEffectBase
{
    private float _random;
    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        _random = Random.Range(0.0f, 1.0f);
        material.SetFloat("yPercentage", _random);
        Graphics.Blit(source, destination, material);
    }

}
