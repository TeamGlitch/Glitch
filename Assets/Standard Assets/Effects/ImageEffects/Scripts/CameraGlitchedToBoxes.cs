﻿using UnityEngine;
using UnityStandardAssets.ImageEffects;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Shaders/GlitchOffsetEffect")]
public class CameraGlitchedToBoxes : ImageEffectBase {

	float cycleEnd = 0;
	Texture2D texture, correction;

	public float intensity = 1;			//Glitch movement
	public float cycleDuration = 0.05f;	//Duration of a glitch cycle
	public float frequency = 0.15f;		//Probability of a glitch cycle having a glitch effect
	public float inestability = 0.3f;	//Probability of a given division to have movement

	public Canvas gui;
	private RectTransform guiRectTrans;

	public List<Vector3> boxesPositions;
	public List<Vector2> boxPositionInPercentage;

	void Start(){
		//Creates a 1x1 texel texture with relative value 1 for correction
		correction = new Texture2D(1,1);
		correction.SetPixel (0, 0, new Color32 (1, 0, 0, 0));
		correction.filterMode = FilterMode.Point;
		correction.Apply ();

		guiRectTrans = gui.GetComponent<RectTransform>();
		boxPositionInPercentage = new List<Vector2> ();
		boxesPositions = new List<Vector3> ();
	}

	// Called by camera to apply image effect
	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		CalculatebBoxPositionsInPercentage ();
		//If the glitch cycle has ended
		if (Time.time >= cycleEnd) {
			//Checks if the new glitch cycle has glitch effect
			if (Random.value < frequency) {

				//If it does, creates a 2D texture with 1x'divisions'
				//texel size and arbitrary asigns 0 and 2 to glitchy 
				//divisions and 1 to non-glitchy divisions
				texture = new Texture2D(100,100);
				for (int z = 0; z < 100; z += 1) {
					for (int w = 0; w < 100; ++w) {
						if (InsideBox(z,w)) {
							if (Random.value < inestability) {
								if (Random.value > 0.5) {
									texture.SetPixel (z, w, new Color32 (0, 0, 0, 0));
								} else {
									texture.SetPixel (z, w, new Color32 (2, 0, 0, 0));
								}
							} else {
								texture.SetPixel (z, w, new Color32 (1, 0, 0, 0));
							}
						} else {
							texture.SetPixel (z, w, new Color32 (1, 0, 0, 0));
						}
					}
				}
				texture.filterMode = FilterMode.Point;
				texture.Apply();

			} else {

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

		Graphics.Blit (source, destination, material);
	}

	public void AddBox(Vector3 position)
	{
		boxesPositions.Add (position);
		boxPositionInPercentage.Add (new Vector2 (0.0f, 0.0f));
	}

	public void RemoveBox(Vector3 position)
	{
		int index = boxesPositions.FindIndex(a => a == position);
//		if(index != -1)
//		{
			boxesPositions.RemoveAt (index);
			boxPositionInPercentage.RemoveAt (index);
//		}
	}

	private void CalculatebBoxPositionsInPercentage()
	{
		for (int i = 0; i < boxesPositions.Count; ++i) {
			Vector3 camPosition = Camera.main.WorldToScreenPoint(boxesPositions[i]);
			camPosition.x *= guiRectTrans.rect.width / Camera.main.pixelWidth; 
			camPosition.y *= guiRectTrans.rect.height / Camera.main.pixelHeight; 
			boxPositionInPercentage [i] = new Vector2 ((camPosition.x / guiRectTrans.rect.width) * 100.0f, 100.0f - (camPosition.y / guiRectTrans.rect.height) * 100.0f);
		}
	}

	private bool InsideBox(int x, int y)
	{
		for (int w = 0; w < boxesPositions.Count; ++w)
		{
			if (x > boxPositionInPercentage [w].x - 10.0f && x < boxPositionInPercentage [w].x + 6.0f && y > boxPositionInPercentage [w].y - 10 && y < boxPositionInPercentage [w].y + 10)
				return true;
		}
		return false;
	}
}