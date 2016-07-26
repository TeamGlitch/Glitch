﻿using UnityEngine;
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

    private BoxCollider collider;
    private int rand;
    private Animator anim;
    private float yPos;
    private Vector3 initPos;

    void Start()
    {
        collider = GetComponent<BoxCollider>();
        initPos = transform.localPosition;
        yPos = initPos.y;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (world.doUpdate)
        {
            switch (mode)
            {
                case debris_state.FALLING:
                    yPos -= world.lag;
                    transform.Translate(0.0f, yPos, 0.0f);
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
        collider.enabled = true;
        yPos = initPos.y;
        anim.SetInteger("State", 0);
        transform.localPosition = initPos;
        mode = debris_state.WAITING;
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
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
        }
        else
        {
            z = 0.2f;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
        }
        initPos = transform.localPosition;
    }
}
