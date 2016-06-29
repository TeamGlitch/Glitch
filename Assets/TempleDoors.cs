using UnityEngine;
using System.Collections;

public class TempleDoors : MonoBehaviour {

    public float timeToGoUp = 1f;
    public float timeToGoDown = 0.25f;
    public float timeWaiting = 2.0f;

    public float distance = 0.07f;
    private Vector3 initialPosition;

    public World world;

    private BoxCollider boxCollider;

    private enum door_state
    {
        GOING_UP,
        GOING_DOWN,
        WAITING,
        WAITING_DOWN
    };

    private door_state doorState;
    private float timeSinceStateChanged;

	// Use this for initialization
	void Start () {
        doorState = door_state.WAITING;
        timeSinceStateChanged = 0.0f;
        initialPosition = transform.position;
        BoxCollider[] boxColliders = transform.GetComponents<BoxCollider>();
        boxCollider = boxColliders[0];
        if (!boxCollider.isTrigger)
            boxCollider = boxColliders[1];
        boxCollider.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        if(world.doUpdate)
        {
            timeSinceStateChanged += world.lag;
            float lerp;
            float auxY;
            switch (doorState)
            {
                case door_state.GOING_UP:
                    lerp = timeSinceStateChanged / timeToGoUp;
                    if (lerp >= 1.0f)
                    {
                        lerp = 1.0f;
                        timeSinceStateChanged = 0.0f;
                        doorState = door_state.WAITING;
                    }
                    auxY = Mathf.Lerp(initialPosition.y - distance, initialPosition.y, lerp);
                    transform.position = new Vector3(transform.position.x, auxY, transform.position.z);
                    break;

                case door_state.GOING_DOWN:
                    lerp = timeSinceStateChanged / timeToGoDown;
                    if (lerp >= 1.0f)
                    {
                        lerp = 1.0f;
                        timeSinceStateChanged = 0.0f;
                        doorState = door_state.WAITING_DOWN;
                        boxCollider.enabled = false;
                    }
                    auxY = Mathf.Lerp(initialPosition.y, initialPosition.y - distance, lerp);
                    transform.position = new Vector3(transform.position.x, auxY, transform.position.z);
                    break;

                case door_state.WAITING:
                    if (timeSinceStateChanged >= timeWaiting)
                    {
                        timeSinceStateChanged = 0.0f;
                        doorState = door_state.GOING_DOWN;
                        boxCollider.enabled = true;

                    }
                    break;

                case door_state.WAITING_DOWN:
                    if (timeSinceStateChanged >= timeWaiting)
                    {
                        timeSinceStateChanged = 0.0f;
                        doorState = door_state.GOING_UP;

                    }
                    break;
            }
        }
    }
}
