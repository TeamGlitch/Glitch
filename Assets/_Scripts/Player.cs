using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	//External references
	public HUDLives guiLife;					//Reference to the Life GUI
	public HUDCollects guiItem;					//Reference to the items GUI
	public DeadMenuScript deadMenuScript;		//Reference to the death menu script

	public CheckPoint lastCheckPoint;			//Reference to the last checkpoint

	//Internal references
	private PlayerController playerController;	//Reference to the player controller

	//Prefabs
	public GameObject glitchPart;				//Reference to a glitch fragment created upon death

	//List
	private ObjectPool glitchPartPool;			//Fragments pool

	//Properties
	public int lives;							// Actual lives
	public int items = 0;						// Items collected

	//State
	private bool moveToCheckpoint = false;							// If it's moving to the last checkpoint
	private Vector3 speedToCheckpoint = new Vector3 (0,0,0);		// Speed vector to the checkpoint
	private float stopMoving;										// When to stop moving

	// Use this for initialization
	void Awake () {

		glitchPartPool = new ObjectPool(glitchPart);
		playerController = GetComponent<PlayerController>();
		lives = 3;

		//Instantiate the glitch fragments to avoid lag later in the game
		GameObject[] parts = new GameObject[100];
		for (int i = 0; i < 100; i++)
			parts[i] = glitchPartPool.getObject();
		for (int i = 0; i < 100; i++)
			parts[i].SetActive(false);
	}

	void OnTriggerEnter(Collider coll){

		//If there's a collision with death
		if(coll.CompareTag("Death")){

			//Decrement lives and update the GUI
			lives -= 1;
			guiLife.Decrementlives();

			//Set the character to dead and disable vSpeed
			playerController.state = PlayerController.player_state.DEATH;
			playerController.vSpeed = 0;

			//Determine if it's the last life
			bool lastLife = (lives > 0) ? false : true;

			//Restart the fragments
			for (int i = 0; i < 100; i++) {
				
				GameObject part = glitchPartPool.getObject();
				part.transform.position = transform.position;

				if (i == 0) {
					part.GetComponent<glitchFragment> ().restart (lastCheckPoint.gameObject.transform.position, lastLife, this);
				} else {
					part.GetComponent<glitchFragment> ().restart (lastCheckPoint.gameObject.transform.position, lastLife);
				}

			}

			//If it is the last life, activate the dead menu
			if(lastLife){
				deadMenuScript.gameObject.SetActive(true);
				deadMenuScript.PlayerDead();
			}

			//Deactivate the sprite renderer
			GetComponent<SpriteRenderer>().enabled = false;

		}
	}
		
	void Update(){
	
		//If it's moving to the checkpoint, do so until the time to move it's over
		if (moveToCheckpoint && Time.time < stopMoving)
			transform.position += speedToCheckpoint * Time.deltaTime;
	
	}
		
	public void moveToCheckPoint(){
		//Prepare to move to the last checkpoint
		moveToCheckpoint = true;
		speedToCheckpoint = ((lastCheckPoint.transform.position - transform.position) / 0.5f);
		stopMoving = Time.time + 0.5f;
	}

	public void Resurrected(){
		moveToCheckpoint = false;
		GetComponent<SpriteRenderer>().enabled = true;
		playerController.state = PlayerController.player_state.JUMPING;
		transform.position = lastCheckPoint.gameObject.transform.position;
	}

	public void healCompletely(){
		lives = 3;
		guiLife.IncrementLives();
	}
}
