﻿using UnityEngine;
using System.Collections;

public class GlitchSpriteOffset : MonoBehaviour {

	float cycleEnd = 0;
	Texture2D texture, correction;
	
	public float intensity = 1;			//Glitch movement
	public int divisions = 10;			//Number of divisions on a glitch cycle
	public float cycleDuration = 0.05f;	//Duration of a glitch cycle
	public float frequency = 0.15f;		//Probability of a glitch cycle having a glitch effect
	public float inestability = 0.3f;	//Probability of a given division to have movement

	// Use this for initialization
	void Start () {
		//Creates a 1x1 texel texture with relative value 1 for correction
		correction = new Texture2D(1,1);
		correction.SetPixel (0, 0, new Color32 (1, 0, 0, 0));
		correction.filterMode = FilterMode.Point;
		correction.Apply ();
	}
	
	// Update is called once per frame
	void Update () {

		Material material = GetComponent<SpriteRenderer>().material;

		//If the glitch cycle has ended
		if (Time.time >= cycleEnd) {
			//Checks if the new glitch cycle has glitch effect
			if (Random.value < frequency) {
				
				//If it does, creates a 2D texture with 1x'divisions'
				//texel size and arbitrary asigns 0 and 2 to glitchy 
				//divisions and 1 to non-glitchy divisions
				texture = new Texture2D(1,divisions);
				for (int z = 0; z < divisions; z += 1) {
					
					if (Random.value < inestability) {
						if (Random.value > 0.5) {
							texture.SetPixel (0, z, new Color32 (0, 0, 0, 0));
						} else {
							texture.SetPixel (0, z, new Color32 (2, 0, 0, 0));
						}
					} else {
						texture.SetPixel (0, z, new Color32 (1, 0, 0, 0));
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
	}
}
