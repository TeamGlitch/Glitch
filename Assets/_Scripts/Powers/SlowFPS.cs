using UnityEngine;
using System.Collections;
using InControl;

public class SlowFPS : MonoBehaviour {

	//Reference
	public World world;

	public float MAXTime = 10.0f;				//Max time the power can be active
	public float timeRemaining;					//Remaining time the power can be activated

	public float recoveryRate = 3.0f;			//Time it takes to make a recovery bump
    private float recoveryTime;					//Time to the next recovery bump

    private float timeCount;
	private float timePerFrame = 0.0f;

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
			if (world.slow == false)
			{
				world.slow = true;
			}
			else
			{        
				world.slow = false;
			}
		}

		// If power is enabled we check the time left and if is minus than 0, we disable the power
        if (world.slow == true)
        {
			timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0.0f)
            {
				timeRemaining = 0;
                world.slow = false;
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

	// This function is a timer that notify to a object update 
	// with a true or false in it's return. In the update of the 
	// slowed object we have to chech if this return is true or false, 
	// and if is false not update the object.
    public bool Slow(float time)
    {
        if (timePerFrame != time)
        {
            timePerFrame = time;
            timeCount = timePerFrame;
        }
        else
        {
            timeCount -= Time.deltaTime;
            if (timeCount < 0.0f)
            {
                timeCount = timePerFrame;
                return true;
            }
        }
        return false;
    }

}
