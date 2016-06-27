using UnityEngine;
using System.Collections;

public class TempleDoors : MonoBehaviour {

    public float timeToGoUp = 1f;
    public float timeToGoDown = 0.25f;
    public float timeWaiting = 2.0f;

    public float distance = 0.07f;
    private Vector3 _initialPosition;

    public World world;

    private BoxCollider boxCollider;

    private enum door_state
    {
        GOING_UP,
        GOING_DOWN,
        WAITING,
        WAITING_DOWN
    };

    private door_state _doorState;
    private float _timeSinceStateChanged;

	// Use this for initialization
	void Start () {
        _doorState = door_state.WAITING;
        _timeSinceStateChanged = 0.0f;
        _initialPosition = transform.position;
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
            _timeSinceStateChanged += world.lag;
            float lerp;
            float auxY;
            switch (_doorState)
            {
                case door_state.GOING_UP:
                    lerp = _timeSinceStateChanged / timeToGoUp;
                    if (lerp >= 1.0f)
                    {
                        lerp = 1.0f;
                        _timeSinceStateChanged = 0.0f;
                        _doorState = door_state.WAITING;
                    }
                    auxY = Mathf.Lerp(_initialPosition.y - distance, _initialPosition.y, lerp);
                    transform.position = new Vector3(transform.position.x, auxY, transform.position.z);
                    break;

                case door_state.GOING_DOWN:
                    lerp = _timeSinceStateChanged / timeToGoDown;
                    if (lerp >= 1.0f)
                    {
                        lerp = 1.0f;
                        _timeSinceStateChanged = 0.0f;
                        _doorState = door_state.WAITING_DOWN;
                    }
                    auxY = Mathf.Lerp(_initialPosition.y, _initialPosition.y - distance, lerp);
                    transform.position = new Vector3(transform.position.x, auxY, transform.position.z);
                    break;

                case door_state.WAITING:
                    if (_timeSinceStateChanged >= timeWaiting)
                    {
                        _timeSinceStateChanged = 0.0f;
                        _doorState = door_state.GOING_DOWN;
                        boxCollider.enabled = true;

                    }
                    break;

                case door_state.WAITING_DOWN:
                    if (_timeSinceStateChanged >= timeWaiting)
                    {
                        _timeSinceStateChanged = 0.0f;
                        _doorState = door_state.GOING_UP;
                        boxCollider.enabled = false;

                    }
                    break;
            }
        }
    }
}
