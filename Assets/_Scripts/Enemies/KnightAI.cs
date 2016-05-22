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
        RETURNING,
        HITTED
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
    private const float speedConstant = 0.5f;

    // Variables
    public bool sight = false;
    public float speed = 2.0f;
    public Player player;
    public BoxCollider collider;
    public BoxCollider fieldOfView;
    public BoxCollider headCollider;
    public BoxCollider swordCollider;
    public Rigidbody rigid;
    public enemy_states states = enemy_states.PATROL;
    public World world;
    public bool attacked;
    public AudioClip hitSound;

    private Transform playerPos;
    private Vector3 lastPosition;
    private Ray ray;
    private RaycastHit hit;
    private float time;
    private float rotationTime = 0.0f;
    private float searchRotationTime = 1.0f;
    private int lives = 2;
    private Animator animator;
    private float timePerAttack = 0.0f;
    private Vector3 origin;
    private bool isInAttack = false;
    private bool isInLimitPoint = false;
    private int layerMask = (~((1 << 13) | (1 << 2) | (1 << 11) | (1 << 8))) | (1 << 9) | (1 << 0);

    void Start()
    {
        playerPos = player.GetComponent<Transform>();
        animator = GetComponent<Animator>();
        swordCollider.enabled = false;
        attacked = false;
        animator.SetInteger("DeadRandom", -1);
    }

    void OnCollisionEnter(Collision coll)
    {
        BerserkerAI berserker;
        KnightAI knight;

        if ((coll.contacts[0].thisCollider.CompareTag("Knight")) && (coll.contacts[0].otherCollider.CompareTag("Player")))
        {
            if ((player.transform.position.y >= (transform.position.y + coll.contacts[0].thisCollider.bounds.extents.y * 2)) && (attacked == false))
            {
                Attacked();
            }
            else if ((player.transform.position.y < (transform.position.y + coll.contacts[0].thisCollider.bounds.extents.y * 2)) && sight == true)
            {
                Attack();
            }
            else if (sight == false)
            {
                if ((transform.rotation.eulerAngles.y < 270.0f + 1) && (transform.rotation.eulerAngles.y > 270.0f - 1))
                {
                    transform.Rotate(0.0f, -(transform.eulerAngles.y - 90), 0.0f);
                }
                else
                {
                    transform.Rotate(0.0f, 270 - transform.eulerAngles.y, 0.0f);
                }
            }
        }
        else if (isInLimitPoint)
        {
            if (coll.contacts[0].otherCollider.CompareTag("Knight"))
            {
                knight = coll.contacts[0].otherCollider.GetComponent<KnightAI>();
                knight.states = enemy_states.SEARCH;
            }
            else if (coll.contacts[0].otherCollider.CompareTag("Berserker"))
            {
                berserker = coll.contacts[0].otherCollider.GetComponent<BerserkerAI>();
                berserker.states = BerserkerAI.enemy_states.RETURNING;
            }
        }
        else if (states == enemy_states.CHASE)
        {
            if (coll.contacts[0].otherCollider.CompareTag("Knight"))
            {
                knight = coll.contacts[0].otherCollider.GetComponent<KnightAI>();
                knight.states = enemy_states.CHASE;
            }
            else if (coll.contacts[0].otherCollider.CompareTag("Berserker"))
            {
                berserker = coll.contacts[0].otherCollider.GetComponent<BerserkerAI>();
                berserker.states = BerserkerAI.enemy_states.CHASE;
            }
        }
    }

    // Trigger that detect collisions with patrol points and limit points
    void OnTriggerEnter(Collider coll)
    {
        // Only detect collisions with patrol points when enemy is patrolling
        if ((coll.gameObject.CompareTag("PatrolPoint")) && (states == enemy_states.PATROL))
        {
            speed = patrolSpeed;
            states = enemy_states.WAIT;
            time = waitTime;
        }
        // If is in a limit he stops and search glitch
        else if ((coll.gameObject.CompareTag("LimitPoint")) && (states != enemy_states.SEARCH))
        {
            speed = searchSpeed;
            states = enemy_states.SEARCH;
            time = searchingTime;
            sight = false;
            isInLimitPoint = true;
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

            if ((Physics.Raycast(ray, out hit, maxSightChase, layerMask)) && hit.collider.gameObject.CompareTag("Player"))
            {
                sight = true;
                speed = chaseSpeed;

                if (states == enemy_states.WAIT || states == enemy_states.PATROL)
                {
                    lastPosition = transform.position;      // We save the point in the patrol to return later
                }
                states = enemy_states.CHASE;
            }
        }
    }

    void Update()
    {

        if (world.doUpdate)
        {
            animator.SetFloat("Speed", speed*speedConstant);
            animator.SetInteger("State", (int)states);
            animator.SetBool("Attack", isInAttack);
            switch (states)
            {
                case enemy_states.WAIT:
                    // ANIMATION OF ROTATION HERE

                    // In patrol point knight turns and continue with the patrol
                    time -= world.lag;
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
                        speed = patrolSpeed;
                        states = enemy_states.PATROL;
                    }
                    break;

                case enemy_states.PATROL:
                    // Patrolling logic
                    transform.Translate(Vector3.forward * speed * world.lag);
                    break;

                // Enemy chases Glitch until reach him, reach a limit point or lose sight of Glitch
                case enemy_states.CHASE:
                    speed = chaseSpeed;
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
                            transform.Rotate(0.0f, 270 - transform.eulerAngles.y, 0.0f);
                            rotationTime = 0.5f;
                        }
                    }

                    rotationTime -= world.lag;
                    if (rotationTime <= 0.0f)
                    {
                        transform.Translate(Vector3.forward * speed * world.lag);
                    }

                    origin = transform.position;
                    origin.y += transform.localScale.y * 0.75f;
                    ray = new Ray(origin, player.transform.position - origin);

                    if (!Physics.Raycast(ray, out hit, maxSightChase, layerMask) || !hit.collider.gameObject.CompareTag("Player"))
                    {
                        speed = searchSpeed;
                        sight = false;
                        states = enemy_states.SEARCH;
                        time = searchingTime;
                    }
                    else if (Vector3.Distance(playerPos.position, transform.position) <= maxSightAttack)
                    {
                        speed = attackSpeed;
                        states = enemy_states.ATTACK;
                        isInAttack = true;
                    }

                    break;

                // Enemy attacks Glitch
                case enemy_states.ATTACK:

                    // Attacking logic

                    // If distance to Glitch is plus than chase field of view then changes to Search state
                    // else if Glitch isn't in attack scope then enemy chases him
                    if ((Vector3.Distance(playerPos.position, transform.position) > maxSightChase) && !isInAttack)
                    {
                        speed = searchSpeed;
                        sight = false;
                        states = enemy_states.SEARCH;
                        time = searchingTime;
                    }
                    else if (Vector3.Distance(playerPos.position, transform.position) > maxSightAttack && !isInAttack)
                    {
                        speed = chaseSpeed;
                        states = enemy_states.CHASE;
                    }
                    break;

                // Enemy looks at one side and the other looking for Glitch. After 5 seconds he returns to last patrol position 
                // and continues patrolling.
                case enemy_states.SEARCH:

                    // Searching logic
                    // SEARCH ANIMATION HERE
                    time -= world.lag;
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
                        states = enemy_states.RETURNING;
                        searchRotationTime = 1.0f;
                        isInLimitPoint = false;
                    }

                    origin = transform.position;
                    origin.y += transform.localScale.y * 0.75f;
                    ray = new Ray(origin, player.transform.position - origin);

                    if ((Physics.Raycast(ray, out hit, maxSightSearch, layerMask)) && (hit.collider.gameObject.CompareTag("Player")) && (sight == true))
                    {
                        speed = chaseSpeed;
                        states = enemy_states.CHASE;
                        isInLimitPoint = false;
                    }
                    break;

                case enemy_states.RETURNING:
                    speed = patrolSpeed;
                    transform.Translate(Vector3.forward * speed * world.lag);
                    if ((transform.position.x < lastPosition.x + 1) && (transform.position.x > lastPosition.x - 1))
                    {
                        sight = false;
                        speed = patrolSpeed;
                        states = enemy_states.PATROL;
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
        if (animator.GetInteger("DeadRandom") == -1 || animator.GetInteger("DeadRandom") == 3)
        {
            animator.SetInteger("DeadRandom", Random.Range(0, 3));
        }
        else
        {
            animator.SetInteger("DeadRandom", 3);
        }
    }

    public void HittedTrigger()
    {
        attacked = false;
        if (states != enemy_states.DEATH)
        {
            speed = chaseSpeed;
            states = enemy_states.CHASE;
        }
    }

    public void AttackTrigger()
    {
        swordCollider.enabled = false;
        isInAttack = false;
    }

    public void BeginAttackTrigger()
    {
        swordCollider.enabled = true;
    }

    public void Attacked()
    {
        attacked = true;
        speed = attackSpeed;
        states = enemy_states.HITTED;
        --lives;
        SoundManager.instance.PlaySingle(hitSound);

        if (lives <= 0)
        {
            states = enemy_states.DEATH;
            rigid.isKinematic = true;
            collider.enabled = false;
            swordCollider.enabled = false;
            fieldOfView.enabled = false;
            headCollider.enabled = false;
            isInAttack = false;
        }

        // To impulse player from enemy
        player.ReactToAttack(transform.position.x);
    }

    public void Attack()
    {
        isInAttack = false;
        player.DecrementLives(damageAttack);
        sight = false;
        if (player.lives > 0)
        {
            speed = searchSpeed;
            time = searchingTime;
            states = enemy_states.SEARCH;
        }
        else
        {
            speed = patrolSpeed;
            time = waitTime;
            states = enemy_states.WAIT;
        }
    }
}
