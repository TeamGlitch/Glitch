﻿using UnityEngine;
using System.Collections;

public class BerserkerAI : MonoBehaviour
{

    public enum enemy_states
    {
        WAIT,
        CHASE,
        ATTACK,
        DEATH,
        FALL,
        IMPACT,
        HITTED
    }

    // Constants
    private const float maxSightChase = 14.0f;
    private const float maxSightAttack = 2.5f;
    private const float waitSpeed = 2.0f;
    private const float chaseSpeed = 6.0f;
    private const float attackSpeed = 6.0f;
    private const float fallSpeed = 6.0f;
    private const float waitTime = 3.0f;
    private const float attackTime = 1.0f;
    private const int damageAttack = 1;
    private const float speedConstant = 0.5f;

    // Variables
    public bool sight = false;
    public float speed = waitSpeed;
    public Player player;
    public BoxCollider collider;
    public BoxCollider fieldOfView;
    public BoxCollider headCollider;
    public BoxCollider axeCollider1;
    public BoxCollider axeCollider2;
    public Rigidbody rigid;
    public enemy_states states = enemy_states.WAIT;
    public World world;

    private Transform playerPos;
    private Vector3 lastPosition;
    private Ray ray;
    private RaycastHit hit;
    private float rotationTime = 0.0f;
    private float searchRotationTime = 1.0f;
    private bool returning = false;
    private int lives = 2;
    private Animator animator;
    private float timePerAttack = 0.0f;
    private Vector3 origin;
    private int layerMask = (~((1 << 13) | (1 << 2) | (1 << 11))) | (1 << 9) | (1 << 0);

    void Start()
    {
        playerPos = player.GetComponent<Transform>();
        animator = GetComponent<Animator>();
        axeCollider1.enabled = false;
        lastPosition = transform.position;
    }

    void OnCollisionEnter(Collision coll)
    {
        if ((coll.contacts[0].thisCollider.CompareTag("Berserker")) && (coll.contacts[0].otherCollider.CompareTag("Player")) && (player.transform.position.y > (transform.position.y + transform.lossyScale.y / 2)))
        {
            Attacked();
        }
        else if ((sight == true) && (coll.contacts[0].thisCollider.CompareTag("Berserker")) && (coll.contacts[0].otherCollider.CompareTag("Player")))
        {
            Attack();
        }
    }

    // Trigger that detect collisions with patrol points and limit points
    void OnTriggerEnter(Collider coll)
    {
        // If is in a limit he stops and search glitch
        if (coll.gameObject.CompareTag("LimitPoint"))
        {
            if (coll.GetComponent<LimitPoint>().fall)
            {
                states = enemy_states.FALL;
            }
            else 
            {
                states = enemy_states.IMPACT;
            }
            speed = fallSpeed;
            sight = false;
        }
    }

    void OnTriggerStay(Collider coll)
    {
        // if the enemy hasn't seen Glitch he patrols and if he detects him with the raycast
        // then changes his state to Chase and changes his speed too.
        if ((sight == false) && coll.gameObject.CompareTag("Player") && (states != enemy_states.HITTED))
        {
            origin = transform.position;
            origin.y += transform.localScale.y * 0.75f;
            ray = new Ray(origin, player.transform.position - origin);

            if ((Physics.Raycast(ray, out hit, float.PositiveInfinity, layerMask)) && hit.collider.gameObject.CompareTag("Player"))
            {
                sight = true;
                speed = chaseSpeed;
                states = enemy_states.CHASE;
            }
        }
    }

    void Update()
    {

        if (world.doUpdate)
        {
            animator.SetFloat("Speed", speed * speedConstant);
            animator.SetInteger("State", (int)states);
            switch (states)
            {
                case enemy_states.WAIT:
                    // IDLE
                    break;

                // Enemy chases Glitch until reach him, reach a limit point or lose sight of Glitch
                case enemy_states.CHASE:

                    // Chasing logic
                    transform.Translate(Vector3.forward * speed * world.lag);

                    if (Vector3.Distance(playerPos.position, transform.position) <= maxSightAttack)
                    {
                        states = enemy_states.ATTACK;
                    }

                    break;

                // Enemy attacks Glitch
                case enemy_states.ATTACK:

                    // Attacking logic
                    // ATTACK ANIMATION AND LOGIC HERE
                    axeCollider1.enabled = true;
                    timePerAttack -= world.lag;
                    if (timePerAttack <= 0.0f)
                    {
                        animator.SetBool("Attack", true);
                    }

                    // If distance to Glitch is plus than chase field of view then changes to Search state
                    // else if Glitch isn't in attack scope then enemy chases him
                    if (Vector3.Distance(playerPos.position, transform.position) > maxSightAttack)
                    {
                        if ((transform.rotation.eulerAngles.y < 270.0f + 1) && (transform.rotation.eulerAngles.y > 270.0f - 1))
                        {
                            if (playerPos.position.x > transform.position.x)
                            {
                                transform.Rotate(0.0f, -(transform.eulerAngles.y - 90), 0.0f);
                            }
                        }
                        else
                        {
                            if (playerPos.position.x < transform.position.x)
                            {
                                transform.Rotate(0.0f, 270 - transform.eulerAngles.y, 0.0f);
                            }
                        }

                        animator.SetBool("Attack", false);
                        states = enemy_states.CHASE;
                    }
                    break;

                case enemy_states.HITTED:
                    // State to put particles or something
                    break;
            }
        }
        else
        {
            animator.SetFloat("Speed", 0.0f);
        }
    }

    public void DeadRandomTrigger()
    {
        animator.SetInteger("DeadRandom", Random.Range(0, 3));
    }

    public void ImpactTrigger()
    {
        states = enemy_states.WAIT;
    }

    public void HittedTrigger()
    {
        if (states != enemy_states.DEATH)
        {
            states = enemy_states.CHASE;
        }
    }

    public void AttackTrigger()
    {
        axeCollider1.enabled = false;
        axeCollider2.enabled = false;
    }

    public void Attacked()
    {
        animator.SetBool("Attack", true);
        speed = attackSpeed;
        states = enemy_states.HITTED;
        --lives;

        if (lives <= 0)
        {
            states = enemy_states.DEATH;
            rigid.isKinematic = true;
            collider.enabled = false;
            axeCollider1.enabled = false;
            axeCollider2.enabled = false;
            fieldOfView.enabled = false;
            headCollider.enabled = false;
            animator.SetBool("Attack", false);
        }

        // To impulse player from enemy
        player.ReactToAttack(transform.position.x);
    }

    public void Attack()
    {
        player.DecrementLives(damageAttack);
        animator.SetBool("Attack", false);
        sight = false;
        speed = waitSpeed;
        states = enemy_states.WAIT;
    }
}