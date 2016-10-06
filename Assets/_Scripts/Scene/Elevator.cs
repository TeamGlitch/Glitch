using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour {

    public World world;
    public float speed;
    public bool up;
	
	void Update () 
    {
        if (world.doUpdate)
        {
            if (up)
            {
                transform.Translate(0.0f, speed * world.lag, 0.0f);
            }
            else
            {
                transform.Translate(0.0f, -(speed * world.lag), 0.0f);
            }
        }
	}
}
