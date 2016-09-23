using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialClouds : MonoBehaviour {

    public RawImage backLayer;
    public RawImage frontLayer;
	public Color[] colors;

	private int actualColor = 0;
	private float colorStart;
	private bool colorChanging;

    private bool extending = true;
    private float timeExtendingStarted = 0;

	void Start(){
		actualColor = 0;
		frontLayer.color = colors[0];
		colorStart = Time.time;
		colorChanging = false;
	}
	
	// Update is called once per frame
	void Update () {

		Rect rect = frontLayer.uvRect;

        float percent = (Time.time - timeExtendingStarted) / 15.0f;


        if (extending)
        {
            if (percent > 1)
            {
                percent = 1;
                timeExtendingStarted = Time.time;
                extending = false;
            }

            float value = (1.5227f * Mathf.Pow(percent, 3)) - (2.284f * Mathf.Pow(percent, 2)) + (0.2113f * percent) + 1;
            rect.height = value;
            rect.width = value;

        }
        else
        {

            if (percent > 1)
            {
                percent = 1;
                timeExtendingStarted = Time.time;
                extending = true;
            }

            percent = 1 - percent;
            float value = (1.5227f * Mathf.Pow(percent, 3)) - (2.284f * Mathf.Pow(percent, 2)) + (0.2113f * percent) + 1;
            rect.height = value;
            rect.width = value;
        }
		rect.x += 0.1f * Time.deltaTime;
		frontLayer.uvRect = rect;

        rect = backLayer.uvRect;
        rect.x -= 0.05f * Time.deltaTime;
        backLayer.uvRect = rect;

		float end;
		if (colorChanging)
		{
			end = colorStart + 2.0f;
			percent = 1 - ((end - Time.time) / 2.0f);

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
