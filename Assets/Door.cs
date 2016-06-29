using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    public SphereCollider sphere;
    public BoxCollider box;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            Loader.LoadScene("Congratulations");
        }
    }

    public void OpenDoor()
    {
        animator.SetBool("Open", true);
        sphere.enabled = false;
        box.enabled = true;
    }
}
