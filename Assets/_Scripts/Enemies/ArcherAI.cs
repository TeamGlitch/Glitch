using UnityEngine;
using System.Collections;

public class ArcherAI : MonoBehaviour {

	enum enemy_states
    {
        WAIT,
        SHOOT,
        MELEE_ATTACK,
        CHASE,
        TURN,
        DEATH
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

    // Variables
    public bool sight = false;
    public float speed = 0.0f;
    public Player player;
    public bool motionless = false;
    public World world;
    public GameObject arrow;				//Reference to an arrow prefab
    public BoxCollider collider;
    public Rigidbody rigid;
    public BoxCollider fieldOfView;

    private ObjectPool arrowPool;			//Arrows pool
    private ArrowScript arrowLogic;
    private Vector3 initialPosition;
    private enemy_states states = enemy_states.WAIT;
    private Ray ray;
    private RaycastHit hit;
    private float rotationTime = 0.0f;
    private float searchRotationTime = 1.0f;
    private bool returning = false;
    private float meleeDamage = 1.0f;
    private Vector3 origin;
    private Animator animator;

    void OnCollisionEnter(Collision coll)
    {
        if ((states != enemy_states.DEATH) && (coll.gameObject.CompareTag("Player")))
        {
            // If is attacking a collison hurts player
            if (states == enemy_states.MELEE_ATTACK)
            {
                player.DecrementLives(meleeDamage);
                
                // To impulse player from enemy
                player.ReactToAttack(transform.position.x);
            }
            else
            {
                Ray ray = new Ray(transform.position, player.transform.position - transform.position);
                Physics.Raycast(ray, out hit);

                // The ray is from archer to player, then collides down of player (-transform.up = down)
                if (hit.normal == -transform.up)
                {
                    states = enemy_states.DEATH;
                    animator.SetBool("Dead", true);
                    rigid.isKinematic = true;
                    collider.enabled = false;
                    fieldOfView.enabled = false;
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
            origin = transform.position;
            origin.y += transform.localScale.y*0.75f;

            ray = new Ray(origin, player.transform.position - origin);
            //Debug.DrawRay(origin, player.transform.position - origin);
            if ((Physics.Raycast(ray, out hit) && (sight == false)) && (hit.collider.gameObject.CompareTag("Player")))
            {
                animator.SetBool("Sighted", true);
                sight = true;
                speed = shootSpeed;
                initialPosition = transform.position;      // We save the initial point to return later
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
            animator.SetBool("Sighted", false);
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

    void Start()
    {
        arrowPool = new ObjectPool(arrow);
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (states != enemy_states.DEATH)
        {
            animator.SetInteger("State", (int)states);

            if (player.playerController.state != PlayerController.player_state.DEATH)
            {

                switch (states)
                {

                    // Enemy shoot arrows to Glitch
                    case enemy_states.SHOOT:

                        // Shooting logic
                        if ((arrow == null) || (!arrow.activeInHierarchy))
                        {
                            animator.SetBool("Shoot", true);
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
                        break;

                    // Enemy chases Glitch until reach him or lose sight of Glitch
                    case enemy_states.CHASE:

                        // Chasing logic
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

                    case enemy_states.TURN:
                        //animator.SetBool("Turn", true);
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
                        states = enemy_states.WAIT;
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

    public void ShootedTrigger()
    {
        origin = transform.position;
        origin.y += transform.localScale.y * 0.75f;

        arrow = arrowPool.getObject();
        arrowLogic = arrow.GetComponent<ArrowScript>();
        arrow.transform.position = origin;
        arrowLogic.player = player;
        float x = origin.x - hit.point.x;
        float y = origin.y - hit.point.y;
        float alfa = Mathf.Atan(y / x);
        alfa = (180.0f * alfa) / 3.14f;

        if (player.transform.position.x > origin.x)
        {
            arrowLogic.transform.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
            arrowLogic.transform.Rotate(0.0f, 0.0f, alfa);
            arrowLogic.isInLeft = false;
        }
        else
        {
            arrowLogic.transform.eulerAngles = new Vector3(0.0f, 180.0f, 90.0f);
            arrowLogic.transform.Rotate(0.0f, 0.0f, -alfa);
            arrowLogic.isInLeft = true;
        }
        arrow.SetActive(true);
        animator.SetBool("Shoot", false);
    }

    public void TurnTrigger()
    {

        animator.SetBool("Turn", false);
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
        states = enemy_states.WAIT;
    }

    public void DeadRandomTrigger()
    {
        animator.SetInteger("DeadRandom", Random.Range(0, 4));
    }
}
