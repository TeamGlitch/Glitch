using UnityEngine;
using System.Collections;

public class CeilingBreakable : MonoBehaviour {

    private Animator anim;
    private BoxCollider box;

	void Start () 
    {
        anim = GetComponent<Animator>();
        box = GetComponent<BoxCollider>();
	}

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("Death"))
        {
            box.enabled = false;
            anim.SetBool("Break", true);
        }
    }
	
}
