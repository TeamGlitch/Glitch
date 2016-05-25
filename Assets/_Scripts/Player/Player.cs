﻿using UnityEngine;
using System.Collections;


public class Player : MonoBehaviour {

    public AudioClip explosionSound;

	// External references
	public HUDLives guiLife;					//Reference to the Life GUI
	public HUDCollects guiItem;					//Reference to the items GUI
	public DeadMenuScript deadMenuScript;		//Reference to the death menu script
	public CheckPoint lastCheckPoint;			//Reference to the last checkpoint

	//Internal references
    private BoxCollider trigger;

	// Player element
	public PlayerController playerController;	//Reference to the player controller
    private SpriteRenderer sprite;

	// Prefabs
	public GameObject glitchPart;				//Reference to a glitch fragment created upon death

	// List
	private ObjectPool glitchPartPool;			//Fragments pool

	// Properties
	public float lives;							// Actual lives
	public int items = 0;						// Items collected

	// State
	private bool moveToCheckpoint = false;							// If it's moving to the last checkpoint
	private Vector3 speedToCheckpoint = new Vector3 (0,0,0);		// Speed vector to the checkpoint
	private float stopMoving;										// When to stop moving

	private int numberOfBoxesActivable = 0;
	public GameObject boxUIActivated;
	public Canvas gui;
	private RectTransform boxUIActivatedRectTransform;
	private RectTransform guiRectTrans;
    private bool lastLife = false;

	public delegate void PlayerDeadDelegate();
	public event PlayerDeadDelegate PlayerDeadEvent;

	private float correctionFactorForExclamation = -11.02155f;
	private Vector2 exclamationSize;

    private SlowFPS slowFPSScript;

    private float _timeLastEnemyHitted;

	void Awake () {
        _timeLastEnemyHitted = Time.time;
        trigger = GetComponentInChildren<BoxCollider>();
		sprite = transform.GetComponentInChildren<SpriteRenderer>();

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
		exclamationSize = boxUIActivatedRectTransform.sizeDelta;
		boxUIActivated.SetActive (false);
		guiRectTrans = gui.GetComponent<RectTransform>();
        slowFPSScript = transform.FindChild("Powers").GetComponentInChildren<SlowFPS>();
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

		if (numberOfBoxesActivable > 0) {

			float correction = Camera.main.transform.position.z / correctionFactorForExclamation;

			boxUIActivatedRectTransform.sizeDelta = new Vector2 (exclamationSize.x/correction, exclamationSize.y/correction);

			//Sight position
			Vector3 boxUIPosition = new Vector3(transform.position.x, transform.position.y + 2.0f, 0);
			Vector3 camPosition = Camera.main.WorldToScreenPoint(boxUIPosition);

			camPosition.x *= guiRectTrans.rect.width / Camera.main.pixelWidth; 
			camPosition.y *= guiRectTrans.rect.height / Camera.main.pixelHeight; 

			boxUIActivatedRectTransform.anchoredPosition = camPosition;

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
		if (PlayerDeadEvent != null) {
			PlayerDeadEvent ();
		}
		moveToCheckpoint = false;
		playerController.allowMovement = true;
		sprite.enabled = true;
        trigger.enabled = true;
        playerController.rigidBody.isKinematic = false;
		playerController.state = PlayerController.player_state.JUMPING;
		transform.position = lastCheckPoint.gameObject.transform.position;
        slowFPSScript.RestartCooldowns();
	}

	public void healCompletely()
    {
		lives = 3;
		guiLife.UpdateLives();
        slowFPSScript.RestartCooldowns();
	}

	public void IncreaseActivableBox()
	{
		if (numberOfBoxesActivable == 0) {
			boxUIActivated.SetActive (true);
		}
		++numberOfBoxesActivable;
	}

	public void DecreaseActivableBox()
	{
		--numberOfBoxesActivable;
		if (numberOfBoxesActivable == 0) {
			boxUIActivated.SetActive (false);
		}
	}

	public void IncreaseItem()
	{
		++items;
		guiItem.GUIItemRepresent ();
	}

	public void DecreaseItem()
	{
		--items;
		guiItem.GUIItemRepresent ();
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
        }

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

        guiLife.UpdateLives();

        if (lives % 1 == 0)
        {
            Death();
        }
    }

    public void Death()
    {

        SoundManager.instance.PlaySingle(explosionSound);

        //Set the character to dead and disable vSpeed
        playerController.state = PlayerController.player_state.DEATH;
        playerController.rigidBody.isKinematic = true;

        //Deactivate the sprite renderer
        sprite.enabled = false;
        trigger.enabled = false;

        slowFPSScript.DeactivatePower();

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
        if(Time.time - _timeLastEnemyHitted > 0.2f)
        {
            // To impulse player from enemy
            playerController.rigidBody.velocity = new Vector3(playerController.rigidBody.velocity.x, 0.0f, 0.0f);
            playerController.rigidBody.AddForce(new Vector3(0.0f, playerController.jumpForce * 2.0f, 0.0f));
            playerController.teleport.teleportUsed = false;
            _timeLastEnemyHitted = Time.time;
        }
    }
}
