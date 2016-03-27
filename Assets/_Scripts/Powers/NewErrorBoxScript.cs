using UnityEngine;
using System.Collections;
using InControl;

public class NewErrorBoxScript : MonoBehaviour
{

	public float timeActive = 5.0f;
	public float timeFlickering = 1.0f;
	public int framesBeforeChangeStateWhenFlickering = 6;
	public Player playerScript;

	public GameObject boxUIActivated1;
	public GameObject boxUIDeactivated1;
	public GameObject boxUIActivated2;
	public GameObject boxUIDeactivated2;
	public Canvas gui;

	private int UIBoxUsed = 0;

	private RectTransform boxUIActivatedRectTransform1;
	private RectTransform boxUIDeactivatedRectTransform1;
	private RectTransform boxUIActivatedRectTransform2;
	private RectTransform boxUIDeactivatedRectTransform2;
	private RectTransform guiRectTrans;

	private int framesInCurrentStateWhenFlickering = 0;
	private float timeActivated = 0.0f;
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

		boxUIActivatedRectTransform1 = boxUIActivated1.GetComponent<RectTransform> ();
		boxUIActivated1.SetActive (false);
		boxUIDeactivatedRectTransform1 = boxUIDeactivated1.GetComponent<RectTransform> ();
		boxUIDeactivated1.SetActive (false);
		guiRectTrans = gui.GetComponent<RectTransform>();
		boxUIActivatedRectTransform2 = boxUIActivated2.GetComponent<RectTransform> ();
		boxUIActivated2.SetActive (false);
		boxUIDeactivatedRectTransform2 = boxUIDeactivated2.GetComponent<RectTransform> ();
		boxUIDeactivated2.SetActive (false);

	}

	// Update is called once per frame
	void Update ()
	{
		if (UIBoxUsed != 0) {
			Vector3 boxUIPosition = new Vector3(transform.position.x, transform.position.y + 4.0f, 0);
			Vector3 camPosition = Camera.main.WorldToScreenPoint(boxUIPosition);
			camPosition.x *= guiRectTrans.rect.width / Camera.main.pixelWidth; 
			camPosition.y *= guiRectTrans.rect.height / Camera.main.pixelHeight; 


			if (UIBoxUsed == 1) {
				boxUIActivatedRectTransform1.anchoredPosition = camPosition;
				boxUIDeactivatedRectTransform1.anchoredPosition = camPosition;
			} else if (UIBoxUsed == 2) {
				boxUIActivatedRectTransform2.anchoredPosition = camPosition;
				boxUIDeactivatedRectTransform2.anchoredPosition = camPosition;
			}

		}

		if (visible && !activated && InputManager.ActiveDevice.Action4.WasPressed)
		{
			playerScript.DecreaseActivableBox ();
			Color boxColor = spriteRenderer.color;
			boxColor.a = 1.0f;
			spriteRenderer.color = boxColor;
			boxCollider.enabled = true;
			timeActivated = 0.0f;
			activated = true;
			framesInCurrentStateWhenFlickering = 0;

			if (UIBoxUsed == 1) {
				boxUIActivated1.SetActive (false);
				boxUIDeactivated1.SetActive (true);
			} else if (UIBoxUsed == 2) {
				boxUIActivated2.SetActive (false);
				boxUIDeactivated2.SetActive (true);
			}
		}
		else if (activated && timeActivated < timeActive && timeActivated >= (timeActive - timeFlickering))
		{
			timeActivated += Time.deltaTime;

			Color boxColor = spriteRenderer.color;
			if (framesInCurrentStateWhenFlickering >= framesBeforeChangeStateWhenFlickering && boxColor.a == 1.0f) {
				framesInCurrentStateWhenFlickering = 0;
				boxColor.a = 0.6f;
			}
			else if (framesInCurrentStateWhenFlickering >= framesBeforeChangeStateWhenFlickering / 2 && boxColor.a != 1.0f) {
				framesInCurrentStateWhenFlickering = 0;
				boxColor.a = 1.0f;
			}
			spriteRenderer.color = boxColor;
			++framesInCurrentStateWhenFlickering;
		}
		else if (activated && timeActivated < timeActive)
		{
			timeActivated += Time.deltaTime;
		}
		else if (activated && visible)
		{
			Color boxColor = spriteRenderer.color;
			boxCollider.enabled = false;
			boxColor.a = 0.5f;
			spriteRenderer.color = boxColor;
			activated = false;
			playerScript.IncreaseActivableBox ();

			if (UIBoxUsed == 1) {
				boxUIActivated1.SetActive (true);
				boxUIDeactivated1.SetActive (false);
			} else if (UIBoxUsed == 2) {
				boxUIActivated2.SetActive (true);
				boxUIDeactivated2.SetActive (false);
			}

		}
		else if (activated)
		{
			Color boxColor = spriteRenderer.color;
			boxCollider.enabled = false;
			boxColor.a = 0.0f;
			spriteRenderer.color = boxColor;
			activated = false;

		}
	}

	void OnTriggerEnter (Collider other)
	{
		Vector3 boxUIPosition = new Vector3(transform.position.x, transform.position.y + 4.0f, 0);
		Vector3 camPosition = Camera.main.WorldToScreenPoint(boxUIPosition);
		camPosition.x *= guiRectTrans.rect.width / Camera.main.pixelWidth; 
		camPosition.y *= guiRectTrans.rect.height / Camera.main.pixelHeight; 

		if (!activated) {
			Color boxColor = spriteRenderer.color;
			boxColor.a = 0.5f;
			spriteRenderer.color = boxColor;
			playerScript.IncreaseActivableBox ();

			if (!boxUIActivated1.activeSelf && !boxUIDeactivated1.activeSelf) {
				UIBoxUsed = 1;
				boxUIActivated1.SetActive (true);

				boxUIActivatedRectTransform1.anchoredPosition = camPosition;
				boxUIDeactivatedRectTransform1.anchoredPosition = camPosition;
			} else {
				UIBoxUsed = 2;
				boxUIActivated2.SetActive (true);

				boxUIActivatedRectTransform2.anchoredPosition = camPosition;
				boxUIDeactivatedRectTransform2.anchoredPosition = camPosition;
			}
		} else {

			if (!boxUIActivated1.activeSelf && !boxUIDeactivated1.activeSelf) {
				UIBoxUsed = 1;
				boxUIDeactivated1.SetActive (true);

				boxUIActivatedRectTransform1.anchoredPosition = camPosition;
				boxUIDeactivatedRectTransform1.anchoredPosition = camPosition;
			} else {
				UIBoxUsed = 2;
				boxUIDeactivated2.SetActive (true);

				boxUIActivatedRectTransform2.anchoredPosition = camPosition;
				boxUIDeactivatedRectTransform2.anchoredPosition = camPosition;
			}
		}
		visible = true;
		playerScript.IncreaseVisibleBox ();

	}

	void OnTriggerExit (Collider other)
	{
		if (UIBoxUsed == 1) {
			boxUIActivated1.SetActive (false);
			boxUIDeactivated1.SetActive (false);
		} else if (UIBoxUsed == 2) {
			boxUIActivated2.SetActive (false);
			boxUIDeactivated2.SetActive (false);
		}

		UIBoxUsed = 0;

		if (!activated) {
			playerScript.DecreaseActivableBox ();
			boxCollider.enabled = false;
			Color boxColor = spriteRenderer.color;
			boxColor.a = 0.0f;
			spriteRenderer.color = boxColor;
		}
		visible = false;
		playerScript.DecreaseVisibleBox ();
	}
}
