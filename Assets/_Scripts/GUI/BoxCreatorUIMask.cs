using UnityEngine;
using System.Collections;

public class BoxCreatorUIMask : MonoBehaviour {

	public float startYPos;
	public float endYPos;

	private float startTime;
	private float endTime;

	private RectTransform maskTransform;
	private RectTransform childTransform;
	private GlowBoxCreatorUI glow;

	void Start(){
		maskTransform = GetComponent<RectTransform>();
		childTransform = transform.GetChild(0).GetComponent<RectTransform>();
		glow = transform.parent.Find("Glow").GetComponent<GlowBoxCreatorUI>();
		gameObject.SetActive(false);
	}

	public void StartMovement(float iEndTime){
		endTime = iEndTime;
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		float percent = (Time.time - startTime) / (endTime - startTime);

		if (percent > 1.0f )
			percent = 1.0f;

		float newY = startYPos + (percent * (endYPos - startYPos));

		maskTransform.anchoredPosition = new Vector2 (0, newY);
		childTransform.anchoredPosition = new Vector2 (0, -newY);

		if (percent == 1.0f) {
			glow.gameObject.SetActive(true);
			glow.StartMovement();
			gameObject.SetActive(false);
		}
	}
}
