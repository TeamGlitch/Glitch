using UnityEngine;
using System.Collections;

public class ArcherAI : MonoBehaviour {

	enum enemy_states
    {
        WAIT,
        SHOOT,
        MELEE_ATTACK,
        CHASE
    }

    // Constants
    private const float maxSightChase = 9.0f;
    private const float maxSightPersecution = 10.0f;
    private const float maxSightMeleeAttack = 1.2f;
    private const float maxSightShoot = 20.0f;
    private const float maxJump = 8.0f;

    // Variables
    public bool sight = false;
    public float speed = 0.0f;
    public Transform playerPos;
    public bool motionless = false;

    private Vector3 initialPosition;
    private enemy_states states = enemy_states.WAIT;
    private Ray ray;
    private RaycastHit hit;
    private float rotationTime = 0.0f;
    private float searchRotationTime = 1.0f;
    private bool returning = false;

    // Trigger that detect player and change the state to Shoot
    void OnTriggerStay(Collider coll)
    {
        // If is the player and between player and archer isn't anything then shoot him
        if (coll.gameObject.tag == "Player")
        {
            ray = new Ray(transform.localPosition, playerPos.position - transform.localPosition);
            Debug.DrawRay(transform.localPosition, playerPos.position - transform.localPosition);
            if ((sight == false) && (Physics.Raycast(ray, out hit)) && (hit.collider.gameObject.tag == "Player"))
            {
                sight = true;
                speed = 6.0f;
                initialPosition = transform.position;      // We save the initial point to return later
                states = enemy_states.SHOOT;
            }
        }
    }

    void Update()
    {

        if (sight == true)
        {
            switch (states)
            {

                // Enemy shoot arrows to Glitch
                case enemy_states.SHOOT:
                    print("Shoot");

                    // Shooting logic
                    // SHOOT ANIMATION AND LOGIC HERE



                    // If distance to Glitch is minus than chase field of view then changes to Chase state.
                    // If Glitch is in melee attack scope then enemy attacks to him with daggers, changing her state to Melee attack
                    // If distance to Glitch is plus than Shoot field of view the enemy changes her state to Wait
                    // If archer is motionless then she can't move
                    if ((motionless == false) && (Vector3.Distance(playerPos.position, transform.position) < maxSightChase))
                    {
                        speed = 10.0f;
                        states = enemy_states.CHASE;
                    }
                    else if (Vector3.Distance(playerPos.position, transform.position) > maxSightShoot)
                    {
                        speed = 0.0f;
                        states = enemy_states.WAIT;
                        sight = false;
                    }
                    break;

                // Enemy chases Glitch until reach him or lose sight of Glitch
                case enemy_states.CHASE:
                    print("Chase");

                    // Chasing logic
                    if ((transform.rotation.eulerAngles.y < 270.0f + 1) && (transform.rotation.eulerAngles.y > 270.0f - 1))
                    {
                        if (playerPos.position.x > transform.position.x)
                        {
                            transform.Rotate(0.0f, -(transform.eulerAngles.y - 90), 0.0f);
                            rotationTime = 0.2f;
                        }
                    }
                    else
                    {
                        if (playerPos.position.x < transform.position.x)
                        {
                            transform.Rotate(0.0f, 270-transform.eulerAngles.y, 0.0f);
                            rotationTime = 0.2f;
                        }
                    }

                    rotationTime -= Time.deltaTime;
                    if (rotationTime <= 0.0f)
                    {
                        transform.Translate(Vector3.forward * speed * Time.deltaTime);
                    }

                    if (Vector3.Distance(playerPos.position, transform.position) > maxSightPersecution)
                    {
                        speed = 6.0f;
                        states = enemy_states.SHOOT;
                    }
                    else if (Vector3.Distance(playerPos.position, transform.position) <= maxSightMeleeAttack)
                    {
                        speed = 10.0f;
                        states = enemy_states.MELEE_ATTACK;
                    }

                    break;

                // Enemy attacks to Glitch with her daggers
                case enemy_states.MELEE_ATTACK:
                    print("Melee Attack");

                    // Melee attack logic
                    // MELEE ATTACK ANIMATION HERE

                    if (Vector3.Distance(playerPos.position, transform.position) > maxSightMeleeAttack)
                    {
                        speed = 9.0f;
                        states = enemy_states.CHASE;
                    }
                   
                    break;
            }
        }
    }
}
