using UnityEngine;
using System.Collections;

public class BoxCreatorUIMask : MonoBehaviour {

	public float startYPos;
	public float endYPos;
    public RectTransform maskTransform;
    public RectTransform childTransform;
    public GlowBoxCreatorUI glow;

	private float startTime;
	private float endTime;
    private float percent;
    private float newY;

	public void StartMovement(float iEndTime){
		endTime = iEndTime;
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		percent = (Time.time - startTime) / (endTime - startTime);

        if (percent > 1.0f)
        {
            percent = 1.0f;
        }

		newY = startYPos + (percent * (endYPos - startYPos));

		maskTransform.anchoredPosition = new Vector2 (0, newY);
		childTransform.anchoredPosition = new Vector2 (0, -newY);

		if (percent == 1.0f) 
        {
			glow.gameObject.SetActive(true);
			glow.StartMovement();
			gameObject.SetActive(false);
		}
	}
}
