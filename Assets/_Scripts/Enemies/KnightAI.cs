using UnityEngine;
using System.Collections;

public class KnightAI : MonoBehaviour {

    enum enemy_states
    {
        PATROL,
        CHASE,
        ATTACK,
        SEARCH
    }

    // Constants
<<<<<<< HEAD
    public const float maxSightPatrol = 8.0f;
    public const float maxSightChase = 16.0f;
    public const float maxSightAttack = 1.0f;
    public const float maxSightSearch = 18.0f;
    public const float searchingTime = 5.0f;
=======
    private const float maxSightPatrol = 8.0f;
    private const float maxSightChase = 14.0f;
    private const float maxSightAttack = 1.0f;
    private const float maxSightSearch = 16.0f;
    private const float searchingTime = 5.0f;
    private const float patrolSpeed = 2.0f;
    private const float chaseSpeed = 6.0f;
    private const float searchSpeed = 4.0f;
    private const float attackSpeed = 6.0f;
>>>>>>> AI

    // Variables
    public bool sight = false;
    public float speed = 2.0f;
<<<<<<< HEAD
    public Transform playerPos;

=======
    public Player player;

    private Transform playerPos;
>>>>>>> AI
    private Vector3 lastPosition;
    private enemy_states states = enemy_states.PATROL;
    private Ray ray;
    private RaycastHit hit;
    private float time;
    private float rotationTime = 0.0f;
    private float searchRotationTime = 1.0f;
    private bool returning = false;
<<<<<<< HEAD

=======
    private float damageAttack = 0.5f;
    private int lives = 2;

    void Start()
    {
        playerPos = player.GetComponent<Transform>();
    }
>>>>>>> AI

    // Trigger that detect collisions with patrol points and limit points
    void OnTriggerEnter(Collider coll)
    {
        // Only detect collisions with patrol points when enemy is patrolling
<<<<<<< HEAD
        if ((coll.gameObject.tag == "PatrolPoint") && (states == enemy_states.PATROL))
=======
        if ((coll.gameObject.CompareTag("PatrolPoint")) && (states == enemy_states.PATROL))
>>>>>>> AI
        {
            // ANIMATION OF ROTATION HERE

            // In patrol point enemy turns and continue with the patrol
            if ((transform.rotation.eulerAngles.y < 270.0f + 1) && (transform.rotation.eulerAngles.y > 270.0f - 1))
            {
                transform.Rotate(0.0f, -(transform.eulerAngles.y - 90), 0.0f);
            }
            else
            {
                transform.Rotate(0.0f, 270 - transform.eulerAngles.y, 0.0f);
            }
        }
<<<<<<< HEAD
        else
        {
            // If is in a limit he stops and search glitch
            if (coll.gameObject.tag == "LimitPoint")
            {
                states = enemy_states.SEARCH;
                time = searchingTime;
=======
        // If is in a limit he stops and search glitch
        else if (coll.gameObject.CompareTag("LimitPoint"))
        {
            states = enemy_states.SEARCH;
            time = searchingTime;
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            // If is attacking a collison hurts player
            if (states == enemy_states.ATTACK)
            {
                player.DecrementLives(damageAttack);

                // To impulse player from enemy
                player.ReactToAttack(transform.position.x);
            }
            else
            {
                // If knight loses sight of player and collides means that is colliding over knight
                Ray ray = new Ray(transform.position, player.transform.position - transform.position);
                Physics.Raycast(ray, out hit);

                // The ray is from knight to player, then collides down of player (-transform.up = down)
                if (hit.normal == -transform.up)
                {
                    --lives;

                    if (lives == 0)
                    {
                        gameObject.SetActive(false);
                    }
                }
>>>>>>> AI
            }
        }
    }

    void Update()
    {
        // if the enemy hasn't seen Glitch he patrols and if he detects him with the raycast
        // then changes his state to Chase and changes his speed too.
        if (sight == false)
        {
            ray.origin = transform.position;
            ray.direction = transform.TransformDirection(Vector3.forward);

            // Patrolling logic
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            if ((Physics.Raycast(ray, out hit, maxSightPatrol)) && (hit.collider.gameObject.tag == "Player"))
            {
                sight = true;
<<<<<<< HEAD
                speed = 6.0f;
=======
                speed = chaseSpeed;
>>>>>>> AI
                lastPosition = transform.position;      // We save the point in the patrol to return later
                states = enemy_states.CHASE;
            }
        }
        else
        {
            switch (states)
            {
<<<<<<< HEAD
                    // Enemy chases Glitch until reach him, reach a limit point or lose sight of Glitch
=======
                // Enemy chases Glitch until reach him, reach a limit point or lose sight of Glitch
>>>>>>> AI
                case enemy_states.CHASE:

                    // Chasing logic
                    if ((transform.rotation.eulerAngles.y < 270.0f + 1) && (transform.rotation.eulerAngles.y > 270.0f - 1))
                    {
                        if (playerPos.position.x > transform.position.x)
                        {
                            transform.Rotate(0.0f, -(transform.eulerAngles.y - 90), 0.0f);
                            rotationTime = 0.5f;
                        }
                    }
                    else
                    {
                        if (playerPos.position.x < transform.position.x)
                        {
                            transform.Rotate(0.0f, 270-transform.eulerAngles.y, 0.0f);
                            rotationTime = 0.5f;
                        }
                    }

                    rotationTime -= Time.deltaTime;
                    if (rotationTime <= 0.0f)
                    {
                        transform.Translate(Vector3.forward * speed * Time.deltaTime);
                    }

                    if (Vector3.Distance(playerPos.position, transform.position) > maxSightChase)
                    {
<<<<<<< HEAD
                        speed = 4.0f;
=======
                        speed = searchSpeed;
>>>>>>> AI
                        states = enemy_states.SEARCH;
                        time = searchingTime;
                    }
                    else if (Vector3.Distance(playerPos.position, transform.position) <= maxSightAttack)
                    {
                        states = enemy_states.ATTACK;
                    }

                    break;

                    // Enemy attacks Glitch
                case enemy_states.ATTACK:

                    // Attacking logic
                    // ATTACK ANIMATION AND LOGIC HERE

                    // If distance to Glitch is plus than chase field of view then changes to Search state
                    // else if Glitch isn't in attack scope then enemy chases him
                    if (Vector3.Distance(playerPos.position, transform.position) > maxSightChase)
                    {
<<<<<<< HEAD
                        speed = 4.0f;
=======
                        speed = searchSpeed;
>>>>>>> AI
                        states = enemy_states.SEARCH;
                        time = searchingTime;
                    }
                    else if (Vector3.Distance(playerPos.position, transform.position) > maxSightAttack)
                    {
                        states = enemy_states.CHASE;
                    }
                    break;

                    // Enemy looks at one side and the other looking for Glitch. After 5 seconds he returns to last patrol position 
                    // and continues patrolling.
                case enemy_states.SEARCH:

                    // Searching logic
                    // SEARCH ANIMATION HERE

<<<<<<< HEAD
=======
                    ray.origin = transform.position;
                    ray.direction = transform.TransformDirection(Vector3.forward);

                    if ((Physics.Raycast(ray, out hit, maxSightSearch)) && (hit.collider.gameObject.tag == "Player"))
                    {
                        speed = chaseSpeed;
                        states = enemy_states.CHASE;

                    }

>>>>>>> AI
                    // If is returning enemy advances to last patrol point, 
                    // else search looking to one side and the other
                    if (returning == false)
                    {
<<<<<<< HEAD
                        ray.origin = transform.position;
                        ray.direction = transform.TransformDirection(Vector3.forward);

                        if ((Physics.Raycast(ray, out hit, maxSightSearch)) && (hit.collider.gameObject.tag == "Player"))
                        {
                            speed = 6.0f;
                            states = enemy_states.CHASE;

                        }
                        else
=======
                        if (states == enemy_states.SEARCH)
>>>>>>> AI
                        {
                            time -= Time.deltaTime;
                            if (time <= 0.0f)
                            {
                                if ((transform.rotation.eulerAngles.y < 270.0f + 1) && (transform.rotation.eulerAngles.y > 270.0f - 1))
                                {
                                    if (lastPosition.x > transform.position.x)
                                    {
                                        transform.Rotate(0.0f, -(transform.eulerAngles.y - 90), 0.0f);
                                    }
                                }
                                else
                                {
                                    if (lastPosition.x < transform.position.x)
                                    {
                                        transform.Rotate(0.0f, 270 - transform.eulerAngles.y, 0.0f);
                                    }
                                }
<<<<<<< HEAD
                                speed = 2.0f;
=======
                                speed = patrolSpeed;
>>>>>>> AI
                                returning = true;
                                searchRotationTime = 1.0f;
                            }
                            else
                            {
                                searchRotationTime -= Time.deltaTime;
                                if (searchRotationTime <= 0.0f)
                                {
                                    transform.Rotate(0.0f, 180, 0.0f);
                                    searchRotationTime = 1.0f;
                                }
                            }
                        }
                    }
                    else
                    {
<<<<<<< HEAD
                        transform.Translate(Vector3.forward * speed * Time.deltaTime);
                        if ((transform.position.x < lastPosition.x + 1) && (transform.position.x > lastPosition.x - 1))
                        {
                            returning = false;
                            sight = false;
                            states = enemy_states.PATROL;
=======
                        if (states == enemy_states.SEARCH)
                        {
                            transform.Translate(Vector3.forward * speed * Time.deltaTime);
                            if ((transform.position.x < lastPosition.x + 1) && (transform.position.x > lastPosition.x - 1))
                            {
                                returning = false;
                                sight = false;
                                states = enemy_states.PATROL;
                            }
>>>>>>> AI
                        }
                    }
                    break;

            }
        }
    }
}
