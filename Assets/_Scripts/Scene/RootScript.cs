﻿using UnityEngine;
using System.Collections;

public class RootScript : MonoBehaviour {

    public World world;
    public bool horizontal = false;
    public bool down = true;
    public bool toLeft = false;

    private bool top;
    private bool right;
    private bool stop = false;

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
                    stop = true;
                    // Is in origin
                    if (toLeft)
                    {
                        Invoke("Reactivate", 4.0f);
                    }
                    else
                    {
                        Invoke("Reactivate", 2.0f);
                    }
                }
                else
                {
                    right = false;
                    stop = true;
                    // Is in origin
                    if (!toLeft)
                    {
                        Invoke("Reactivate", 4.0f);
                    }
                    else
                    {
                        Invoke("Reactivate", 2.0f);
                    }
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
                    else
                    {
                        stop = true;
                        Invoke("Reactivate", 2.0f);
                    }
                }
                else
                {
                    down = false;
                    if (!top)
                    {
                        enabled = false;
                    }
                    else
                    {
                        stop = true;
                        Invoke("Reactivate", 2.0f);
                    }
                }
            }
        }
    }

	void Update () {
        if (!stop)
        {
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
                        transform.Translate(-world.lag * 50, 0.0f, 0.0f);
                    }
                }
                else
                {
                    if (!right)
                    {
                        transform.Translate(world.lag * 50, 0.0f, 0.0f);
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
                    transform.Translate(0.0f, -world.lag * 30, 0.0f);
                }
                else
                {
                    transform.Translate(0.0f, world.lag * 10, 0.0f);
                }
            }
        }
	}

    public void Reactivate()
    {
        stop = false;
    }
}
