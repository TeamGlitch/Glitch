using UnityEngine;
using System.Collections;

public class KnightAI : MonoBehaviour {

    public enum enemy_states
    {
        WAIT,
        PATROL,
        CHASE,
        ATTACK,
        SEARCH,
        DEATH,
        RETURNING
    }

    // Constants
    private const float maxSightPatrol = 8.0f;
    private const float maxSightChase = 14.0f;
    private const float maxSightAttack = 2.5f;
    private const float maxSightSearch = 16.0f;
    private const float searchingTime = 5.0f;
    private const float patrolSpeed = 2.0f;
    private const float chaseSpeed = 6.0f;
    private const float searchSpeed = 4.0f;
    private const float attackSpeed = 6.0f;
    private const float waitTime = 3.0f;
    private const float attackTime = 1.0f;
    private const int damageAttack = 1;

    // Variables
    public bool sight = false;
    public float speed = 2.0f;
    public Player player;
    public BoxCollider collider;
    public BoxCollider fieldOfView;
    public Rigidbody rigid;
    public BoxCollider swordCollider;
    public enemy_states states = enemy_states.PATROL;

    private Transform playerPos;
    private Vector3 lastPosition;
    private Ray ray;
    private RaycastHit hit;
    private float time;
    private float rotationTime = 0.0f;
    private float searchRotationTime = 1.0f;
    private bool returning = false;
    private int lives = 2;
    private Animator animator;
    private float timePerAttack = 0.0f;
    private Vector3 origin;

    void Start()
    {
        playerPos = player.GetComponent<Transform>();
        animator = GetComponent<Animator>();
    }

    // Trigger that detect collisions with patrol points and limit points
    void OnTriggerEnter(Collider coll)
    {
        // Only detect collisions with patrol points when enemy is patrolling
        if ((coll.gameObject.CompareTag("PatrolPoint")) && (states == enemy_states.PATROL))
        {
            states = enemy_states.WAIT;
            time = waitTime;
        }
        // If is in a limit he stops and search glitch
        else if (coll.gameObject.CompareTag("LimitPoint"))
        {
            speed = searchSpeed;
            states = enemy_states.SEARCH;
            time = searchingTime;
        }
    }

    void OnTriggerStay(Collider coll)
    {
        // if the enemy hasn't seen Glitch he patrols and if he detects him with the raycast
        // then changes his state to Chase and changes his speed too.
        if ((sight == false) && coll.gameObject.CompareTag("Player"))
        {
            origin = transform.position;
            origin.y += transform.localScale.y * 0.75f;

            ray = new Ray(origin, player.transform.position - origin);
            Debug.DrawRay(origin, player.transform.position - origin);
            if ((Physics.Raycast(ray, out hit)) && hit.collider.gameObject.CompareTag("Player"))
            {
                sight = true;
                speed = chaseSpeed;
                lastPosition = transform.position;      // We save the point in the patrol to return later
                states = enemy_states.CHASE;
            }
        }
    }

    void Update()
    {
        animator.SetInteger("State", (int)states);
        
        switch (states)
        {
            case enemy_states.WAIT:
                // ANIMATION OF ROTATION HERE

                // In patrol point knight turns and continue with the patrol
                time -= Time.deltaTime;
                if (time <= 0.0f)
                {
                    if ((transform.rotation.eulerAngles.y < 270.0f + 1) && (transform.rotation.eulerAngles.y > 270.0f - 1))
                    {
                        transform.Rotate(0.0f, -(transform.eulerAngles.y - 90), 0.0f);
                    }
                    else
                    {
                        transform.Rotate(0.0f, 270 - transform.eulerAngles.y, 0.0f);
                    }
                    states = enemy_states.PATROL;
                }
                break;

            case enemy_states.PATROL:
                // Patrolling logic
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
                break;

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
                    speed = searchSpeed;
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
                timePerAttack -= Time.deltaTime;
                if (timePerAttack <= 0.0f)
                {
                    animator.SetBool("Near", true);
                }

                // If distance to Glitch is plus than chase field of view then changes to Search state
                // else if Glitch isn't in attack scope then enemy chases him
                if (Vector3.Distance(playerPos.position, transform.position) > maxSightChase)
                {
                    animator.SetBool("Near", false);
                    speed = searchSpeed;
                    states = enemy_states.SEARCH;
                    time = searchingTime;
                }
                else if (Vector3.Distance(playerPos.position, transform.position) > maxSightAttack)
                {
                    animator.SetBool("Near", false);
                    states = enemy_states.CHASE;
                }
                break;

                // Enemy looks at one side and the other looking for Glitch. After 5 seconds he returns to last patrol position 
                // and continues patrolling.
            case enemy_states.SEARCH:

                // Searching logic
                // SEARCH ANIMATION HERE

                origin = transform.position;
                origin.y += transform.localScale.y * 0.75f;

                ray = new Ray(origin, player.transform.position - origin);
                Debug.DrawRay(origin, player.transform.position - origin);

                if ((Physics.Raycast(ray, out hit, maxSightSearch)) && (hit.collider.gameObject.CompareTag("Player")))
                {
                    speed = chaseSpeed;
                    states = enemy_states.CHASE;

                }

                // If is returning enemy, advances to last patrol point 
                // else search looking to one side and the other
                if (returning == false)
                {
                    if (states == enemy_states.SEARCH)
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
                            speed = patrolSpeed;
                            returning = true;
                            searchRotationTime = 1.0f;
                            animator.SetBool("Returning", true);
                        }
                    }
                }
                else
                {
                    if (states == enemy_states.SEARCH)
                    {
                        transform.Translate(Vector3.forward * speed * Time.deltaTime);
                        if ((transform.position.x < lastPosition.x + 1) && (transform.position.x > lastPosition.x - 1))
                        {
                            returning = false;
                            sight = false;
                            animator.SetBool("Returning", false);
                            states = enemy_states.PATROL;
                        }
                    }
                }
                break;

        }
    }

    public void DeadRandomTrigger()
    {
        animator.SetInteger("DeadRandom", Random.Range(0, 3));
    }

    public void Attacked()
    {
        animator.SetBool("Near", true);
        speed = attackSpeed;
        states = enemy_states.ATTACK;
        --lives;

        if (lives == 0)
        {
            states = enemy_states.DEATH;
            rigid.isKinematic = true;
            collider.enabled = false;
            swordCollider.enabled = false;
            fieldOfView.enabled = false;
            animator.SetBool("Near", false);
        }
        else
        {
            // To impulse player from enemy
            player.ReactToAttack(transform.position.x);
        }
    }

    public void Attack()
    {
        player.DecrementLives(damageAttack);
        animator.SetBool("Near", false);
        speed = searchSpeed;
        states = enemy_states.SEARCH;
    }
}
