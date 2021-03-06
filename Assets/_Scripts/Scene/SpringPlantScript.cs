﻿using UnityEngine;
using System.Collections;
using InControl;

public class SpringPlantScript : MonoBehaviour
{

    public PlayerController playerController;
    public float forceToPlayerWhenEnter = 700.0f;
    public float forceToPlayerWhenEnterAndJump = 1200.0f;
    public AudioClip bouncyClip;

    private Animator animator;

    void Start()
    {
        animator = transform.GetComponent<Animator>();
    }

    public void EndAnimation()
    {
        animator.SetBool("PlayerInside", false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController.rigidBody.velocity = new Vector3(playerController.rigidBody.velocity.x, 0.0f, 0.0f);
            playerController.rigidBody.AddForce(new Vector3(0.0f, forceToPlayerWhenEnter, 0.0f));
            animator.SetBool("PlayerInside", true);
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (InputManager.ActiveDevice.Action1.IsPressed)
            {
                playerController.rigidBody.AddForce(new Vector3(0.0f, forceToPlayerWhenEnterAndJump, 0.0f));
            }
            playerController.teleport.teleportUsed = false;
        }
        SoundManager.instance.PlaySingle(bouncyClip);
    }

}
