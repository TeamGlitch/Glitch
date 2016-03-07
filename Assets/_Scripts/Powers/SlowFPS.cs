﻿using UnityEngine;
using System.Collections;
using InControl;

public class SlowFPS : MonoBehaviour {

	//Reference
	public World world;

	private bool powerActive = false;

	public float timeBetweenUpdates = 1;		//Seconds between slow updates
	private float timeLastUpdate = 0;			//When the last slow update was done

	public float MAXTime = 10.0f;				//Max time the power can be active
	public float timeRemaining;					//Remaining time the power can be active

	public float recoveryRate = 3.0f;			//Time it takes to make a recovery bump
    private float recoveryTime;					//Time to the next recovery bump

    void Start()
    {
        // Begin with 10 seconds and the cooldown time for each second are 3 seconds
		timeRemaining = MAXTime;
		recoveryTime = recoveryRate;
    }

	// In the update we control the available time of the power and it's recovery
    void Update()
    {
		if (InputManager.ActiveDevice.LeftBumper.WasPressed)
		{
			if (powerActive == false)
			{
				powerActive = true;
				world.doUpdate = false;
				world.toggleSlowFPS();
			}
			else
			{        
				deactivatePower();
			}
		}

		// If power is enabled we check the time left and if is minus than 0, we disable the power
		if (powerActive)
        {
			timeRemaining -= Time.deltaTime;

			if (timeRemaining <= 0.0f)
			{
				timeRemaining = 0;
				deactivatePower ();

			} else {

				if (world.doUpdate == true) {
					world.doUpdate = false;
					timeLastUpdate = Time.time;
				} else if (Time.time >= timeLastUpdate + timeBetweenUpdates) {
					world.doUpdate = true;
				}

			}
        }
        else
        {
			// This is to check the recovery time. 3 seconds gives us one second of power
			if (timeRemaining < MAXTime)
            {
                recoveryTime -= Time.deltaTime;

                if (recoveryTime <= 0.0f)
                {
                    timeRemaining += 1.0f;

					if (timeRemaining > MAXTime)
					{
						timeRemaining = MAXTime;
					}

					recoveryTime = recoveryRate;

                }
            }
        }
    }

	private void deactivatePower(){
		powerActive = false;
		world.doUpdate = true;
		world.toggleSlowFPS();
	}
}
