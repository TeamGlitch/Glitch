using UnityEngine;
using System.Collections;

public class Rock : MonoBehaviour {

    public Animator animatorWall;
    public World world;

    private int touches;
    private Animator animatorRock;

    void OnCollisionEnter(Collision coll)
    {
        if (touches == 0 && coll.collider.CompareTag("Floor"))
        {
            //animatorWall.setBool("rompe");
            ++touches;
        }
        else if (touches == 1 && coll.collider.CompareTag("Floor"))
        {
            //animatorRock.setBool("rota");
            ++touches;
        }
        else if (touches == 2 && coll.collider.CompareTag("Wall"))
        {
            //animatorRock.setBool("romper");
            ++touches;
        }
    }

	void Start () 
    {
        touches = 0;
        animatorRock = GetComponent<Animator>();
	}
	
	void Update () 
    {
        if (world.doUpdate)
        {
            if (touches <= 2)
            {
                transform.Translate(0.0f, -(world.lag * 5), 0.0f);
            }
            else if(touches == 3)
            {
                transform.Translate(-(world.lag * 5), 0.0f, 0.0f);
            }
        }
	}
}
