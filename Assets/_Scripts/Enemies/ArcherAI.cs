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
    private const float maxSightMeleeAttack = 1.0f;
    private const float maxSightShoot = 20.0f;
    private const float chaseSpeed = 10.0f;
    private const float waitSpeed = 0.0f;
    private const float meleeAttackSpeed = 10.0f;
    private const float shootSpeed = 6.0f;
    private const float maxJump = 8.0f;

    // Variables
    public bool sight = false;
    public float speed = 0.0f;
    public Player player;
    public bool motionless = false;
    public GameObject arrow;
    public ArrowScript arrowLogic;

    private Vector3 initialPosition;
    private enemy_states states = enemy_states.WAIT;
    private Ray ray;
    private RaycastHit hit;
    private float rotationTime = 0.0f;
    private float searchRotationTime = 1.0f;
    private bool returning = false;
    private float meleeDamage = 0.25f;

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            // If is attacking a collison hurts player
            if (states == enemy_states.MELEE_ATTACK)
            {
                player.DecrementLives(meleeDamage);
                
                // To impulse player from enemy
                if (transform.position.x > player.transform.position.x)
                {
                    player.transform.Translate(-5.0f, 0.0f, 0.0f);
                }
                else
                {
                    player.transform.Translate(5.0f, 0.0f, 0.0f);
                }
            }
            else
            {
                // If knight loses sight of player and collides means that is colliding over knight
                Ray ray = new Ray(transform.position, player.transform.position - transform.position);
                Physics.Raycast(ray, out hit);

                // The ray is from knight to player, then collides down of player (-transform.up = down)
                if (hit.normal == -transform.up)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }

    // Trigger that detect player and change the state to Shoot
    void OnTriggerStay(Collider coll)
    {
        // If is the player and between player and archer isn't anything then shoot him
        if (coll.CompareTag("Player"))
        {
            ray = new Ray(transform.localPosition, player.transform.position - transform.localPosition);
            Debug.DrawRay(transform.localPosition, player.transform.position - transform.localPosition);
            if ((sight == false) && (Physics.Raycast(ray, out hit)) && (hit.collider.gameObject.tag == "Player"))
            {
                sight = true;
                speed = shootSpeed;
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

                    if (!arrowLogic.isActiveAndEnabled)
                    {
                        arrow.SetActive(true);
                        arrowLogic.direction = ray.direction;
                        arrowLogic.transform.position = transform.position;
                    }

                    // If distance to Glitch is minus than chase field of view then changes to Chase state.
                    // If Glitch is in melee attack scope then enemy attacks to him with daggers, changing her state to Melee attack
                    // If distance to Glitch is plus than Shoot field of view the enemy changes her state to Wait
                    // If archer is motionless then she can't move
                    if ((motionless == false) && (Vector3.Distance(player.transform.position, transform.position) < maxSightChase))
                    {
                        speed = chaseSpeed;
                        states = enemy_states.CHASE;
                    }
                    else if (Vector3.Distance(player.transform.position, transform.position) > maxSightShoot)
                    {
                        speed = waitSpeed;
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
                        if (player.transform.position.x > transform.position.x)
                        {
                            transform.Rotate(0.0f, -(transform.eulerAngles.y - 90), 0.0f);
                            rotationTime = 0.2f;
                        }
                    }
                    else
                    {
                        if (player.transform.position.x < transform.position.x)
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

                    if (Vector3.Distance(player.transform.position, transform.position) > maxSightPersecution)
                    {
                        speed = shootSpeed;
                        states = enemy_states.SHOOT;
                    }
                    else if (Vector3.Distance(player.transform.position, transform.position) <= maxSightMeleeAttack)
                    {
                        speed = meleeAttackSpeed;
                        states = enemy_states.MELEE_ATTACK;
                    }

                    break;

                // Enemy attacks to Glitch with her daggers
                case enemy_states.MELEE_ATTACK:
                    print("Melee Attack");

                    // Melee attack logic
                    // MELEE ATTACK ANIMATION HERE

                    if (Vector3.Distance(player.transform.position, transform.position) > maxSightMeleeAttack)
                    {
                        speed = chaseSpeed;
                        states = enemy_states.CHASE;
                    }
                   
                    break;
            }
        }
    }
}
