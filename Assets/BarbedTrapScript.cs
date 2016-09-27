using UnityEngine;
using System.Collections;

public enum barbedTrapState
{
    RESTING,
    WAITING,
    JUMPING,
    ACTIVE,
    HIDING
};


public class BarbedTrapScript : MonoBehaviour {

    public World world;

    private barbedTrapState state;

    public float timeTrapWaitsInActivation = 1.0f;
    public float timeTrapWaitsInDeactivation = 2.0f;
    public float lerpTime = 1.0f;
    public Vector3 moveDistance = new Vector3(0f, 1.0f, 0f);

    public AudioClip trapSound;

    Vector3 startPosition;
    Vector3 endPosition;
    float currentLerpTime;

    private float timeToJump;
    private float timeToDeactivate;

    // Use this for initialization
    void Start()
    {
        startPosition = transform.position;
        endPosition = transform.position + moveDistance;
        currentLerpTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (world.doUpdate)
        {

            switch (state)
            {

                case barbedTrapState.WAITING:


                    if (Time.time >= timeToJump)
                    {
                        state = barbedTrapState.JUMPING;
                    }

                    break;

                case barbedTrapState.JUMPING:

                    currentLerpTime += world.lag;
                    float lerpPercentage = currentLerpTime / lerpTime;
                    if (lerpPercentage > 1.0f)
                    {
                        timeToDeactivate = Time.time + timeTrapWaitsInDeactivation;
                        state = barbedTrapState.ACTIVE;
                    }

                    transform.position = Vector3.Lerp(startPosition, endPosition, lerpPercentage);


                    break;

                case barbedTrapState.ACTIVE:

                    if (Time.time >= timeToDeactivate)
                    {
                        currentLerpTime = 0.0f;
                        state = barbedTrapState.HIDING;
                    }

                    break;

                case barbedTrapState.HIDING:

                    currentLerpTime += world.lag;
                    float lerpPercentageO = currentLerpTime / lerpTime;
                    if (lerpPercentageO > 1.0f)
                    {
                        currentLerpTime = 1.0f;
                        state = barbedTrapState.RESTING;
                    }

                    transform.position = Vector3.Lerp(endPosition, startPosition, lerpPercentageO);

                    break;

            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && state == barbedTrapState.RESTING)
        {
            timeToJump = Time.time + timeTrapWaitsInActivation;
            currentLerpTime = 0.0f;
            state = barbedTrapState.WAITING;
        }
    }
}
