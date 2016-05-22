using UnityEngine;
using System.Collections;

public class ArcherAI : MonoBehaviour {

	public enum enemy_states
    {
        WAIT,
        SHOOT,
        MELEE_ATTACK,
        CHASE,
        TURN,
        DEATH,
        HITTED
    }

    // Constants
    private const float maxSightChase = 9.0f;
    private const float maxSightPersecution = 10.0f;
    private const float maxSightMeleeAttack = 2.5f;
    private const float maxSightShoot = 20.0f;
    private const float chaseSpeed = 10.0f;
    private const float waitSpeed = 0.0f;
    private const float meleeAttackSpeed = 10.0f;
    private const float shootSpeed = 6.0f;
    private const float speedConstant = 0.5f;

    // Variables
    public enemy_states states = enemy_states.WAIT;
    public bool sight = false;
    public float speed = 0.0f;
    public Player player;
    public bool motionless = false;
    public World world;
    public GameObject arrow;				//Reference to an arrow prefab
    public Rigidbody rigid;
    public BoxCollider collider;
    public BoxCollider fieldOfView;
    public BoxCollider kickCollider;
    public BoxCollider headCollider;
	public AudioClip hitSound;

    private ObjectPool arrowPool;			//Arrows pool
    private ArrowScript arrowLogic;
    private Ray ray;
    private RaycastHit hit;
    private float rotationTime = 0.0f;
    private float searchRotationTime = 1.0f;
    private bool returning = false;
    private float meleeDamage = 1.0f;
    private Vector3 origin;
    private Animator animator;
    private float timePerKick = 0.0f;
    private bool shooted = false;
    private int layerMask = (~((1 << 13) | (1 << 2) | (1 << 11))) | (1 << 9) | (1 << 0);

    // Trigger that detect player and change the state to Shoot
    void OnTriggerStay(Collider coll)
    {
        // If is the player and between player and archer isn't anything then shoot him
        if (coll.CompareTag("Player"))
        {
            origin = transform.position;
            origin.y += transform.localScale.y*0.75f;
            ray = new Ray(origin, player.transform.position - origin);

            if (Vector3.Distance(player.transform.position, transform.position) <= maxSightMeleeAttack)
            {
                speed = meleeAttackSpeed;
                timePerKick = 0.0f;
                states = enemy_states.MELEE_ATTACK;
            }
            else if (Physics.Raycast(ray, out hit, maxSightShoot, layerMask) && (sight == false) && (states != enemy_states.HITTED) && (hit.collider.gameObject.CompareTag("Player")))
            {
                sight = true;
                speed = shootSpeed;
                states = enemy_states.SHOOT;
            }
        }
    }

    // Trigger for when she loses of sight Glitch 
    void OnTriggerExit(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            sight = false;
            if (Vector3.Distance(player.transform.position, transform.position) < maxSightShoot)
            {
                states = enemy_states.TURN;
            }
            else
            {
                speed = waitSpeed;
                states = enemy_states.WAIT;
            }
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if ((states != enemy_states.DEATH) && (sight == true) && (coll.contacts[0].thisCollider.CompareTag("Archer")) && (coll.contacts[0].otherCollider.CompareTag("Player")))
        {
            Kick();
        }
        else if ((sight == false) && (coll.contacts[0].thisCollider.CompareTag("Archer")) && (coll.contacts[0].otherCollider.CompareTag("Player")))
        {
            states = enemy_states.TURN;
        }
    }

    void Start()
    {
        arrowPool = new ObjectPool(arrow);
        animator = GetComponent<Animator>();
        animator.SetInteger("DeadRandom", -1);
    }

    void Update()
    {
        if (world.doUpdate)
        {
            animator.SetInteger("State", (int)states);
            animator.SetFloat("Speed", speed * speedConstant);
           
            switch (states)
            {
                // Enemy shoot arrows to Glitch
                case enemy_states.SHOOT:

                    // Shooting logic
                    if (player.playerController.state == PlayerController.player_state.DEATH)
                    {
                        states = enemy_states.WAIT;
                        sight = false;
                    }
                    else if ((player.playerController.state != PlayerController.player_state.DEATH) && ((arrow == null) || (!arrow.activeInHierarchy)))
                    {
                        animator.SetBool("Shoot", true);
                        shooted = false;
                    }

                    if (shooted == false)
                    {
                        origin = transform.position;
                        origin.y += collider.bounds.extents.y * 2 * 0.75f;

                        float x = origin.x - hit.point.x;
                        float y = origin.y - hit.point.y;
                        float alfa = Mathf.Atan(y / x);
                        alfa = (180.0f * alfa) / Mathf.PI;
                        print(alfa);
                        // ShootLevel 1->Up, 2->Down, 0->Middle 
                        if (player.transform.position.x > origin.x)
                        {
                            if (alfa > 15.0f)
                            {
                                animator.SetInteger("ShootLevel", 1);
                            }
                            else if (alfa < -15.0f)
                            {
                                animator.SetInteger("ShootLevel", 2);
                            }
                            else
                            {
                                animator.SetInteger("ShootLevel", 0);
                            }
                        }
                        else
                        {
                            if (-alfa > 15.0f)
                            {
                                animator.SetInteger("ShootLevel", 1);
                            }
                            else if (-alfa < -15.0f)
                            {
                                animator.SetInteger("ShootLevel", 2);
                            }
                            else
                            {
                                animator.SetInteger("ShootLevel", 0);
                            }
                        }
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

                    if (Vector3.Distance(player.transform.position, transform.position) <= maxSightMeleeAttack)
                    {
                        speed = meleeAttackSpeed;
                        timePerKick = 0.0f;
                        states = enemy_states.MELEE_ATTACK;
                    }
                    break;

                // Enemy chases Glitch until reach him or lose sight of Glitch
                case enemy_states.CHASE:

                    // Chasing logic
                    rotationTime -= world.lag;
                    if (rotationTime <= 0.0f)
                    {
                        transform.Translate(Vector3.forward * speed * world.lag);
                    }

                    if (Vector3.Distance(player.transform.position, transform.position) > maxSightPersecution)
                    {
                        speed = shootSpeed;
                        states = enemy_states.SHOOT;
                    }
                    else if (Vector3.Distance(player.transform.position, transform.position) <= maxSightMeleeAttack)
                    {
                        speed = meleeAttackSpeed;
                        timePerKick = 0.0f;
                        states = enemy_states.MELEE_ATTACK;
                    }

                    break;

                case enemy_states.TURN:
                    //animator.SetBool("Turn", true);
                    if ((player.transform.position.x > transform.position.x) && (transform.eulerAngles.y > 269.0f))
                    {
                        transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
                        rotationTime = 0.5f;
                    }
                    else if ((player.transform.position.x < transform.position.x) && (transform.eulerAngles.y < 91.0f))
                    {
                        transform.eulerAngles = new Vector3(0.0f, 270.0f, 0.0f);
                        rotationTime = 0.5f;
                    }
                    speed = waitSpeed;
                    states = enemy_states.WAIT;
                    break;

                // Enemy attacks to Glitch with her daggers
                case enemy_states.MELEE_ATTACK:
                    // Melee attack logic
                    timePerKick -= world.lag;
                    if (timePerKick <= 0.0f)
                    {
                        animator.SetBool("Attack", true);
                    }

                    if (Vector3.Distance(player.transform.position, transform.position) > maxSightMeleeAttack)
                    {
                        if (motionless == false)
                        {
                            speed = chaseSpeed;
                            states = enemy_states.CHASE;
                        }
                        else
                        {
                            speed = shootSpeed;
                            states = enemy_states.SHOOT;
                        }
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

    public void ShootedTrigger()
    {
        if (shooted == false)
        {
            origin = transform.position;
            origin.y += collider.bounds.extents.y * 2 * 0.75f;

            arrow = arrowPool.getObject();
            arrowLogic = arrow.GetComponent<ArrowScript>();
            arrow.transform.position = origin;
            arrowLogic.player = player;
            arrowLogic.world = world;
            float x = origin.x - hit.point.x;
            float y = origin.y - hit.point.y;
            float alfa = Mathf.Atan(y / x);
            alfa = (180.0f * alfa) / Mathf.PI;

            if (player.transform.position.x > origin.x)
            {
                if (alfa > 45.0f)
                {
                    alfa = 45.0f;
                }
                else if (alfa < -45.0f)
                {
                    alfa = -45.0f;
                }
                arrowLogic.transform.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
                arrowLogic.transform.Rotate(0.0f, 0.0f, alfa);
                arrowLogic.isInLeft = false;
            }
            else
            {
                if (-alfa > 45.0f)
                {
                    alfa = -45.0f;
                }
                else if (-alfa < -45.0f)
                {
                    alfa = 45.0f;
                }
                arrowLogic.transform.eulerAngles = new Vector3(0.0f, 180.0f, 90.0f);
                arrowLogic.transform.Rotate(0.0f, 0.0f, -alfa);
                arrowLogic.isInLeft = true;
            }
            arrow.SetActive(true);
            animator.SetBool("Shoot", false);
            shooted = true;
        }
    }

    public void TurnTrigger()
    {

        //animator.SetBool("Turn", false);
        if ((player.transform.position.x > transform.position.x) && (transform.eulerAngles.y == 270.0f))
        {
            transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
            rotationTime = 0.5f;
        }
        else if ((player.transform.position.x < transform.position.x) && (transform.eulerAngles.y == 90.0f))
        {
            transform.eulerAngles = new Vector3(0.0f, 270.0f, 0.0f);
            rotationTime = 0.5f;
        }
        speed = waitSpeed;
        states = enemy_states.WAIT;
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

    public void FinishKickTrigger()
    {
        animator.SetBool("Attack", false);
        timePerKick = 1.0f;
    }

    public void HittedTrigger()
    {
        speed = waitSpeed;
        states = enemy_states.DEATH;
    }

    public void Kick()
    {
        player.DecrementLives(meleeDamage);
        sight = false;
        speed = waitSpeed;
        states = enemy_states.WAIT;
    }

    public void Defeated()
    {
        sight = false;
        speed = meleeAttackSpeed;
        states = enemy_states.HITTED;
        animator.SetBool("Attack", false);
        animator.SetBool("Shoot", false);
        rigid.isKinematic = true;
        collider.enabled = false;
        kickCollider.enabled = false;
        fieldOfView.enabled = false;
        headCollider.enabled = false;
        SoundManager.instance.PlaySingle(hitSound);

        // To impulse player from enemy
        player.ReactToAttack(transform.position.x);
    }
}
