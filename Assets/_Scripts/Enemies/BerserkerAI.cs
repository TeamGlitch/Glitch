using UnityEngine;
using System.Collections;

public class BerserkerAI : MonoBehaviour
{

    public enum enemy_states
    {
        WAIT,
        CHASE,
        ATTACK,
        DEATH,
        SLIP,
        IMPACT,
        HITTED,
        RETURNING,
        FALL
    }

    // Constants
    private const float maxSightChase = 14.0f;
    private const float maxSightAttack = 2.5f;
    private const float waitSpeed = 2.0f;
    private const float chaseSpeed = 10.0f;
    private const float attackSpeed = 3.0f;
    private const float fallSpeed = 5.0f;
    private const float walkSpeed = 2.5f;
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
    public AudioClip hitSound;

    private Transform playerPos;
    private Vector3 initialPosition;
    private Ray ray;
    private RaycastHit hit;
    private float rotationTime = 0.0f;
    private int lives = 3;
    private Animator animator;
    private float timePerAttack = 0.0f;
    private Vector3 origin;
    private float time = waitTime;
    private bool isInAttack = false;
    private bool attackSucces = false;
    private bool isInLimit = false;
    private int layerMask = (~((1 << 13) | (1 << 2) | (1 << 11))) | (1 << 9) | (1 << 0);

    void Start()
    {
        playerPos = player.GetComponent<Transform>();
        animator = GetComponent<Animator>();
        axeCollider1.enabled = false;
        axeCollider2.enabled = false;
        initialPosition = transform.position;
        animator.SetInteger("DeadRandom", -1);
    }

    void OnCollisionEnter(Collision coll)
    {
        BerserkerAI berserker;
        KnightAI knight;

        if ((coll.contacts[0].thisCollider.CompareTag("Berserker")) && (coll.contacts[0].otherCollider.CompareTag("Player")))
        {
            if (player.transform.position.y >= (transform.position.y + coll.contacts[0].thisCollider.bounds.extents.y * 2))
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
        else if (isInLimit)
        {
            if (coll.contacts[0].otherCollider.CompareTag("Knight"))
            {
                knight = coll.contacts[0].otherCollider.GetComponent<KnightAI>();
                knight.states = KnightAI.enemy_states.SEARCH;
            }
            else if (coll.contacts[0].otherCollider.CompareTag("Berserker"))
            {
                berserker = coll.contacts[0].otherCollider.GetComponent<BerserkerAI>();
                berserker.states = enemy_states.RETURNING;
            }
        }
        else if (states == enemy_states.CHASE)
        {
            if (coll.contacts[0].otherCollider.CompareTag("Knight"))
            {
                knight = coll.contacts[0].otherCollider.GetComponent<KnightAI>();
                knight.states = KnightAI.enemy_states.CHASE;
            }
            else if (coll.contacts[0].otherCollider.CompareTag("Berserker"))
            {
                berserker = coll.contacts[0].otherCollider.GetComponent<BerserkerAI>();
                berserker.states = enemy_states.CHASE;
            }
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("LimitPoint") && (states != enemy_states.HITTED))
        {
            isInLimit = true;
        }
    }

    void OnTriggerStay(Collider coll)
    {
        if (coll.gameObject.CompareTag("LimitPoint") && (states != enemy_states.HITTED))
        {
            isInLimit = true;
        }

        // if the enemy hasn't seen Glitch he patrols and if he detects him with the raycast
        // then changes his state to Chase and changes his speed too.
        if ((sight == false) && coll.gameObject.CompareTag("Player") && ((states == enemy_states.WAIT) || (states == enemy_states.RETURNING) || (states == enemy_states.CHASE)))
        {
            origin = transform.position;
            origin.y = collider.bounds.extents.y;
            ray = new Ray(origin, player.transform.position - origin);

            if ((Physics.Raycast(ray, out hit, maxSightChase, layerMask)) && hit.collider.gameObject.CompareTag("Player"))
            {
                sight = true;
                speed = chaseSpeed;
                states = enemy_states.CHASE;
            }
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            sight = false;
        }
        else if (coll.gameObject.CompareTag("LimitPoint"))
        {
            if (isInLimit)
            {
                isInLimit = false;
            }
            else if ((coll.GetComponent<LimitPoint>().fall) && (states == enemy_states.CHASE))
            {
                states = enemy_states.SLIP;
                sight = false;
                speed = fallSpeed;
            }
        }
        else if (coll.gameObject.CompareTag("Death"))
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {

        if (world.doUpdate)
        {
            animator.SetFloat("Speed", speed * speedConstant);
            animator.SetInteger("State", (int)states);
            animator.SetBool("Attack", isInAttack);
            switch (states)
            {
                case enemy_states.WAIT:
                    // IDLE
                    break;

                // Enemy chases Glitch until reach him, reach a limit point or lose sight of Glitch
                case enemy_states.CHASE:
                    speed = chaseSpeed;
                    // Chasing logic
                    transform.Translate(Vector3.forward * speed * world.lag);

                    if ((sight == true) && (Vector3.Distance(playerPos.position, transform.position) <= maxSightAttack))
                    {
                        speed = attackSpeed;
                        isInAttack = true;
                        states = enemy_states.ATTACK;
                    }

                    break;

                // Enemy attacks Glitch
                case enemy_states.ATTACK:

                    // Attacking logic

                    // If distance to Glitch is plus than chase field of view then changes to Search state
                    // else if Glitch isn't in attack scope then enemy chases him
                    if ((Vector3.Distance(playerPos.position, transform.position) > maxSightAttack) && !isInAttack)
                    {
                        speed = chaseSpeed;
                        states = enemy_states.CHASE;
                    }
                    break;

                case enemy_states.HITTED:
                    // State to put particles or something
                    break;

                case enemy_states.RETURNING:
                    speed = walkSpeed;
                    if (!isInAttack)
                    {
                        if ((transform.position.x < initialPosition.x + 1) && (transform.position.x > initialPosition.x - 1))
                        {
                            transform.Rotate(0.0f, 270 - transform.eulerAngles.y, 0.0f);
                            speed = waitSpeed;
                            states = enemy_states.WAIT;
                        }
                        else
                        {
                            transform.Translate(Vector3.forward * speed * world.lag);
                        }

                        if (attackSucces)
                        {
                            if ((transform.rotation.eulerAngles.y < 270.0f + 1) && (transform.rotation.eulerAngles.y > 270.0f - 1))
                            {
                                if (initialPosition.x > transform.position.x)
                                {
                                    transform.Rotate(0.0f, -(transform.eulerAngles.y - 90), 0.0f);
                                }
                            }
                            else
                            {
                                if (initialPosition.x < transform.position.x)
                                {
                                    transform.Rotate(0.0f, 270 - transform.eulerAngles.y, 0.0f);
                                }
                            }
                            attackSucces = false;
                        }
                    }
                    break;

                case enemy_states.IMPACT:
                    speed = fallSpeed;
                    time -= world.lag;
                    if (time <= 0.0f)
                    {
                        sight = false;
                        time = waitTime;
                        speed = walkSpeed;
                        states = enemy_states.RETURNING;
                    }
                    break;

                case enemy_states.SLIP:
                    transform.Translate(Vector3.forward * speed * world.lag);
                    break;

                case enemy_states.FALL:
                    transform.Translate(Vector3.down * speed * world.lag);
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

    public void RecoverTrigger()
    {
        rigid.isKinematic = false;
        if ((transform.rotation.eulerAngles.y < 270.0f + 1) && (transform.rotation.eulerAngles.y > 270.0f - 1))
        {
            transform.Rotate(0.0f, -(transform.eulerAngles.y - 90), 0.0f);
        }
        else
        {
            transform.Rotate(0.0f, 270 - transform.eulerAngles.y, 0.0f);
        }
    }

    public void HittedTrigger()
    {
        if (states != enemy_states.DEATH)
        {
            rigid.isKinematic = false;
            collider.enabled = true;
            fieldOfView.enabled = true;
            headCollider.enabled = true;
            speed = chaseSpeed;
            states = enemy_states.CHASE;
        }
    }

    public void AttackTrigger()
    {
        axeCollider1.enabled = false;
        axeCollider2.enabled = false;
        isInAttack = false;

        if (attackSucces)
        {
            speed = walkSpeed;
            states = enemy_states.RETURNING;
        }
    }

    public void BeginAttackTrigger()
    {
        axeCollider1.enabled = true;
        axeCollider2.enabled = true;
    }

    public void SlipTrigger()
    {
        speed = fallSpeed;
        states = enemy_states.FALL;
    }

    public void Attacked()
    {
        speed = attackSpeed;
        states = enemy_states.HITTED;
        rigid.isKinematic = true;
        collider.enabled = false;
        axeCollider1.enabled = false;
        axeCollider2.enabled = false;
        fieldOfView.enabled = false;
        headCollider.enabled = false;
        --lives;
        SoundManager.instance.PlaySingle(hitSound);

        if (lives <= 0)
        {
            states = enemy_states.DEATH;
            isInAttack = false;
        }
        else
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
        }

        // To impulse player from enemy
        player.ReactToAttack(transform.position.x);
    }

    public void Attack()
    {
        player.DecrementLives(damageAttack);
        isInAttack = false;
        sight = false;
        speed = walkSpeed;
        attackSucces = true;
    }
}