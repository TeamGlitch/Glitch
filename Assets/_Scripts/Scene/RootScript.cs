using UnityEngine;
using System.Collections;

public class RootScript : MonoBehaviour {

    public GameObject root;
    public float waitingTime;

    private Animator anim;
    private bool stop = false;

    void Start()
    {
        anim = root.GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider coll)
    {
        if (!stop && coll.CompareTag("Player"))
        {
            anim.SetBool("Extend", true);
            stop = true;
            Invoke("Shrink", waitingTime);
        }
    }

    public void Shrink()
    {
        anim.SetBool("Extend", false);
        Invoke("Reactivate", 1.0f);
    }

    public void Reactivate()
    {
        stop = false;
    }
}
