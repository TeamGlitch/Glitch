using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BoxCreatorUI : MonoBehaviour {
	
	public RectTransform[] masksPositions = new RectTransform[3];
	public RectTransform[] unactivatedImagePositions = new RectTransform[3];

	public RectTransform[] bloomPartsPosition = new RectTransform[3];
	private Image[] bloomPartsImages = new Image[3];

	public float startYPos = 3;
	public float endYPos = 33;

	private bool filling = false;
	private bool blooming = false;

	//Active nodes
	private int activeNodes = 0;

	//Start and end times (different so they can be paralel)
	private float fillStartTime = 0;
	private float fillEndTime = 0;
	private float bloomStartTime = 0;
	private float bloomEndTime = 0;

	void Awake(){
		
		//Gets the bloom images, disables them and disables the mask parts
		for (int i = 0; i < 3; i++)
		{
			bloomPartsImages[i] = bloomPartsPosition[i].gameObject.GetComponent<Image>();
			bloomPartsImages [i].enabled = false;
			masksPositions[i].gameObject.SetActive(false);
		}
	}

	void Update(){

		if (filling) {

			//Calculate how much of the phase has passed
			float percentF = (Time.time - fillStartTime) / (fillEndTime - fillStartTime);

			if (percentF < 1.0f) {

				//Image movement
				float newY = startYPos + (percentF * (endYPos - startYPos));

				//Update active markers
				for (int i = 0; i < activeNodes; i++) {
					masksPositions [i].anchoredPosition = new Vector2 (0, newY);
					unactivatedImagePositions [i].anchoredPosition = new Vector2 (0, -newY);
				}
			
			} else {

				//Start blooming, hide the masks and
				//show the bloom images
				filling = false;
				activeNodes = 0;

				blooming = true;
				bloomStartTime = Time.time;
				bloomEndTime = bloomStartTime + 0.5f;


				for (int i = 0; i < activeNodes; i++) {
					masksPositions [i].gameObject.SetActive (false);
				}
				for (int i = 0; i < 3; i++) {
					bloomPartsImages [i].enabled = true;
				}

			}
		}

		if (blooming) {

			float percentB = (Time.time - bloomStartTime) / (bloomEndTime - bloomStartTime);
		
			if (percentB < 1.0f) {

				//Change alpha and scale
				Color newColor = bloomPartsImages [0].color;
				newColor.a = (1 - percentB);

				for (int i = 0; i < 3; i++) {
					bloomPartsPosition [i].localScale = new Vector3 (1.0f + (1.0f * percentB), 1.0f + (1.0f * percentB), 1);
					bloomPartsImages [i].color = newColor;
				}

			} else {
				
				blooming = false;
				for (int i = 0; i < 3; i++) {
					bloomPartsImages[i].enabled = false;
				}

			}
		
		}
	}

	//Called when a box was just used
	public void boxUsed(int index, float endTime){

		if (activeNodes < 3) {
			
			masksPositions[activeNodes].gameObject.SetActive(true);
			fillEndTime = endTime;
			activeNodes++;

			if (!filling) {
				filling = true;
				fillStartTime = Time.time;
			}

		}
	}

}
