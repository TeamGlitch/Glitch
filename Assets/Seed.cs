using UnityEngine;
using System.Collections;

public class Seed : MonoBehaviour {

    public World world;
    public ShooterArtichoke artichoke;

    private Vector3 initPos;

	void Start ()
    {
        initPos = transform.position;
	}
	
	void Update ()
    {
	    if (world.doUpdate)
        {
            transform.Translate(0.0f, world.lag*50, 0.0f);
        }
	}

    void OnCollisionEnter(Collision coll)
    {
        transform.position = initPos;
        artichoke.RestartShoot();
        enabled = false;
    }
}
