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
    public const float maxSightPatrol = 8.0f;
    public const float maxSightChase = 16.0f;
    public const float maxSightAttack = 1.0f;
    public const float maxSightSearch = 18.0f;
    public const float searchingTime = 5.0f;

    // Variables
    public bool sight = false;
    public float speed = 2.0f;
    public Transform playerPos;

    private Vector3 lastPosition;
    private enemy_states states = enemy_states.PATROL;
    private Ray ray;
    private RaycastHit hit;
    private float time;
    private float rotationTime = 0.0f;
    private float searchRotationTime = 1.0f;
    private bool returning = false;


    // Trigger that detect collisions with patrol points and limit points
    void OnTriggerEnter(Collider coll)
    {
        // Only detect collisions with patrol points when enemy is patrolling
        if ((coll.gameObject.tag == "PatrolPoint") && (states == enemy_states.PATROL))
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
        else
        {
            // If is in a limit he stops and search glitch
            if (coll.gameObject.tag == "LimitPoint")
            {
                states = enemy_states.SEARCH;
                time = searchingTime;
            }
        }
    }

    void Update()
    {
        // if the enemy hasn't seen Glitch he patrols and if he detects him with the raycast
        // then changes his state to Chase and changes his speed too.
        if (sight == false)
        {
            print("Patrolling");
            ray.origin = transform.position;
            ray.direction = transform.TransformDirection(Vector3.forward);

            // Patrolling logic
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            if ((Physics.Raycast(ray, out hit, maxSightPatrol)) && (hit.collider.gameObject.tag == "Player"))
            {
                sight = true;
                speed = 6.0f;
                lastPosition = transform.position;      // We save the point in the patrol to return later
                states = enemy_states.CHASE;
            }
        }
        else
        {
            switch (states)
            {
                    // Enemy chases Glitch until reach him, reach a limit point or lose sight of Glitch
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
                        speed = 4.0f;
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
                        speed = 4.0f;
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

                    // If is returning enemy advances to last patrol point, 
                    // else search looking to one side and the other
                    if (returning == false)
                    {
                        ray.origin = transform.position;
                        ray.direction = transform.TransformDirection(Vector3.forward);

                        if ((Physics.Raycast(ray, out hit, maxSightSearch)) && (hit.collider.gameObject.tag == "Player"))
                        {
                            speed = 6.0f;
                            states = enemy_states.CHASE;

                        }
                        else
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
                                speed = 2.0f;
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
                        transform.Translate(Vector3.forward * speed * Time.deltaTime);
                        if ((transform.position.x < lastPosition.x + 1) && (transform.position.x > lastPosition.x - 1))
                        {
                            returning = false;
                            sight = false;
                            states = enemy_states.PATROL;
                        }
                    }
                    break;

            }
        }
    }
}
