using UnityEngine;
using System.Collections;

public class ShooterArtichoke : MonoBehaviour {

    public Seed seed;
    public float seconds;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        Invoke("RestartShoot", seconds);
    }

	public void Shoot()
    {
        seed.enabled = true;
        anim.SetBool("Shoot", false);
    }

    public void RestartShoot()
    {
        anim.SetBool("Shoot", true);
    }
}
