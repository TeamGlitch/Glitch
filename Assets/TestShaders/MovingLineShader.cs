using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;


public class MovingLineShader : ImageEffectBase
{
    [Range(0f,1f)]
    public float xPer = 0.2f;
    [Range(0f, 1f)]
    public float yPer = 0.2f;

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetFloat("xPercentage", xPer);
        material.SetFloat("yPercentage", yPer);
        Graphics.Blit(source, destination, material);
    }

}
