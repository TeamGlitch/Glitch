using UnityEngine;
using System.Collections;

public class BatMovement : MonoBehaviour {

    private bool goingRight;

    private Quaternion rotation;
    public Vector3 radius = new Vector3(5f, 0f, 0f);
    public float speed = 100f;
    private Vector3 initPos;
    float currentRotation = 0f;

    private bool goingUp = true;
    private float upDistance = 5f;
    private float timeGoingUp = 0f;
    public float timeToGoUp = 3f;
    private float initPosY;

    public World world;


    // Use this for initialization
    void Start () {
        goingRight = true;
        Vector3 pos = transform.position;
        pos.y -= upDistance / 2f;
        transform.position = pos;
        radius = new Vector3(5f, 0f, 0f);
        initPos = transform.position;
        initPosY = initPos.y;
    }
	
	// Update is called once per frame
	void Update () {
        if (world.doUpdate)
        {

            if (goingRight)
            {
                currentRotation += world.lag * 100;
                rotation.eulerAngles = new Vector3(0, currentRotation, 0);
                transform.position = rotation * radius + initPos + radius;
                transform.rotation = rotation;
                if (currentRotation >= 360f + 180f)
                {
                    currentRotation = 0f;
                    goingRight = false;
                }
            }
            else
            {
                currentRotation -= world.lag * speed;
                rotation.eulerAngles = new Vector3(0, currentRotation, 0);
                transform.position = rotation * radius + initPos - radius;
                rotation.eulerAngles = new Vector3(0, currentRotation - 180f, 0);
                transform.rotation = rotation;
                if (currentRotation <= -360f)
                {
                    currentRotation = 180f;
                    goingRight = true;
                }
            }
            if (goingUp)
            {
                timeGoingUp += world.lag;
                if (timeGoingUp >= timeToGoUp)
                    timeGoingUp = timeToGoUp;
                Vector3 pos = transform.position;
                pos.y = Mathf.Lerp(initPosY, initPosY + upDistance, timeGoingUp / timeToGoUp);
                transform.position = pos;
                if (timeGoingUp == timeToGoUp)
                {
                    timeGoingUp = 0f;
                    goingUp = false;
                    initPosY = transform.position.y;
                }
            }
            else
            {
                timeGoingUp += world.lag;
                if (timeGoingUp >= timeToGoUp)
                    timeGoingUp = timeToGoUp;
                Vector3 pos = transform.position;
                pos.y = Mathf.Lerp(initPosY, initPosY - upDistance, timeGoingUp / timeToGoUp);
                transform.position = pos;
                if (timeGoingUp == timeToGoUp)
                {
                    timeGoingUp = 0f;
                    goingUp = true;
                    initPosY = transform.position.y;
                }
            }
        }
    }
}
