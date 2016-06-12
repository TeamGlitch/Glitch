using UnityEngine;
using System.Collections;

public class RootScript : MonoBehaviour {

    public World world;
    public bool down = true;

    private bool top;

    void Start()
    {
        if (down)
        {
            top = true;
        }
        else
        {
            top = false;
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if (!coll.collider.CompareTag("Player"))
        {
            if (!down)
            {
                down = true;
                if (top)
                {
                    enabled = false;
                }
            }
            else
            {
                down = false;
                if (!top)
                {
                    enabled = false;
                }
            }
        }
    }

	void Update () {
        if (down)
        {
            transform.Translate(0.0f, -world.lag * 20, 0.0f);
        }
        else
        {
            transform.Translate(0.0f, world.lag * 10, 0.0f);
        }
	}
}
