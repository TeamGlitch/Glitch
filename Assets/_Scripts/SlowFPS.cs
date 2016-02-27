using UnityEngine;
using System.Collections;

public class SlowFPS : MonoBehaviour {

    private float MAXTime;
    private float recoveryTime;
    private float timePerFrame = 0.0f;
    private float timeCount;
    public PlayerController player;

    void Start()
    {
        // Begin with 10 seconds and the cooldown time for each second are 3 seconds
        MAXTime = 10.0f;
        recoveryTime = 3.0f;
    }

	// In the update we control the available time of the power and it's recovery
    void Update()
    {
		// If power is enabled we check the time left and if is minus than 0, we disable the power
        if (player.world.slow == true)
        {
            if (MAXTime > 0.0f)
            {
                MAXTime -= Time.deltaTime;
            }
            else
            {
                player.world.slow = false;
            }
        }
        else
        {
			// This is to check the recovery time. 3 seconds gives us one second of power
            if (MAXTime < 10.0f)
            {
                recoveryTime -= Time.deltaTime;
                if (recoveryTime <= 0.0f)
                {
                    MAXTime += 1.0f;
                    recoveryTime = 3.0f;
                    if (MAXTime > 10.0f)
                    {
                        MAXTime = 10.0f;
                    }
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
