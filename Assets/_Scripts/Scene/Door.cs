using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    public SphereCollider sphere;
    public BoxCollider box;
    public AudioClip sound;

    private Animator animator;

    void Start()
    {
        SoundManager.instance.PlaySingle(sound);
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
        SoundManager.instance.PlaySingle(sound);
        animator.SetBool("Open", true);
        sphere.enabled = false;
        box.enabled = true;
    }
}
