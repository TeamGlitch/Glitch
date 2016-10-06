using UnityEngine;
using System.Collections;

public class Pirana : MonoBehaviour
{

    public World world;

    private bool jumping;

    public float jumpAltitude = 10f;
    public float jumpTime = 2.5f;
    public float waitingTimeToAddForce = 10.0f;
    public float possibleVariation = 1.0f;
    public float changingZ = 0.4f;

    private float initialZ;
    private float initialY;

    private float jumpStart = 0f;
    private float lastUpdateTime = 0f;
    private float nextJump;


    // Use this for initialization
    void Start()
    {
        initialZ = transform.position.z;
        initialY = transform.position.y;
        jumpStart = Time.time;
        lastUpdateTime = Time.time;
        jumping = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (world.doUpdate)
        {

            //If we're jumping
            if (jumping)
            {

                //We calculate where in the animation we are
                //The actual time is lasUpdateTime + world.lag, witch is .deltaTime (elapsed time
                //since last update) in normal conditions and the time passed with slowdown when
                //slowFPS are activated
                float timePassed = ((lastUpdateTime + world.lag) - jumpStart) / jumpTime;
                lastUpdateTime = Time.time;

                //If the animation is complete, deactivate the jumping state
                //and prepare the next jump
                if (timePassed >= 1)
                {
                    timePassed = 0;
                    jumping = false;
                    nextJump = Time.time + Random.Range(waitingTimeToAddForce - possibleVariation, waitingTimeToAddForce + possibleVariation);
                }

                //Does the function
                //T * sin^2(((x-x^2)*T)/PI)*2
                //T = Highest altitude
                //x = Time elapsed (0->1)
                //that does a up-down movement with smoothing at the top
                Vector3 newPos = transform.position;
                Vector3 newRot = new Vector3(270f, 0, 90f);

                float calc = jumpAltitude * (timePassed - 1) * timePassed;
                calc /= Mathf.PI;
                newPos.y = initialY + (2 * jumpAltitude * Mathf.Pow(Mathf.Sin(calc), 2));
                newPos.z = Mathf.Lerp(initialZ, initialZ + changingZ, timePassed);

                newRot.x = Mathf.Lerp(270f, 90f, timePassed);

                transform.position = newPos;

                //    Debug.Log(transform.eulerAngles);

                transform.eulerAngles = newRot;

            }
            //If we're not jumping but the time to jump has come
            else if (Time.time > nextJump)
            {
                jumpStart = Time.time;
                lastUpdateTime = Time.time;
                jumping = true;
            }
        }
    }

}
