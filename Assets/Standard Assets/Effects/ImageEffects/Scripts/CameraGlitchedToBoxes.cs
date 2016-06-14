using UnityEngine;
using UnityStandardAssets.ImageEffects;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Shaders/GlitchOffsetEffect")]
public class CameraGlitchedToBoxes : ImageEffectBase {

	Texture2D texture, correction, distortionBase;

	public float intensity = 1;			//Glitch movement
    public float distorsionRate = 0.2f; //How many time betwen distorsions
    public float distorsionRateOffset = 0.1f;   //Variance in distorsion rate
    public float distorsionTime = 0.2f; //How many time it is distorted
    public float distorsionSpeed = 0.02f; //How much time there is between distorsion changes
	public float inestability = 0.3f;	//Probability of a given division to have movement

    public bool isFPSActivated = false;

	public Canvas gui;
	private RectTransform guiRectTrans;

    private bool distorting = false;
    private float phaseChange = 0;
    private float nextDistortion = 0;

	public List<Vector3> boxesPositions;

	void Start(){
		//Creates a 1x1 texel texture with relative value 1 for correction
		correction = new Texture2D(1,1);
		correction.SetPixel (0, 0, new Color32 (1, 0, 0, 0));
		correction.filterMode = FilterMode.Point;
		correction.Apply ();

		guiRectTrans = gui.GetComponent<RectTransform>();
		boxesPositions = new List<Vector3> ();

        texture = correction;

        distortionBase = new Texture2D(100, 100);
         for (int x = 0; x < 100; x++)
         {
             for (int y = 0; y < 100; y++)
             {
                 distortionBase.SetPixel(x, y, new Color32(1, 0, 0, 0));
             }
         }
	}

	// Called by camera to apply image effect
	void OnRenderImage (RenderTexture source, RenderTexture destination) {

        //If we're working with at least one box
        if (boxesPositions.Count > 0)
        {

            //If there's a phase change
            if (Time.time >= phaseChange)
            {

                //If it was distorting
                if (distorting)
                {

                    //If the new glitch cycle hasn't glitch effect,
                    //the movement texture is the correction one,
                    //so there's no movement
                    texture = correction;
                    phaseChange = Time.time + distorsionRate + Random.Range(-distorsionRateOffset, distorsionRateOffset);
                    distorting = false;

                }
                else
                {

                    //If it wasn't distorting, create a distortion
                    distort();
                    distorting = true;

                    phaseChange = Time.time + distorsionTime;

                }
            }
            //If there's not a phase change, it is distorting and it's time for the next distortion
            else if (distorting && Time.time > nextDistortion)
            {
                distort();
            }
        }

		//Sends properties to the shader and paints
		material.SetFloat("_Intensity", intensity);
		material.SetTexture("_DispTex", texture);
		material.SetTexture("_Corr", correction);

		Graphics.Blit (source, destination, material);
	}

    private void distort(){

        //Creates a 2D texture with 1x'divisions'
        //texel size and arbitrary asigns 0 and 2 to glitchy 
        //divisions and 1 to non-glitchy divisions
        texture = Instantiate(distortionBase) as Texture2D;

        //For every box
        for (int i = 0; i < boxesPositions.Count; i++)
        {

            //Calculates the top-left border
            Vector3 supiz = boxesPositions[i];
            supiz.x -= 1.5f;
            supiz.y += 1f;
            supiz = Camera.main.WorldToViewportPoint(supiz);

            //Calculates the bottom-right border 
            Vector3 infder = boxesPositions[i];
            infder.x += 1.5f;
            infder.y -= 1f;
            infder = Camera.main.WorldToViewportPoint(infder);

            //Goes to int and makes them percent
            int sup = (int)(supiz.y * 100);
            int iz = (int)(supiz.x * 100);
            int inf = (int)(infder.y * 100);
            int der = (int)(infder.x * 100);

            //From corner to corner, assign random values
            for (int x = iz; x < der; x++)
            {
                for (int y = inf; y < sup; y++)
                {
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

        texture.filterMode = FilterMode.Point;
        texture.Apply();
        nextDistortion = Time.time + distorsionSpeed;
    }

	public void AddBox(Vector3 position)
	{
		boxesPositions.Add (position);
	}

	public void RemoveBox(Vector3 position)
	{
		int index = boxesPositions.FindIndex(a => a == position);
		boxesPositions.RemoveAt (index);

        if (boxesPositions.Count == 0)
        {
            texture = correction;
        }
	}
}