using UnityEngine;
using System.Collections;

public class Debris : MonoBehaviour {

    public enum debris_state
    {
        WAITING,
        FALLING
    };
    
    public debris_state mode = debris_state.WAITING;
    public AudioClip impact;
    public World world;
    public BossArcherIA boss;

    private BoxCollider collider;
    private int rand;
    private Animator anim;
    private Vector3 initPos;

    void Start()
    {
        collider = GetComponent<BoxCollider>();
        initPos = transform.localPosition;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (world.doUpdate)
        {
            switch (mode)
            {
                case debris_state.FALLING:
                    transform.Translate(0.0f, -world.lag*30, 0.0f);
                    break;
            }
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        SoundManager.instance.PlaySingle(impact);
        collider.enabled = false;
        Invoke("Restart", 5.0f);
        anim.SetInteger("State", 1);
        mode = debris_state.WAITING;
    }

    public void Restart()
    {
        mode = debris_state.WAITING;
        collider.enabled = true;
        anim.SetInteger("State", 0);
        transform.localPosition = initPos;
    }

    public void Fall()
    {
        anim.SetInteger("State", 0);
        mode = debris_state.FALLING;
    }

    public void Reubicate()
    {
        rand = Random.Range(0, 2);
        float z;
        if (rand == 0)
        {
            z = -0.125f;
            transform.localPosition = new Vector3(initPos.x, initPos.y, z);
        }
        else
        {
            z = 0.2f;
            transform.localPosition = new Vector3(initPos.x, initPos.y, z);
        }
        initPos.z = transform.localPosition.z;
    }
}
