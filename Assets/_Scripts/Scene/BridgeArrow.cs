using UnityEngine;
using System.Collections;

public class BridgeArrow : MonoBehaviour {

    public float speed;
    public World world;

    private Vector3 initPosition;

	void Awake () 
    {
        initPosition = transform.position;
	}
	
	void Update () 
    {
        if (world.doUpdate)
        {
            transform.Translate(0.0f, -(speed * world.lag), 0.0f);
        }
	}

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player") || coll.CompareTag("Death"))
        {
            transform.position = initPosition;
        }
    }
}
