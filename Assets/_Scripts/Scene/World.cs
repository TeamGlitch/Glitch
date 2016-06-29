using UnityEngine;
using System.Collections.Generic;
using InControl;

public class World : MonoBehaviour {

	public enum update_status
	{
		REGULAR_UPDATE,			//Just return the delta time
		WAITING,				//Don't do anything (doUpdate == false)
		UPDATE_REQUESTED,		//DoUpdate = true in the next Update cycle
		UPDATING				//DoUpdate returns to false
	};

	public PlayerController player;
    public GameObject mainCamera;
    public GameObject powers;
    public GameObject gui;
	public GameObject dialogues;
    public GameObject pauseMenu;

    private PauseScript pauseScript;
	private update_status state = update_status.REGULAR_UPDATE;

	// Moving objects look at this to know if they should move or not.
	// Changed by the slowfps power up. Can also be used for other pauses.
	public bool doUpdate = true;

	// How much can the world move in this update
	public float lag = 0.03f;

	void Start()
    {
		// We begin the game activating camera and movements of player
        mainCamera.SetActive(true);
        gui.SetActive(true);
		dialogues.SetActive(true);
        pauseMenu.SetActive(false);
        pauseScript = pauseMenu.GetComponent<PauseScript>();
        //Cursor.visible = false;
    }

	void Update(){

        if (InputManager.ActiveDevice.MenuWasPressed && pauseMenu.activeInHierarchy)
        {
            if(pauseScript.Unpause())
            pauseMenu.SetActive(false);
        }
        else if (InputManager.ActiveDevice.MenuWasPressed)
        {
            pauseMenu.SetActive(true);
            pauseScript.Pause();
        }


		switch (state) {

		case update_status.REGULAR_UPDATE:
			
			lag = Time.deltaTime;
			break;

		case update_status.UPDATE_REQUESTED:

			doUpdate = true;
			state = update_status.UPDATING;
			break;

		case update_status.UPDATING:
			doUpdate = false;
			state = update_status.WAITING;
			break;
		}
		
	}

	//When in SlowFPS, request to mark "doUpdate" as true in the next World update
	//inputLag is the lag this update will have
	public void requestUpdate(float inputLag){
		lag = inputLag;
		state = update_status.UPDATE_REQUESTED;
	}

    // Modifies fps of world (included enemies)
	public void toggleSlowFPS (){
		if (state != update_status.REGULAR_UPDATE) {
			state = update_status.REGULAR_UPDATE;
			doUpdate = true;
		} else {
			state = update_status.WAITING;
		}
	}
}
