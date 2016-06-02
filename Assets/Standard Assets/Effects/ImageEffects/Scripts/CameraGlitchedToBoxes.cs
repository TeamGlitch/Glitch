using UnityEngine;
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

    public bool isFPSActivated = false;

	public Canvas gui;
	private RectTransform guiRectTrans;

	// Needs to be divided by the zoom
	private float correctionMarkerWidth = (-13.59751f*11.02f);
	private float correctionMarkerHeight = (-13.59751f*14.9f);
	private float correctionDueToAspect = 1.777605f;

	public List<Vector3> boxesPositions;

	void Start(){
		//Creates a 1x1 texel texture with relative value 1 for correction
		correction = new Texture2D(1,1);
		correction.SetPixel (0, 0, new Color32 (1, 0, 0, 0));
		correction.filterMode = FilterMode.Point;
		correction.Apply ();

		guiRectTrans = gui.GetComponent<RectTransform>();
		boxesPositions = new List<Vector3> ();
	}

	// Called by camera to apply image effect
	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		
		//If the glitch cycle has ended
		if (Time.time >= cycleEnd) {
			//Checks if the new glitch cycle has glitch effect
			if (Random.value < frequency) {

				//If it does, creates a 2D texture with 1x'divisions'
				//texel size and arbitrary asigns 0 and 2 to glitchy 
				//divisions and 1 to non-glitchy divisions
				texture = new Texture2D(100,100);

				//TODO: Clone a base so it doesn't need to do this?
				for (int x = 0; x < 100; x++) {
					for (int y = 0; y < 100; y++) {
						texture.SetPixel (x, y, new Color32 (1, 0, 0, 0));
					}
				}

				//For every box
				for (int i = 0; i < boxesPositions.Count; i++) {

					//If is on screen
					Vector3 position = Camera.main.WorldToViewportPoint(boxesPositions[i]);
					if (position.x > 0 && position.x < 1 && position.y > 0 && position.y < 1) {

						//Calculates the top-left border
						Vector3 supiz = boxesPositions[i];
						supiz.x -= 1.5f;
						supiz.y += 1f;
						supiz = Camera.main.WorldToViewportPoint(supiz);

						//Calculates the bottom-right border 
						Vector3 infder = boxesPositions [i];
						infder.x += 1.5f;
						infder.y -= 1f;
						infder = Camera.main.WorldToViewportPoint(infder);

						//Goes to int and makes them percent
						int sup = (int) (supiz.y * 100);
						int iz = (int) (supiz.x * 100);
						int inf = (int) (infder.y * 100);
						int der = (int) (infder.x * 100);

						//From corner to corner, assign random values
						for(int x = iz; x < der; x++){
							for (int y = inf; y < sup; y++) {
								if (Random.value > 0.5)
								{
									texture.SetPixel(x, y, new Color32(0, 0, 0, 0));
								}
								else
								{
									texture.SetPixel(x, y, new Color32(2, 0, 0, 0));
								}
							}
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
	}

	public void RemoveBox(Vector3 position)
	{
		int index = boxesPositions.FindIndex(a => a == position);
		boxesPositions.RemoveAt (index);
	}
}