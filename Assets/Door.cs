using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    public SphereCollider sphere;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            print("HAS GANADO!");
        }
    }

    public void OpenDoor()
    {
        animator.SetBool("Open", true);
        sphere.enabled = false;
    }
}
