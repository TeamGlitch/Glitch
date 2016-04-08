using UnityEngine;
using System.Collections.Generic;

public class World : MonoBehaviour {

	// Moving objects look at this to know if they should move or not.
	// Changed by the slowfps power up. Can also be used for other pauses.
	public bool doUpdate = true;
	public float slowDown = 1;			//This is intended to be a global slowdown scale during slowfps
	public PlayerController player;
    public Camera mainCamera;
    public GameObject powers;
    public GameObject gui;

	private List<GameObject> gameObjectList;

	void Start()
    {
		// We begin the game activating camera and movements of player
        mainCamera.gameObject.SetActive(true);
        gui.SetActive(true);
        powers.SetActive(true);
        player.enabled = true;
		gameObjectList = new List<GameObject> ();
    }
	
    // Modifies fps of world (included enemies)
	public void toggleSlowFPS (){
		if (slowDown == 1) {
			slowDown = 0.5f;
		} else {
			slowDown = 1.0f;
		}
	}
}
