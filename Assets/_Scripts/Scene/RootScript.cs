using UnityEngine;
using System.Collections;

public class RootScript : MonoBehaviour {

    public World world;
    public bool horizontal = false;
    public bool down = true;
    public bool toLeft = false;

    private bool top;
    private bool right;

    void Start()
    {
        if (horizontal)
        {
            if (toLeft)
            {
                right = true;
            }
            else
            {
                right = false;
            }

        }
        else
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
    }

    void OnCollisionEnter(Collision coll)
    {
        if (!coll.collider.CompareTag("Player"))
        {
            if (horizontal)
            {
                if (!right)
                {
                    right = true;
                }
                else
                {
                    right = false;
                }
            }
            else
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
    }

	void Update () {
        if (horizontal)
        {
            if (toLeft)
            {
                if (!right)
                {
                    transform.Translate(world.lag * 10, 0.0f, 0.0f);
                }
                else
                {
                    transform.Translate(-world.lag * 30, 0.0f, 0.0f);
                }
            }
            else
            {
                if (!right)
                {
                    transform.Translate(world.lag * 30, 0.0f, 0.0f);
                }
                else
                {
                    transform.Translate(-world.lag * 10, 0.0f, 0.0f);
                }
            }
        }
        else
        {
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
}
