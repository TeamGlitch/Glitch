using UnityEngine;
using System.Collections;

public class MovingStoneScript : MonoBehaviour {

	public Vector3 distanceToMove = new Vector3 (0.0f, -5.0f, 0.0f);
	public float lerpTime = 1.0f;
	public World world;

	Vector3 startPosition;
	Vector3 endPosition;
	float currentLerpTime;

	// Use this for initialization
	void Start () {
		startPosition = transform.position;
		endPosition = transform.position + distanceToMove;
		currentLerpTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (world.doUpdate)
		{
			currentLerpTime += world.lag;

			if (currentLerpTime > lerpTime)
				currentLerpTime = lerpTime;

			float t = currentLerpTime / lerpTime;
			t = t * t * t * (t * (6.0f * t - 15.0f) + 10.0f);
			transform.position = Vector3.Lerp(startPosition, endPosition, t);

			if (currentLerpTime == lerpTime)
			{
				currentLerpTime = 0.0f;
				Vector3 aux = startPosition;
				startPosition = endPosition;
				endPosition = aux;
			}
		}
	}

	void OnTriggerEnter (Collider collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			collision.gameObject.transform.parent = transform;
		}
	}

	void OnTriggerExit (Collider collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			transform.DetachChildren();
		}
	}

}


// 932530910
