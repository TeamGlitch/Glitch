using UnityEngine;
using System.Collections;
using InControl;

public class NewErrorBoxScript : MonoBehaviour
{

	public float timeActive = 5.0f;

	private BoxCollider boxCollider;
	private SpriteRenderer spriteRenderer;

	private bool activated = false;
	private bool visible = false;

	void Start()
	{
		spriteRenderer = transform.GetComponent<SpriteRenderer> ();
		boxCollider = transform.GetComponent<BoxCollider> ();
		boxCollider.enabled = false;
		Color boxColor = spriteRenderer.color;
		boxColor.a = 0.0f;
		spriteRenderer.color = boxColor;
	}

	// Update is called once per frame
	void Update ()
	{
		if (visible && InputManager.ActiveDevice.Action4.WasPressed)
		{
			Color boxColor = spriteRenderer.color;
			boxColor.a = 1.0f;
			spriteRenderer.color = boxColor;
			boxCollider.enabled = true;
			timeActive = 0.0f;
			activated = true;
		}
		else if (activated && visible && timeActive < 5.0f)
		{
			timeActive += Time.deltaTime;
		}
		else if (activated && visible)
		{
			Color boxColor = spriteRenderer.color;
			boxCollider.enabled = false;
			boxColor.a = 0.5f;
			spriteRenderer.color = boxColor;
			activated = false;
		}
	}

	void OnTriggerEnter (Collider other)
	{
		visible = true;
		Color boxColor = spriteRenderer.color;
		boxColor.a = 0.5f;
		spriteRenderer.color = boxColor;
	}

	void OnTriggerExit (Collider other)
	{
		boxCollider.enabled = false;
		Color boxColor = spriteRenderer.color;
		boxColor.a = 0.0f;
		spriteRenderer.color = boxColor;
		visible = false;
		activated = false;
	}
}
