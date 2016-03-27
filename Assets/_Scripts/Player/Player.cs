using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	// External references
	public HUDLives guiLife;					//Reference to the Life GUI
	public HUDCollects guiItem;					//Reference to the items GUI
	public DeadMenuScript deadMenuScript;		//Reference to the death menu script
	public CheckPoint lastCheckPoint;			//Reference to the last checkpoint

	//Internal references
	private CharacterController characterController;

	// Player element
	public PlayerController playerController;	//Reference to the player controller
    public SpriteRenderer sprite;

	// Prefabs
	public GameObject glitchPart;				//Reference to a glitch fragment created upon death

	// List
	private ObjectPool glitchPartPool;			//Fragments pool

	// Properties
	public int lives;							// Actual lives
	public int items;						// Items collected

	// State
	private bool moveToCheckpoint = false;							// If it's moving to the last checkpoint
	private Vector3 speedToCheckpoint = new Vector3 (0,0,0);		// Speed vector to the checkpoint
	private float stopMoving;										// When to stop moving

	private int numberOfBoxesActivable = 0;
	private int numberOfBoxesVisible = 0;
	public GameObject boxUIActivated;
	public GameObject boxUIDeactivated;
	public Canvas gui;
	private RectTransform boxUIActivatedRectTransform;
	private RectTransform boxUIDeactivatedRectTransform;
	private RectTransform guiRectTrans;



	void Awake () {

		characterController = GetComponent<CharacterController>();

		glitchPartPool = new ObjectPool(glitchPart);
		lives = 3;
        items = 0;

		//Instantiate the glitch fragments to avoid lag later in the game
		GameObject[] parts = new GameObject[100];
		for (int i = 0; i < 100; i++) {
			parts [i] = glitchPartPool.getObject ();
		}
		for (int i = 0; i < 100; i++) {
            parts[i].SetActive(false);
        }

		boxUIActivatedRectTransform = boxUIActivated.GetComponent<RectTransform> ();
		boxUIActivated.SetActive (false);
		boxUIDeactivatedRectTransform = boxUIDeactivated.GetComponent<RectTransform> ();
		boxUIDeactivated.SetActive (false);
		guiRectTrans = gui.GetComponent<RectTransform>();

	}

	void OnTriggerEnter(Collider coll){

		//If there's a collision with death
		if(coll.CompareTag("Death"))
        {
			//Decrement lives and update the GUI
			lives -= 1;
			guiLife.UpdateLives();

			//Set the character to dead and disable vSpeed
			playerController.state = PlayerController.player_state.DEATH;
			playerController.vSpeed = 0;

			//Determine if it's the last life
            bool lastLife = false;
            if (lives == 0)
            {
                lastLife = true;
            }

			//Restart the fragments
			for (int i = 0; i < 100; i++) 
            {		
				GameObject part = glitchPartPool.getObject();
				part.transform.position = transform.position;

				if (i == 0) 
                {
					part.GetComponent<glitchFragment> ().restart (lastCheckPoint.gameObject.transform.position, lastLife, this);
				} 
                else 
                {
					part.GetComponent<glitchFragment> ().restart (lastCheckPoint.gameObject.transform.position, lastLife);
				}
			}

			//If it is the last life, activate the dead menu
			if(lastLife)
            {
				deadMenuScript.gameObject.SetActive(true);
				deadMenuScript.PlayerDead();
			}

			//Deactivate the sprite renderer
			sprite.enabled = false;
			characterController.detectCollisions = false;

		}
	}
		
	void Update(){
	
		//If it's moving to the checkpoint, do so until the time to move it's over
        if (moveToCheckpoint && (Time.time < stopMoving))
        {
            transform.position += speedToCheckpoint * Time.deltaTime;
        }

		if (numberOfBoxesActivable > 0 || numberOfBoxesVisible > 0) {
			//Sight position
			Vector3 boxUIPosition = new Vector3(transform.position.x, transform.position.y + 4.0f, 0);
			Vector3 camPosition = Camera.main.WorldToScreenPoint(boxUIPosition);

			camPosition.x *= guiRectTrans.rect.width / Camera.main.pixelWidth; 
			camPosition.y *= guiRectTrans.rect.height / Camera.main.pixelHeight; 

			boxUIActivatedRectTransform.anchoredPosition = camPosition;
			boxUIDeactivatedRectTransform.anchoredPosition = camPosition;

		}
	
	}
		
	public void moveToCheckPoint()
    {
		//Prepare to move to the last checkpoint
		moveToCheckpoint = true;
		speedToCheckpoint = ((lastCheckPoint.transform.position - transform.position) / 0.5f);
		stopMoving = Time.time + 0.5f;
	}

	public void Resurrected()
    {
		moveToCheckpoint = false;
		sprite.enabled = true;
		characterController.detectCollisions = true;
		playerController.state = PlayerController.player_state.JUMPING;
		transform.position = lastCheckPoint.gameObject.transform.position;
	}

	public void healCompletely()
    {
		lives = 3;
		guiLife.UpdateLives();
	}

	public void IncreaseActivableBox()
	{
/*		if (numberOfBoxesActivable == 0) {
			boxUIActivated.SetActive (true);
			boxUIDeactivated.SetActive (false);
		}
		++numberOfBoxesActivable;
*/	}

	public void DecreaseActivableBox()
	{
		/*		--numberOfBoxesActivable;
		if (numberOfBoxesActivable == 0 && numberOfBoxesVisible == 0) {
			boxUIActivated.SetActive (false);
			boxUIDeactivated.SetActive (false);
		} else if (numberOfBoxesActivable == 0) {
			boxUIActivated.SetActive (false);
			boxUIDeactivated.SetActive (true);
		}
*/	}

	public void IncreaseVisibleBox()
	{
/*		if (numberOfBoxesVisible == 0 && numberOfBoxesActivable == 0) {
			boxUIDeactivated.SetActive (true);
		}
		++numberOfBoxesVisible;
*/	}

	public void DecreaseVisibleBox()
	{
/*		--numberOfBoxesVisible;
		if (numberOfBoxesVisible == 0) {
			boxUIDeactivated.SetActive (false);
		}
*/	}
}
