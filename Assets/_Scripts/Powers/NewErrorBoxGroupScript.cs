using UnityEngine;
using System.Collections;
using InControl;

public class NewErrorBoxGroupScript : MonoBehaviour {

	public float distanceToActive = 10.0f;
	public float timeActive = 5.0f;

	private struct BoxModificator {
//		public GameObject boxObject;
		public BoxCollider boxCollider;
		public SpriteRenderer spriteRenderer;
	}

	private BoxModificator[] boxesArray;
	private bool activated = false;
	private bool visible = false;

	// Use this for initialization
	void Start () {
		Color colorAux;
		boxesArray = new BoxModificator[transform.childCount];
		for (int i = 0; i < boxesArray.Length; ++i)
		{
//			boxesArray [i].boxObject = transform.GetChild (i);
			boxesArray [i].boxCollider = transform.GetChild (i).transform.GetComponent<BoxCollider> ();
			boxesArray [i].boxCollider.enabled = false;
			boxesArray [i].spriteRenderer = transform.GetChild (i).transform.GetComponent<SpriteRenderer> ();
			colorAux = boxesArray [i].spriteRenderer.color;
			colorAux.a = 0.0f;
			boxesArray [i].spriteRenderer.color = colorAux;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (visible && InputManager.ActiveDevice.Action4.WasPressed)
		{
			Color boxColor;
			for (int i = 0; i < boxesArray.Length; ++i)
			{
				boxColor = boxesArray [i].spriteRenderer.color;
				boxColor.a = 1.0f;
				boxesArray [i].boxCollider.enabled = true;
				timeActive = 0.0f;
				boxesArray [i].spriteRenderer.color = boxColor;

			}
			timeActive = 0.0f;
			activated = true;
		}
		else if (activated && visible && timeActive < 5.0f)
		{
			timeActive += Time.deltaTime;
		}
		else if (activated && visible)
		{
			Color boxColor;
			for (int i = 0; i < boxesArray.Length; ++i) {
				boxColor = boxesArray [i].spriteRenderer.color;
				boxesArray [i].boxCollider.enabled = false;
				boxColor.a = 0.5f;
				boxesArray [i].spriteRenderer.color = boxColor;
			}
			activated = false;
		}

	}

	void OnTriggerEnter (Collider other)
	{
		Debug.Log ("ENTER");
		visible = true;
		Color boxColor;
		for (int i = 0; i < boxesArray.Length; ++i) {
			boxColor = boxesArray [i].spriteRenderer.color;
			boxColor.a = 0.5f;
			boxesArray [i].spriteRenderer.color = boxColor;
		}
	}

	void OnTriggerExit (Collider other)
	{
		Debug.Log ("EXIT");
		Color boxColor;
		for (int i = 0; i < boxesArray.Length; ++i) {
			boxesArray [i].boxCollider.enabled = false;
			boxColor = boxesArray [i].spriteRenderer.color;
			boxColor.a = 0.0f;
			boxesArray [i].spriteRenderer.color = boxColor;
		}
		visible = false;
		activated = false;
	}


}
