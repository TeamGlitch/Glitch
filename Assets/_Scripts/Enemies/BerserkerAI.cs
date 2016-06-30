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
    public GameObject item;

    private static GameObject item1;
    private static GameObject item2;
    private static GameObject item3;
    private ObjectPool itemPool;
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
        itemPool = new ObjectPool(item);
        initialPosition = transform.position;
        animator.SetInteger("DeadRandom", -1);
    }

    void OnCollisionEnter(Collision coll)
    {
        BerserkerAI berserker;
        KnightAI knight;

        // If collides with player, berserker is kinematic. 
        // Else if collides with other enemy decides what to do depending in te behaviour of the other enemy
        if (coll.contacts[0].otherCollider.CompareTag("Player"))
        {
            rigid.isKinematic = true;
            if (player.transform.position.y >= (transform.position.y + coll.contacts[0].thisCollider.bounds.extents.y * 2))
            {
                Attacked();
            }
            else if (sight == false)
            {
                if ((states != enemy_states.IMPACT) && (states != enemy_states.SLIP))
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

    void OnCollisionStay(Collision coll)
    {
        BerserkerAI berserker;
        KnightAI knight;

        if (isInLimit)
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

    void OnCollisionExit(Collision coll)
    {
        // If exit collides with glitch, enemy return to non kinematic
        if (coll.collider.gameObject.CompareTag("Player"))
        {
            rigid.isKinematic = false;
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("LimitPoint") && (states == enemy_states.HITTED) && (!coll.GetComponent<LimitPoint>().fall))
        {
            isInLimit = true;
        }
    }

    void OnTriggerStay(Collider coll)
    {
        if (coll.gameObject.CompareTag("LimitPoint") && (states == enemy_states.HITTED) && (!coll.GetComponent<LimitPoint>().fall))
        {
            isInLimit = true;
        }

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

    // Controls slip and death when exits a collider 
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
        // To control fps
        if (world.doUpdate)
        {
            // Update state and speed in animator
            animator.SetFloat("Speed", speed * speedConstant);
            animator.SetInteger("State", (int)states);
            animator.SetBool("Attack", isInAttack);
            switch (states)
            {
                case enemy_states.WAIT:
                    // IDLE
                    break;

                // Enemy chases Glitch until reach him, reach a limit point
                case enemy_states.CHASE:
                   
                    // Chasing logic

                    speed = chaseSpeed;
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

                    // If Glitch isn't in attack scope then enemy chases him
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

    // Trigger of death animation
    public void DeadRandomTrigger()
    {
        int random = animator.GetInteger("DeadRandom");
        while (random == animator.GetInteger("DeadRandom"))
        {
            animator.SetInteger("DeadRandom", Random.Range(0, 3));
        }
    }

    // Trigger in recover animation
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

        sight = false;
        speed = walkSpeed;
        states = enemy_states.RETURNING;
    }

    // Trigger of hit animation
    public void HittedTrigger()
    {
        if (states != enemy_states.DEATH)
        {
            rigid.isKinematic = false;
            collider.enabled = true;
            fieldOfView.enabled = true;
            headCollider.enabled = true;

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

            speed = chaseSpeed;
            states = enemy_states.CHASE;
        }
    }


    // Trigger of end attack, disables colliders of axes
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


    // Trigger of begin attack, enables colliders of axes
    public void BeginAttackTrigger()
    {
        axeCollider1.enabled = true;
        axeCollider2.enabled = true;
    }

    // Trigger of begin standup
    public void BeginImpactRecover()
    {
        collider.center = new Vector3(0.0f, 0.095f, 0.0f);
        collider.size = new Vector3(0.1f, 0.18f, 0.05f);
    }

    // Trigger of standup animation
    public void InGroundTrigger()
    {
        collider.center = new Vector3(0.0f, 0.066f, -0.11f);
        collider.size = new Vector3(0.1f, 0.12f, 0.05f);
    }

    // Trigger of slip animation
    public void SlipTrigger()
    {
        speed = fallSpeed;
        states = enemy_states.FALL;
    }

    // Logic of attacked received. Deactivates temporary all the colliders.
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
            dropItems();
            states = enemy_states.DEATH;
            isInAttack = false;
        }

        // To impulse player from enemy
        player.ReactToAttack(transform.position.x);
    }

    // Logic of attack
    public void Attack()
    {
        player.DecrementLives(damageAttack);
        isInAttack = false;
        sight = false;
        speed = walkSpeed;
        attackSucces = true;
    }

    public void dropItems()
    {
        if (item1 == null || item1.activeInHierarchy)
        {
            item1 = itemPool.getObject();
        }
        else
        {
            item1.SetActive(true);
        }
        CollectibleScript itemScript = item1.GetComponent<CollectibleScript>();
        itemScript.player = player;
        itemScript.isFalling = true;
        float rand = Random.Range(-5.0f, 5.0f);
        item1.transform.position = new Vector3(transform.position.x, transform.position.y + collider.bounds.extents.y, 0.0f);
        Rigidbody rigid = item1.GetComponent<Rigidbody>();
        BoxCollider boxCollider = item1.GetComponent<BoxCollider>();
        rigid.isKinematic = false;
        boxCollider.enabled = false;
        rigid.AddForce(rand, 15.0f, 0.0f, ForceMode.Impulse);
        itemScript.Parable();

        if (item2 == null || item2.activeInHierarchy)
        {
            item2 = itemPool.getObject();
        }
        else
        {
            item2.SetActive(true);
        }
        itemScript = item2.GetComponent<CollectibleScript>();
        itemScript.player = player;
        itemScript.isFalling = true;
        rand = Random.Range(-5.0f, 5.0f);
        item2.transform.position = new Vector3(transform.position.x, transform.position.y + collider.bounds.extents.y, 0.0f);
        rigid = item2.GetComponent<Rigidbody>();
        boxCollider = item2.GetComponent<BoxCollider>();
        rigid.isKinematic = false;
        boxCollider.enabled = false;
        rigid.AddForce(rand, 15.0f, 0.0f, ForceMode.Impulse);
        itemScript.Parable();

        if (item3 == null || item3.activeInHierarchy)
        {
            item3 = itemPool.getObject();
        }
        else
        {
            item3.SetActive(true);
        }
        itemScript = item3.GetComponent<CollectibleScript>();
        itemScript.player = player;
        itemScript.isFalling = true;
        rand = Random.Range(-5.0f, 5.0f);
        item3.transform.position = new Vector3(transform.position.x, transform.position.y + collider.bounds.extents.y, 0.0f);
        item3.GetComponent<Rigidbody>().AddForce(rand, 1.0f, 0.0f, ForceMode.Impulse);
        rigid = item3.GetComponent<Rigidbody>();
        boxCollider = item3.GetComponent<BoxCollider>();
        rigid.isKinematic = false;
        boxCollider.enabled = false;
        rigid.AddForce(rand, 15.0f, 0.0f, ForceMode.Impulse);
        itemScript.Parable();
    }
}