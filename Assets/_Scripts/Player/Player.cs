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
    private BoxCollider trigger;

	// Player element
	public PlayerController playerController;	//Reference to the player controller
    public SpriteRenderer sprite;

	// Prefabs
	public GameObject glitchPart;				//Reference to a glitch fragment created upon death

	// List
	private ObjectPool glitchPartPool;			//Fragments pool

	// Properties
	public float lives;							// Actual lives
	public int items;						// Items collected

	// State
	private bool moveToCheckpoint = false;							// If it's moving to the last checkpoint
	private Vector3 speedToCheckpoint = new Vector3 (0,0,0);		// Speed vector to the checkpoint
	private float stopMoving;										// When to stop moving
    private bool lastLife = false;


	void Awake () {

		characterController = GetComponent<CharacterController>();
        trigger = GetComponent<BoxCollider>();

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
	}

	void OnTriggerEnter(Collider coll){

		//If there's a collision with some lethal thing in scene
		if(coll.gameObject.CompareTag("Death"))
        {
            DecrementLives(1);
        }
	}
		
	void Update(){
	
		//If it's moving to the checkpoint, do so until the time to move it's over
        if (moveToCheckpoint && (Time.time < stopMoving))
        {
            transform.position += speedToCheckpoint * Time.deltaTime;
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
        trigger.enabled = true;
		playerController.state = PlayerController.player_state.JUMPING;
		transform.position = lastCheckPoint.gameObject.transform.position;
	}

	public void healCompletely()
    {
		lives = 3;
		guiLife.UpdateLives();
	}

    //Decrement lives and update the GUI
    public void DecrementLives(float damage)
    {
        if ((lives % 1 == 0) || ((lives - damage) > Mathf.FloorToInt(lives)))
        {
            lives -= damage;
        }
        else
        {
            lives = Mathf.FloorToInt(lives);
            if (lives <= 0)
            {
                lives = 0;
                lastLife = true;
            }

            //If it is the last life, activate the dead menu
            if (lastLife)
            {
                Death();
                deadMenuScript.gameObject.SetActive(true);
                deadMenuScript.PlayerDead();
            }
        }
 
        guiLife.UpdateLives();

        if (lives % 1 == 0)
        {
            Death();
        }
    }

    public void Death()
    {
        //Set the character to dead and disable vSpeed
        playerController.state = PlayerController.player_state.DEATH;
        playerController.vSpeed = 0;

        //Deactivate the sprite renderer
        sprite.enabled = false;
        characterController.detectCollisions = false;
        trigger.enabled = false;

        //Restart the fragments
        for (int i = 0; i < 100; i++)
        {
            GameObject part = glitchPartPool.getObject();
            part.transform.position = transform.position;

            if (i == 0)
            {
                part.GetComponent<glitchFragment>().restart(lastCheckPoint.gameObject.transform.position, lastLife, this);
            }
            else
            {
                part.GetComponent<glitchFragment>().restart(lastCheckPoint.gameObject.transform.position, lastLife);
            }
        }
    }

    public void ReactToAttack(float enemyX)
    {
        // To impulse player from enemy
        if (enemyX > transform.position.x)
        {
            transform.Translate(-5.0f, 0.0f, 0.0f);
        }
        else
        {
            transform.Translate(5.0f, 0.0f, 0.0f);
        }
    }
}
