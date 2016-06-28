using UnityEngine;
using UnityStandardAssets.ImageEffects;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Shaders/GlitchOffsetEffect")]
public class CameraGlitchedToBoxes : ImageEffectBase
{

    float cycleEnd = 0;
    Texture2D texture, correction;

    public float intensity = 1;         //Glitch movement
    public float cycleDuration = 0.05f; //Duration of a glitch cycle
    public float frequency = 0.15f;     //Probability of a glitch cycle having a glitch effect
    public float inestability = 0.3f;	//Probability of a given division to have movement

    public bool isFPSActivated = false;

    public Canvas gui;
    private RectTransform guiRectTrans;

    // Needs to be divided by the zoom
    private float correctionMarkerWidth = (-13.59751f * 11.02f);
    private float correctionMarkerHeight = (-13.59751f * 14.9f);
    private float correctionDueToAspect = 1.777605f;

    public List<Vector3> boxesPositions;
    public List<Vector2> boxPositionInPercentage;

    void Start()
    {
        //Creates a 1x1 texel texture with relative value 1 for correction
        correction = new Texture2D(1, 1);
        correction.SetPixel(0, 0, new Color32(1, 0, 0, 0));
        correction.filterMode = FilterMode.Point;
        correction.Apply();

        guiRectTrans = gui.GetComponent<RectTransform>();
        boxPositionInPercentage = new List<Vector2>();
        boxesPositions = new List<Vector3>();
    }

    // Called by camera to apply image effect
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        CalculatebBoxPositionsInPercentage();
        //If the glitch cycle has ended
        float horizontalPercentage = (correctionDueToAspect / Camera.main.aspect) * (correctionMarkerWidth / Camera.main.transform.position.z) / 2.0f;
        float verticalPercentage = (correctionMarkerHeight / Camera.main.transform.position.z) / 2.0f;

        if (Time.time >= cycleEnd)
        {
            //Checks if the new glitch cycle has glitch effect
            if (Random.value < frequency)
            {

                //If it does, creates a 2D texture with 1x'divisions'
                //texel size and arbitrary asigns 0 and 2 to glitchy 
                //divisions and 1 to non-glitchy divisions
                texture = new Texture2D(100, 100);
                if ((QualitySettings.antiAliasing != 0 && isFPSActivated) || (QualitySettings.antiAliasing == 0 && !isFPSActivated))
                {
                    for (int z = 0; z < 100; z += 1)
                    {
                        for (int w = 0; w < 100; ++w)
                        {
                            if (InsideBox(z, 99 - w, horizontalPercentage, verticalPercentage))
                            {
                                if (Random.value < inestability)
                                {
                                    if (Random.value > 0.5)
                                    {
                                        texture.SetPixel(z, w, new Color32(0, 0, 0, 0));
                                    }
                                    else
                                    {
                                        texture.SetPixel(z, w, new Color32(2, 0, 0, 0));
                                    }
                                }
                                else
                                {
                                    texture.SetPixel(z, w, new Color32(1, 0, 0, 0));
                                }
                            }
                            else
                            {
                                texture.SetPixel(z, w, new Color32(1, 0, 0, 0));
                            }
                        }
                    }
                }
                else
                {
                    for (int z = 0; z < 100; z += 1)
                    {
                        for (int w = 0; w < 100; ++w)
                        {
                            if (InsideBox(z, w, horizontalPercentage, verticalPercentage))
                            {
                                if (Random.value < inestability)
                                {
                                    if (Random.value > 0.5)
                                    {
                                        texture.SetPixel(z, w, new Color32(0, 0, 0, 0));
                                    }
                                    else
                                    {
                                        texture.SetPixel(z, w, new Color32(2, 0, 0, 0));
                                    }
                                }
                                else
                                {
                                    texture.SetPixel(z, w, new Color32(1, 0, 0, 0));
                                }
                            }
                            else
                            {
                                texture.SetPixel(z, w, new Color32(1, 0, 0, 0));
                            }
                        }
                    }
                }

                texture.filterMode = FilterMode.Point;
                texture.Apply();

            }
            else
            {

                //If the new glitch cycle hasn't glitch effect,
                //the movement texture is the correction one,
                //so there's no movement
                texture = correction;

            }
            cycleEnd = Time.time + cycleDuration;
        }

        //Sends properties to the shader and paints
        material.SetFloat("_Intensity", intensity);
        material.SetTexture("_DispTex", texture);
        material.SetTexture("_Corr", correction);

        Graphics.Blit(source, destination, material);
    }

    public void AddBox(Vector3 position)
    {
        boxesPositions.Add(position);
        boxPositionInPercentage.Add(new Vector2(0.0f, 0.0f));
    }

    public void RemoveBox(Vector3 position)
    {
        int index = boxesPositions.FindIndex(a => a == position);
        boxesPositions.RemoveAt(index);
        boxPositionInPercentage.RemoveAt(index);
    }

    private void CalculatebBoxPositionsInPercentage()
    {
        for (int i = 0; i < boxesPositions.Count; ++i)
        {
            Vector3 camPosition = Camera.main.WorldToScreenPoint(boxesPositions[i]);
            camPosition.x *= guiRectTrans.rect.width / Camera.main.pixelWidth;
            camPosition.y *= guiRectTrans.rect.height / Camera.main.pixelHeight;
            boxPositionInPercentage[i] = new Vector2((camPosition.x / guiRectTrans.rect.width) * 100.0f, 100.0f - (camPosition.y / guiRectTrans.rect.height) * 100.0f);
        }
    }

    private bool InsideBox(int x, int y, float horizontalPercentage, float verticalPercentage)
    {
        for (int w = 0; w < boxesPositions.Count; ++w)
        {
            if (x > boxPositionInPercentage[w].x - horizontalPercentage && x < boxPositionInPercentage[w].x + horizontalPercentage && y > boxPositionInPercentage[w].y - verticalPercentage && y < boxPositionInPercentage[w].y + verticalPercentage)
                return true;
        }
        return false;
    }

    private bool BorderBox(int x, int y, float horizontalPercentage, float verticalPercentage)
    {
        for (int w = 0; w < boxesPositions.Count; ++w)
        {
            if ((x > boxPositionInPercentage[w].x - horizontalPercentage &&
                x < boxPositionInPercentage[w].x - horizontalPercentage + 1f &&
                y > boxPositionInPercentage[w].y - verticalPercentage &&
                y < boxPositionInPercentage[w].y + verticalPercentage) ||

                (x < boxPositionInPercentage[w].x + horizontalPercentage &&
                x > boxPositionInPercentage[w].x + horizontalPercentage - 1f &&
                y > boxPositionInPercentage[w].y - verticalPercentage &&
                y < boxPositionInPercentage[w].y + verticalPercentage) ||

                (y > boxPositionInPercentage[w].y - verticalPercentage &&
                y < boxPositionInPercentage[w].y - verticalPercentage + 1f &&
                x > boxPositionInPercentage[w].x - horizontalPercentage &&
                x < boxPositionInPercentage[w].x + horizontalPercentage) ||

                (y < boxPositionInPercentage[w].y + verticalPercentage &&
                y > boxPositionInPercentage[w].y + verticalPercentage - 1f &&
                x > boxPositionInPercentage[w].x - horizontalPercentage &&
                x < boxPositionInPercentage[w].x + horizontalPercentage))
                return true;
        }
        return false;
    }

}