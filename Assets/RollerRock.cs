using UnityEngine;
using System.Collections;

public class RollerRock : MonoBehaviour {

    public bool toLeft;
    public World world;
    public Animator anim;

    private float speed = 10;
    private bool move = false;
    private Rigidbody rigid;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Impact"))
        {
            move = true;
            anim.enabled = true;
        }
        else if (coll.CompareTag("Fall"))
        {
            move = false;
            coll.enabled = false;
            rigid.isKinematic = false;
            rigid.useGravity = true;
        }
        else if (coll.CompareTag("Crush"))
        {
            //aqui va la animacion de romper
            gameObject.SetActive(false);
        }
    }

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

	void Update () 
    {
        if (world.doUpdate)
        {
            if (move)
            {
                if (toLeft)
                {
                    transform.Translate(-(speed * world.lag), 0.0f, 0.0f);
                }
                else
                {
                    transform.Translate(speed * world.lag, 0.0f, 0.0f);
                }
            }
        }
	}
}
