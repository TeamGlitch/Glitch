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
        // Begin with 10 seconds
        MAXTime = 10.0f;
        recoveryTime = 3.0f;
    }

    void Update()
    {
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
