using UnityEngine;
using System.Collections;

public class ErrorBoxScript : MonoBehaviour {

	public ErrorBoxCreator errorBoxCreator;
	public float startTime = -1;
	public float duration = -1;
	public float cooldown = -1;

	private bool active = true;
	private SpriteRenderer spriteRenderer = null;
	private BoxCollider boxCollider = null;

	void Start(){
		spriteRenderer = GetComponent<SpriteRenderer>();
		boxCollider = GetComponent<BoxCollider>();
	}

	// Update is called once per frame
	void Update () {
		if (startTime != -1) {
			if (active && (Time.time - startTime >= duration)) {
				spriteRenderer.enabled = false;
				boxCollider.enabled = false;
				active = false;
			} else if (!active && (Time.time - startTime >= cooldown)) {
				errorBoxCreator.errorBoxDeleted(1);
				gameObject.SetActive(false);
			}

		}
	}

	//When is disabled, enable the sprite renderer and box collider
	//to be ready if the object pool calls it again
	void OnDisable(){
		active = true;
		if(spriteRenderer != null) spriteRenderer.enabled = true;
		if(boxCollider != null) boxCollider.enabled = true;
	}

}
