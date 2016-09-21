using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    public BossStageCamera camera;
    public SphereCollider sphere;
    public BoxCollider box;
    public AudioClip sound;
    public Animator leftDoor;

    private Animator animator;

    void Start()
    {
        SoundManager.instance.PlaySingle(sound);
        animator = GetComponent<Animator>();
        leftDoor.SetBool("Close", true);
        animator.SetBool("Close", true);
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            camera.DarknessIn();
            Invoke("Congrats", 3.0f);
        }
    }

    public void Congrats()
    {
        Loader.LoadScene("Congratulations", false, false, true, true);
    }

    public void OpenDoor()
    {
        SoundManager.instance.PlaySingle(sound);
        animator.SetBool("Open", true);
        sphere.enabled = false;
        box.enabled = true;
    }
}
