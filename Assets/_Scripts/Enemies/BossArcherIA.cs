using UnityEngine;
using System.Collections;

public class BossArcherIA : MonoBehaviour
{

    #region Variable Declaration

    public enum bossArcherIA
    {
        IDLE,
        SHOOTING,
        PRESHOOT,
        POSTSHOOT,
        MOVING,
        FALLING_JUMP,
        JUMPING,
        TURNING_RIGHT_TO_RUN,
        TURNING_LEFT_TO_RUN,
        TURNING_RIGHT_TO_STOP,
        TURNING_LEFT_TO_STOP,
        HITTED,
        DEAD
    }

    public enum bossArcherPos
    {
        MAXLEFT,
        MEDIUMLEFT,
        MEDIUMRIGHT,
        MAXRIGHT
    }

    public enum shootTypes
    {
        ULTRA_DIFFICULT_ULTRA,
        LEFT_TO_RIGHT,
        RIGHT_TO_LEFT,
        SIDES_TO_MIDDLE,
        THREE_NEAR_GLITCH,
        FIVE_NEAR_GLITCH,
        SEVEN_NEAR_GLITCH,
        NINE_NEAR_GLITCH,
        ELEVEN_NEAR_GLITCH
    }

    public AudioClip hit;
    public AudioClip scream;
    public float horizontalVelocity = 10.0f;
    public Door door;
    public bool start = false;
    public float timeInPreShoot = 2.0f;
    public float timeInPostShoot = 2.0f;
    private float timeSinceStateChanged;
    [SerializeField]
    private bool movingRight = true;

    [SerializeField]
    private bossArcherIA bossState;
    [SerializeField]
    private bossArcherPos bossPos;
    private Rigidbody rigidbody;
    private BoxCollider boxCollider;

    public Transform arrowPool;
    private Transform[] arrows;
    private BossArrowScript[] arrowsScript;

    private float maxLeft = -36f;
    private float maxRight = 36f;
    public float prepareArrowYPos = 12f;

    public float timeToShootArrows = 3f;

    private shootTypes lastShootType;

    public Transform objectiveTransform;

    private Animator animator;

    private int layerMask = (~((1 << 13) | (1 << 2) | (1 << 11) | (1 << 8))) | (1 << 9) | (1 << 0);

    private bool fallingJump = false;
    private bool firstStopPoint = true;

    public int lives = 3;

    private float currentSpecialSpeed = 1f;

    private Vector3 currentStartJumpPoint;
    private Vector3 currentEndJumpPoint;
    private Vector3 currentMiddleJumpPoint;
    public Transform[] EndJumpPoint;
    public Transform[] MiddleJumpPoint;
    private float timeJumping;
    public float timeToJump = 2.0f;

    public float timeToMoveZWhileFall = 1.0f;
    private float startZPosWhenDead;
    private float timeFalling = 0.0f;
    public float endZPosWhenDead = 6f - 5.06f;
    public float startXPosWhenDead;
    public float endXPosWhenDead;
    private bool fallingDead = false;

    public World world;

    private bool slowFPSactivated = false;
    public SlowFPS slowFPS;

    private float timeWhenLastHitted;

    public BossArrowScript upArrow;

	private bool shootInThisPlatform = false;

    public AudioClip bowSound;
    public AudioClip jumpSound;


    #endregion

    #region Init & Update

    // Use this for initialization
    void Start()
    {

        rigidbody = transform.GetComponent<Rigidbody>();
        boxCollider = transform.GetComponent<BoxCollider>();
        timeSinceStateChanged = 0.0f;
        bossPos = bossArcherPos.MEDIUMRIGHT;
        bossState = bossArcherIA.IDLE;
        timeSinceStateChanged = 0.0f;
        timeJumping = 0.0f;

        arrows = new Transform[arrowPool.childCount];
        for (int i = 0; i < arrowPool.childCount; ++i)
        {
            arrows[i] = arrowPool.GetChild(i);
        }
        arrowsScript = new BossArrowScript[arrowPool.childCount];
        for (int i = 0; i < arrows.Length; ++i)
        {
            arrowsScript[i] = arrows[i].GetComponent<BossArrowScript>();
        }
        animator = transform.GetComponent<Animator>();
        firstStopPoint = true;
        slowFPS.SlowFPSChangedStatusEvent += SlowFPSStateChanged;
        timeWhenLastHitted = Time.time;

        upArrow.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (slowFPSactivated && (bossState == bossArcherIA.JUMPING || bossState == bossArcherIA.FALLING_JUMP))
        {
            animator.speed = 0.5f;
        }
        else if (slowFPSactivated)
        {
            animator.speed = currentSpecialSpeed * 0.5f;
        }
        else if (bossState == bossArcherIA.JUMPING || bossState == bossArcherIA.FALLING_JUMP)
        {
            animator.speed = 1f;
        }
        else
        {
            animator.speed = currentSpecialSpeed;
        }
        if (world.doUpdate)
        {
            if (!fallingDead)
                rigidbody.velocity = Vector3.zero;
            switch (bossState)
            {
                case bossArcherIA.MOVING:
                    transform.Translate(Vector3.forward * world.lag * horizontalVelocity);
                    break;

                case bossArcherIA.FALLING_JUMP:
                    if (fallingJump)
                        bossState = bossArcherIA.MOVING;
                    break;

                case bossArcherIA.JUMPING:
                    float perc;
                    timeJumping += world.lag;
                    perc = timeJumping / timeToJump;
                    perc = perc * perc * (3f - 2f * perc);

                    Vector3 firstLerpPoint = Vector3.Lerp(currentStartJumpPoint, currentMiddleJumpPoint, perc);
                    Vector3 secondLerpPoint = Vector3.Lerp(currentMiddleJumpPoint, currentEndJumpPoint, perc);
                    Vector3 currentPosition = Vector3.Lerp(firstLerpPoint, secondLerpPoint, perc);
                    transform.position = currentPosition;

                    if (IsGrounded() && perc >= 0.5f)
                    {
						shootInThisPlatform = false;
                        bossState = bossArcherIA.FALLING_JUMP;
                    }
                    break;

                case bossArcherIA.PRESHOOT:
                    timeSinceStateChanged += world.lag;
                    if (timeSinceStateChanged >= timeInPreShoot)
                    {
                        bossState = bossArcherIA.SHOOTING;
                        animator.speed = currentSpecialSpeed;
                        animator.SetTrigger("Attack");
                    }
                    break;

                case bossArcherIA.SHOOTING:
                    break;

                case bossArcherIA.POSTSHOOT:
                    timeSinceStateChanged += world.lag;
                    if (timeSinceStateChanged >= timeInPostShoot)
                    {
                        int random;
                        switch (bossPos)
                        {
                            case bossArcherPos.MAXLEFT:
                                movingRight = true;
                                break;

                            case bossArcherPos.MEDIUMLEFT:
                                random = Random.Range(1, 3);
                                if (random == 1)
                                {
                                    movingRight = true;
                                }
                                else
                                {
                                    movingRight = false;
                                }
                                break;
                            case bossArcherPos.MEDIUMRIGHT:
                                random = Random.Range(1, 3);
                                if (random == 1)
                                {
                                    movingRight = true;
                                }
                                else
                                {
                                    movingRight = false;
                                }
                                break;
                            case bossArcherPos.MAXRIGHT:
                                movingRight = false;
                                break;
                        }
                        if (movingRight)
                        {
                            bossState = bossArcherIA.TURNING_LEFT_TO_RUN;
                            animator.SetBool("Run", true);
                            animator.speed = currentSpecialSpeed;
                            animator.SetTrigger("TurnLeft");
                        }
                        else
                        {
                            bossState = bossArcherIA.TURNING_RIGHT_TO_RUN;
                            animator.SetBool("Run", true);
                            animator.speed = currentSpecialSpeed;
                            animator.SetTrigger("TurnRight");
                        }
                    }
                    break;

                case bossArcherIA.TURNING_LEFT_TO_RUN:
                case bossArcherIA.TURNING_RIGHT_TO_RUN:
                case bossArcherIA.TURNING_LEFT_TO_STOP:
                case bossArcherIA.TURNING_RIGHT_TO_STOP:
                case bossArcherIA.HITTED:
                    break;

                case bossArcherIA.IDLE:
                    if (start)
                    {
                        bossState = bossArcherIA.PRESHOOT;
                        start = false;
                    }
                    break;

                case bossArcherIA.DEAD:
                    if (fallingDead)
                    {
                        timeFalling += world.lag;
                        float percentage = timeFalling / timeToMoveZWhileFall;
                        if (percentage >= 1.0f)
                            percentage = 1.0f;
                        float xPos = Mathf.Lerp(startXPosWhenDead, endXPosWhenDead, percentage);
                        float zPos = Mathf.Lerp(startZPosWhenDead, endZPosWhenDead, percentage);
                        transform.position = new Vector3(xPos, transform.position.y, zPos);
                    }
                    break;

            }
        }
    }

    #endregion

    #region Colliders

    public void OnTriggerEnter(Collider coll)
    {
        if (coll.transform.name == "JumpPoint" && bossState == bossArcherIA.MOVING && bossState != bossArcherIA.DEAD)
        {
            currentStartJumpPoint = transform.position;
            timeJumping = 0.0f;
            SoundManager.instance.PlaySingle(jumpSound);
            if (bossPos == bossArcherPos.MAXRIGHT)
            {
                bossPos = bossArcherPos.MEDIUMRIGHT;
                currentEndJumpPoint = EndJumpPoint[4].position + new Vector3(-1f, -1f, 0f);
                currentMiddleJumpPoint = MiddleJumpPoint[2].position;
            }
            else if (bossPos == bossArcherPos.MAXLEFT)
            {
                bossPos = bossArcherPos.MEDIUMLEFT;
                currentEndJumpPoint = EndJumpPoint[1].position + new Vector3(+1f, -1f, 0f);
                currentMiddleJumpPoint = MiddleJumpPoint[0].position;
            }
            else if (bossPos == bossArcherPos.MEDIUMLEFT && movingRight)
            {
                bossPos = bossArcherPos.MEDIUMRIGHT;
                currentEndJumpPoint = EndJumpPoint[3].position + new Vector3(1f, -1f, 0f);
                currentMiddleJumpPoint = MiddleJumpPoint[1].position;
            }
            else if (bossPos == bossArcherPos.MEDIUMLEFT && !movingRight)
            {
                bossPos = bossArcherPos.MAXLEFT;
                currentEndJumpPoint = EndJumpPoint[0].position + new Vector3(-1f, -1f, 0f);
                currentMiddleJumpPoint = MiddleJumpPoint[0].position;
            }
            else if (bossPos == bossArcherPos.MEDIUMRIGHT && movingRight)
            {
                bossPos = bossArcherPos.MAXRIGHT;
                currentEndJumpPoint = EndJumpPoint[5].position + new Vector3(1f, -1f, 0f);
                currentMiddleJumpPoint = MiddleJumpPoint[2].position;
            }
            else if (bossPos == bossArcherPos.MEDIUMRIGHT && !movingRight)
            {
                bossPos = bossArcherPos.MEDIUMLEFT;
                currentEndJumpPoint = EndJumpPoint[2].position + new Vector3(-1f, -1f, 0f);
                currentMiddleJumpPoint = MiddleJumpPoint[1].position;
            }
            else
            {
                Debug.Log("HEHE; ERROR");
            }
            bossState = bossArcherIA.JUMPING;
            fallingJump = false;
            animator.SetTrigger("Jump");
        }
        else if (coll.transform.name == "StopPoint" && firstStopPoint && bossState != bossArcherIA.DEAD && !shootInThisPlatform)
        {
            firstStopPoint = false;
        }
        else if (coll.transform.name == "StopPoint" && !firstStopPoint && bossState != bossArcherIA.DEAD && !shootInThisPlatform)
        {
            if (bossPos == bossArcherPos.MEDIUMLEFT || bossPos == bossArcherPos.MEDIUMRIGHT)
                transform.position = new Vector3(transform.position.x, transform.position.y, 17.5f - 5.34f);
            if (movingRight)
            {
                bossState = bossArcherIA.TURNING_RIGHT_TO_STOP;
                animator.speed = currentSpecialSpeed;
                animator.SetTrigger("TurnRight");
            }
            else
            {
                bossState = bossArcherIA.TURNING_LEFT_TO_STOP;
                animator.speed = currentSpecialSpeed;
                animator.SetTrigger("TurnLeft");
            }
            animator.SetBool("Run", false);
        }
    }

    public void OnCollisionEnter(Collision coll)
    {
        if (bossState == bossArcherIA.DEAD && !coll.collider.CompareTag("BossHit") && Time.time - timeWhenLastHitted >= 1f)
        {
            animator.speed = 1f;
            animator.SetTrigger("GroundHitted");
        }
        else if (coll.collider.CompareTag("BossHit") && Time.time - timeWhenLastHitted >= 1f && bossState != bossArcherIA.DEAD)
        {
            SoundManager.instance.PlaySingle(hit);
            timeWhenLastHitted = Time.time;
            transform.localEulerAngles = new Vector3(0f, 180f, 0f);
            animator.speed = 1f;
            --lives;
            if (lives == 0)
            {
                SoundManager.instance.PlaySingle(scream);
                animator.SetTrigger("LastHitted");
                bossState = bossArcherIA.DEAD;
                door.OpenDoor();
            }
            else if (lives == 1)
            {
                timeToShootArrows = 1;
                timeInPreShoot = 0f;
                timeInPostShoot = 0f;
                animator.SetTrigger("Hitted");
                bossState = bossArcherIA.HITTED;
            }
            else if (lives == 2)
            {
                timeToShootArrows = 2;
                timeInPreShoot = 1f;
                timeInPostShoot = 1f;
                animator.SetTrigger("Hitted");
                bossState = bossArcherIA.HITTED;
            }
        }
    }

    public void InsultingAnimationEnded()
    {
        if (lives == 2)
            currentSpecialSpeed = 2f;
        else if (lives == 1)
            currentSpecialSpeed = 3f;

        int random;
        switch (bossPos)
        {
            case bossArcherPos.MAXLEFT:
                movingRight = true;
                break;

            case bossArcherPos.MEDIUMLEFT:
                random = Random.Range(1, 3);
                if (random == 1)
                {
                    movingRight = true;
                }
                else
                {
                    movingRight = false;
                }
                break;
            case bossArcherPos.MEDIUMRIGHT:
                random = Random.Range(1, 3);
                if (random == 1)
                {
                    movingRight = true;
                }
                else
                {
                    movingRight = false;
                }
                break;
            case bossArcherPos.MAXRIGHT:
                movingRight = false;
                break;
        }


        if (!shootInThisPlatform)
	        bossState = bossArcherIA.PRESHOOT;
		else if(movingRight)
        {
            bossState = bossArcherIA.TURNING_LEFT_TO_RUN;
            animator.SetBool("Run", true);
            animator.speed = currentSpecialSpeed;
            animator.SetTrigger("TurnLeft");
        }
        else
        {
            bossState = bossArcherIA.TURNING_RIGHT_TO_RUN;
            animator.SetBool("Run", true);
            animator.speed = currentSpecialSpeed;
            animator.SetTrigger("TurnRight");
        }
        timeSinceStateChanged = 0.0f;
    }

    #endregion

    #region Utils

    private bool IsGrounded()
    {
        bool result;
        result = Physics.Raycast(new Vector3(transform.position.x, transform.position.y + boxCollider.center.y, transform.position.z), -Vector3.up, boxCollider.size.y + 0.1f, 1, QueryTriggerInteraction.Ignore);
        return result;
    }

    public void EndTurning()
    {
        if (bossState == bossArcherIA.TURNING_LEFT_TO_RUN)
        {
            transform.localEulerAngles = new Vector3(0f, 90f, 0f);
            bossState = bossArcherIA.MOVING;
        }
        else if (bossState == bossArcherIA.TURNING_RIGHT_TO_RUN)
        {
            transform.localEulerAngles = new Vector3(0f, 270f, 0f);
            bossState = bossArcherIA.MOVING;
        }
        else if (bossState == bossArcherIA.TURNING_LEFT_TO_STOP)
        {
            transform.localEulerAngles = new Vector3(0f, 180f, 0f);
            bossState = bossArcherIA.PRESHOOT;
        }
        else if (bossState == bossArcherIA.TURNING_RIGHT_TO_STOP)
        {
            transform.localEulerAngles = new Vector3(0f, 180f, 0f);
            bossState = bossArcherIA.PRESHOOT;
        }
        animator.speed = 1;
    }

    public void EndJump()
    {
        animator.speed = currentSpecialSpeed;
        fallingJump = true;
    }

    public void StartFallingAnimationEnded()
    {
        Vector3 auxPos = transform.position + new Vector3(-2f, -2.2f, 0f);
        rigidbody.useGravity = true;
        startXPosWhenDead = endXPosWhenDead = transform.position.x;
        if (transform.position.x >= 16f)
        {
            endXPosWhenDead = startXPosWhenDead + 2f;
        }
        startZPosWhenDead = transform.position.z;
        timeFalling = 0.0f;
        transform.position = auxPos;
        fallingDead = true;
    }

    public void SlowFPSStateChanged()
    {
        if (slowFPSactivated)
        {
            slowFPSactivated = false;
            animator.speed = 1f;
        }
        else
        {
            slowFPSactivated = true;
            animator.speed = 0.5f;
        }
    }

    #endregion

    #region Shoot

    private void PrepareArrows(shootTypes shootType)
    {
        lastShootType = shootType;
        float distance = maxRight - maxLeft;
        float distanceBetweenArrows = distance / (arrows.Length / 2);
        bool shootDependingObjective = false;
        int numberOfArrows = 0;
        switch (shootType)
        {
            case shootTypes.LEFT_TO_RIGHT:
                for (int i = 0; i < arrows.Length; ++i)
                {
                    arrows[i].gameObject.SetActive(false);
                    arrows[i].position = new Vector3(maxLeft + i * distanceBetweenArrows, prepareArrowYPos, 0f);
                    arrows[i].localEulerAngles = new Vector3(0f, 180f, 0f);
                    arrowsScript[i].canMove = false;
                }
                break;

            case shootTypes.RIGHT_TO_LEFT:
                for (int i = 0; i < arrows.Length; ++i)
                {
                    arrows[i].gameObject.SetActive(false);
                    arrows[i].position = new Vector3(maxRight - i * distanceBetweenArrows, prepareArrowYPos, 0f);
                    arrows[i].localEulerAngles = new Vector3(0f, 180f, 0f);
                    arrowsScript[i].canMove = false;
                }
                break;

            case shootTypes.SIDES_TO_MIDDLE:
                for (int i = 0; i < arrows.Length; ++i)
                {
                    arrows[i].gameObject.SetActive(false);
                    if (i % 2 == 0)
                    {
                        arrows[i].position = new Vector3(maxRight - i / 2 * distanceBetweenArrows, prepareArrowYPos, 0f);
                    }
                    else
                    {
                        arrows[i].position = new Vector3(maxLeft + (i / 2 + 1) * distanceBetweenArrows, prepareArrowYPos, 0f);
                    }
                    arrows[i].localEulerAngles = new Vector3(0f, 180f, 0f);
                    arrowsScript[i].canMove = false;
                }
                break;

            case shootTypes.ULTRA_DIFFICULT_ULTRA:
                for (int i = 0; i < arrows.Length; ++i)
                {
                    arrows[i].gameObject.SetActive(false);
                    if (i < (arrows.Length / 2))
                    {
                        arrows[i].position = new Vector3(maxLeft + i * distanceBetweenArrows, prepareArrowYPos, 0f);
                    }
                    else
                    {
                        arrows[i].position = new Vector3(maxRight - (i - arrows.Length/ 2 + 0.5f) * distanceBetweenArrows, prepareArrowYPos, 0f);
                    }
                    arrows[i].localEulerAngles = new Vector3(0f, 180f, 0f);
                    arrowsScript[i].canMove = false;

                }
                break;


            case shootTypes.THREE_NEAR_GLITCH:
                shootDependingObjective = true;
                numberOfArrows = 3;
                break;

            case shootTypes.FIVE_NEAR_GLITCH:
                shootDependingObjective = true;
                numberOfArrows = 5;
                break;

            case shootTypes.SEVEN_NEAR_GLITCH:
                shootDependingObjective = true;
                numberOfArrows = 7;
                break;

            case shootTypes.NINE_NEAR_GLITCH:
                shootDependingObjective = true;
                numberOfArrows = 9;
                break;

            case shootTypes.ELEVEN_NEAR_GLITCH:
                shootDependingObjective = true;
                numberOfArrows = 11;
                break;
        }

        if (shootDependingObjective)
        {
            for (int i = 0; i < numberOfArrows; ++i)
            {
                arrows[i].gameObject.SetActive(false);
                if (i % 2 == 0)
                {
                    arrows[i].position = new Vector3(objectiveTransform.position.x - (i / 2) * distanceBetweenArrows, prepareArrowYPos, 0f);
                }
                else
                {
                    arrows[i].position = new Vector3(objectiveTransform.position.x + (i / 2 + 1) * distanceBetweenArrows, prepareArrowYPos, 0f);
                }
                arrows[i].rotation = new Quaternion(0f, 0f, 0f, 0f);
                arrowsScript[i].canMove = false;

            }
            for (int i = numberOfArrows; i < arrows.Length; ++i)
            {
                arrows[i].gameObject.SetActive(false);
                arrowsScript[i].canMove = false;
            }

        }

    }

    public void ShootUpArrow()
    {
        SoundManager.instance.PlaySingle(bowSound);
        upArrow.gameObject.SetActive(true);
        upArrow.transform.position = new Vector3(transform.position.x + 0.18f, transform.position.y + 3.2f, transform.position.z - 0.07f);
        upArrow.ShootArrow();
        Invoke("ShootArrows", 0.5f);
    }

    public void ShootArrows()
    {
        upArrow.gameObject.SetActive(false);
        upArrow.canMove = false;
        bossState = bossArcherIA.POSTSHOOT;
        timeSinceStateChanged = 0.0f;
        animator.speed = 1f;
        int random;
        if (lives == 3)
        {
            random = Random.Range(1, 5);
            switch (random)
            {
                case 1:
                    PrepareArrows(shootTypes.THREE_NEAR_GLITCH);
                    break;
                case 2:
                    PrepareArrows(shootTypes.FIVE_NEAR_GLITCH);
                    break;
                case 3:
                    PrepareArrows(shootTypes.SEVEN_NEAR_GLITCH);
                    break;
                case 4:
                    PrepareArrows(shootTypes.NINE_NEAR_GLITCH);
                    break;
            }
        }
        else if (lives == 2)
        {
            random = Random.Range(1, 4);
            switch (random)
            {
                case 1:
                    PrepareArrows(shootTypes.LEFT_TO_RIGHT);
                    break;
                case 2:
                    PrepareArrows(shootTypes.RIGHT_TO_LEFT);
                    break;
                case 3:
                    PrepareArrows(shootTypes.ELEVEN_NEAR_GLITCH);
                    break;
            }
        }
        else
        {
            random = Random.Range(1, 5);
            switch (random)
            {
                case 1:
                    PrepareArrows(shootTypes.LEFT_TO_RIGHT);
                    break;
                case 2:
                    PrepareArrows(shootTypes.RIGHT_TO_LEFT);
                    break;
                case 3:
                    PrepareArrows(shootTypes.SIDES_TO_MIDDLE);
                    break;
                case 4:
                    PrepareArrows(shootTypes.ULTRA_DIFFICULT_ULTRA);
                    break;
            }
        }
		shootInThisPlatform = true;
        StartCoroutine("CO_ShootArrows");
    }

    IEnumerator CO_ShootArrows()
    {
        float timeShooting = -Time.deltaTime;
        bool oneByOne = false;
        int i;
        int numberOfArrows = 0;
        switch (lastShootType)
        {
            case shootTypes.LEFT_TO_RIGHT:
            case shootTypes.RIGHT_TO_LEFT:
            case shootTypes.SIDES_TO_MIDDLE:
                oneByOne = true;
                numberOfArrows = arrows.Length / 2;
                break;

            case shootTypes.ULTRA_DIFFICULT_ULTRA:
                oneByOne = true;
                numberOfArrows = arrows.Length;
                break;

            case shootTypes.THREE_NEAR_GLITCH:
                numberOfArrows = 3;
                break;

            case shootTypes.FIVE_NEAR_GLITCH:
                numberOfArrows = 5;
                break;

            case shootTypes.SEVEN_NEAR_GLITCH:
                numberOfArrows = 7;
                break;

            case shootTypes.NINE_NEAR_GLITCH:
                numberOfArrows = 9;
                break;

            case shootTypes.ELEVEN_NEAR_GLITCH:
                numberOfArrows = 11;
                break;
        }
        i = 0;
        if (oneByOne)
        {
            while (i < numberOfArrows)
            {
                timeShooting += Time.deltaTime;
                if (timeShooting >= (i * timeToShootArrows / arrows.Length))
                {
                    for (int j = i; j < i + 5; ++j)
                    {
                        arrowsScript[j].ShootArrow();
                        arrows[j].gameObject.SetActive(true);
                    }
                    i += 5;
                }
                yield return null;
            }
        }
        else
        {
            while (i < numberOfArrows)
            {
                timeShooting += Time.deltaTime;
                if (timeShooting >= (i * timeToShootArrows / (arrows.Length * 2)))
                {
                    if (numberOfArrows % 2 != 0 && i == 0)
                    {
                        arrows[i].gameObject.SetActive(true);
                        arrowsScript[i].ShootArrow();
                    }
                    else
                    {
                        arrows[i].gameObject.SetActive(true);
                        arrowsScript[i].ShootArrow();
                        arrows[i + 1].gameObject.SetActive(true);
                        arrowsScript[i + 1].ShootArrow();
                        ++i;
                    }
                    ++i;
                }
                yield return null;
            }
        }
    }

    #endregion
}