using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GlowBoxCreatorUI : MonoBehaviour {

	private RectTransform maskTransform;
	private Image image;

	private float startTime;
	private float endTime;

	// Use this for initialization
	void Awake () {
		maskTransform = GetComponent<RectTransform>();
		image = GetComponent<Image>();
	}

	public void StartMovement(){
		startTime = Time.time;
		endTime = Time.time + 0.5f;
	}
	
	// Update is called once per frame
	void Update () {
		float percent = (Time.time - startTime) / (endTime - startTime);

		if (percent > 1.0f )
			percent = 1.0f;

		maskTransform.localScale = new Vector3 ( 1.0f + (1.0f * percent) , 1.0f + (1.0f * percent), 1);
		image.color = new Color(image.color.r, image.color.g, image.color.b, (1 - percent));

		if (percent == 1.0f)
			gameObject.SetActive(false);
	}
}
