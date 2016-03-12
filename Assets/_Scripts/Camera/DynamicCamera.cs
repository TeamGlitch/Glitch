﻿using UnityEngine;
using System.Collections;
using InControl;

public class DynamicCamera : MonoBehaviour {
    public Transform player;
    public Camera mainCamera;
    public World world;
    public GameObject titles;

    private int speed = 10;
    private float delay = 5.0f;
	private int offsetX = 7;
	private int zPosition = -20;

    void Update () {

		if (InputManager.ActiveDevice.AnyButton.IsPressed) {
			
			if (titles.activeSelf == true)
				titles.SetActive (false);

			transform.position = new Vector3(player.position.x + offsetX, transform.position.y, zPosition);
			beginGame ();

		} else if (transform.position.x <= (player.position.x + offsetX)) {
			
			// Camera freeze for a "delay" time to show title of level
			delay -= Time.deltaTime;

			if(titles.activeSelf != true)
				titles.SetActive(true);

			if (delay <= 0.0f)
			{
				// Camera do a zoom to player and the game begins
				titles.SetActive(false);
				transform.Translate(0.0f, 0.0f, speed*Time.deltaTime);
				if (transform.position.z >= zPosition)
				{
					beginGame();
				}
			}

		}
		else
		{
			// Camera moves to left until reach player
			transform.Translate((Time.deltaTime) * -speed, 0.0f, 0.0f);
		}

       
    }

	// Function that positions the main camera, active "World" (that active player
	// movements) and deactive this class.
    void beginGame()
    {
        mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);
        world.enabled = true;
        gameObject.SetActive(false);
    } 
}