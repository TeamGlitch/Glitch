using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialClouds : MonoBehaviour {

    //public RawImage backLayer;
    public RawImage frontLayer;
	public Color[] colors;

	private int actualColor = 0;
	private float colorStart;
	private bool colorChanging;

	void Start(){
		actualColor = 0;
		frontLayer.color = colors[0];
		colorStart = Time.time;
		colorChanging = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		Rect rect = frontLayer.uvRect;
		rect.x += 0.2f * Time.deltaTime;
		frontLayer.uvRect = rect;

		float end;
		if (colorChanging)
		{
			end = colorStart + 2.0f;
			float percent = 1 - ((end - Time.time) / 2.0f);

			int nextColor = actualColor + 1;
			if (nextColor == colors.Length)
				nextColor = 0;

			Color trans;
			if (percent < 1.0f) {
				trans = new Color();
				trans.a = colors[nextColor].a;
				trans.r = colors[actualColor].r + ( percent * (colors [nextColor].r - colors [actualColor].r));
				trans.g = colors[actualColor].g + ( percent * (colors [nextColor].g - colors [actualColor].g));
				trans.b = colors[actualColor].b + ( percent * (colors [nextColor].b - colors [actualColor].b));
			}
			else
			{
				trans = colors[nextColor];
				actualColor = nextColor;
				colorStart = Time.time;
				colorChanging = false;
			}

			frontLayer.color = trans;

		}
		else
		{
			end = colorStart + 4.0f;

			if (Time.time >= end)
			{
				colorStart = Time.time;
				colorChanging = true;
			}

		}
	}
}
