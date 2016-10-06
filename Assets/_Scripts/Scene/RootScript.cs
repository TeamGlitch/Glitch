using UnityEngine;
using System.Collections;

public class RootScript : MonoBehaviour {

    public GameObject root;
    public float waitUntilExtend;
    public float waitUntilShrink;
    public bool continousMoving;

    private Animator anim;
    private bool stop = false;

    void Start()
    {
        anim = root.GetComponent<Animator>();
    }

    void Update()
    {
        if (continousMoving && !stop)
        {
            Invoke("Extend", waitUntilExtend);
            stop = true;
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (!continousMoving && !stop && coll.CompareTag("Player"))
        {
            Invoke("Extend", waitUntilExtend);
            stop = true;
        }
    }

    public void Extend()
    {
        anim.SetBool("Extend", true);
        Invoke("Shrink", waitUntilShrink);
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
