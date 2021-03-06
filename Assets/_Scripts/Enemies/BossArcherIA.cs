using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using InControl;

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
        ELEVEN_NEAR_GLITCH,
        NEW_SHOOT_TYPE_BIG,
        NEW_SHOOT_TYPE_NORMAL,
        NEW_SHOOT_TYPE_SMALL
    }

    public BossStageCamera camera;
    public CameraShake shake;
    public AudioClip hit;
    public AudioClip scream;
    public DebrisManagerGlitch glitchDebris;        // Debris in "z" of Glitch
    public float horizontalVelocity = 10.0f;
    public bool start = false;
    public float timeInPreShoot = 2.0f;
    public float timeInPostShoot = 2.0f;
    public bool holesActives = false;
    private float timeSinceStateChanged;
    [SerializeField]
    private bool movingRight = true;
    public GameObject glitchCollider;
    public GameObject glitchDialogue;

    [SerializeField]
    private bossArcherIA bossState;
    [SerializeField]
    private bossArcherPos bossPos;
    private Rigidbody rigidbody;
    private BoxCollider boxCollider;

    public Transform arrowPool;
    private Transform[] arrows;
    private BossArrowScript[] arrowsScript;

    private float maxLeft = -26f;
    private float maxRight = 26f;
    public float prepareArrowYPos = 12f;

    public float timeToShootArrows = 1f;

    private shootTypes lastShootType;

    public Transform objectiveTransform;

    private Animator animator;

    private int layerMask = (~((1 << 13) | (1 << 2) | (1 << 11) | (1 << 8))) | (1 << 9) | (1 << 0);

    private bool fallingJump = false;
    private bool firstStopPoint = true;

    public int lives = 5;

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
    [SerializeField]
    private bool fallingDead = false;

    public World world;

    private bool slowFPSactivated = false;
    public SlowFPS slowFPS;

    private float timeWhenLastHitted;

    public BossArrowScript upArrow;

    private bool shootInThisPlatform = false;

    public AudioClip bowSound;
    public AudioClip jumpSound;

    private bool canShoot = true;
    public Player playerScript;

    public Transform HUDLives;
    private Image heart1;
    private Image heart2;
    private Image heart3;
    private Image heart4;
    private Image heart5;

    public Sprite heartFull;
    public Sprite heartEmpty;

    public EndBossDialoguePoint endBoss;

    public delegate void BossDeadDelegate();
    public event BossDeadDelegate BossDeadEvent;

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

        playerScript.PlayerDeadEvent += GlitchDead;
        playerScript.PlayerReviveEvent += GlitchRevives;

        upArrow.gameObject.SetActive(false);

        heart1 = HUDLives.FindChild("Heart1").GetComponent<Image>();
        heart2 = HUDLives.FindChild("Heart2").GetComponent<Image>();
        heart3 = HUDLives.FindChild("Heart3").GetComponent<Image>();
        heart4 = HUDLives.FindChild("Heart4").GetComponent<Image>();
        heart5 = HUDLives.FindChild("Heart5").GetComponent<Image>();

        //transform.FindChild("GlitchCollider").GetComponent<GlitchArcher>().BossGlitchedEvent += GlitchArcher;

        endBoss.BossGlitchEvent += GlitchArcher;
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
                    if (canShoot)
                    {
                        timeSinceStateChanged += world.lag;
                        if (timeSinceStateChanged >= timeInPreShoot)
                        {
                            bossState = bossArcherIA.SHOOTING;
                            animator.speed = currentSpecialSpeed;
                            animator.SetTrigger("Attack");
                        }
                    }
                    else
                    {
                        timeSinceStateChanged = 0.0f;
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
            int random;
            random = Random.Range(1, 3);
            if (bossPos == bossArcherPos.MAXRIGHT)
            {
                if (random == 1)
                {
                    bossPos = bossArcherPos.MEDIUMRIGHT;
                    currentEndJumpPoint = EndJumpPoint[4].position + new Vector3(-1f, -1f, 0f);
                    currentMiddleJumpPoint = MiddleJumpPoint[4].position;
                }
                else
                {
                    bossPos = bossArcherPos.MEDIUMLEFT;
                    currentEndJumpPoint = EndJumpPoint[2].position + new Vector3(-1f, -1f, 0f);
                    currentMiddleJumpPoint = MiddleJumpPoint[3].position;
                }
            }
            else if (bossPos == bossArcherPos.MAXLEFT)
            {
                if (random == 1)
                {
                    bossPos = bossArcherPos.MEDIUMLEFT;
                    currentEndJumpPoint = EndJumpPoint[1].position + new Vector3(+1f, -1f, 0f);
                    currentMiddleJumpPoint = MiddleJumpPoint[0].position;
                }
                else
                {
                    bossPos = bossArcherPos.MEDIUMRIGHT;
                    currentEndJumpPoint = EndJumpPoint[3].position + new Vector3(1f, -1f, 0f);
                    currentMiddleJumpPoint = MiddleJumpPoint[1].position;
                }
            }
            else if (bossPos == bossArcherPos.MEDIUMLEFT && movingRight)
            {
                if (random == 1)
                {
                    bossPos = bossArcherPos.MEDIUMRIGHT;
                    currentEndJumpPoint = EndJumpPoint[3].position + new Vector3(1f, -1f, 0f);
                    currentMiddleJumpPoint = MiddleJumpPoint[2].position;
                }
                else
                {
                    bossPos = bossArcherPos.MAXRIGHT;
                    currentEndJumpPoint = EndJumpPoint[5].position + new Vector3(1f, -1f, 0f);
                    currentMiddleJumpPoint = MiddleJumpPoint[3].position;
                }
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
                currentMiddleJumpPoint = MiddleJumpPoint[4].position;
            }
            else if (bossPos == bossArcherPos.MEDIUMRIGHT && !movingRight)
            {
                if (random == 1)
                {
                    bossPos = bossArcherPos.MEDIUMLEFT;
                    currentEndJumpPoint = EndJumpPoint[2].position + new Vector3(-1f, -1f, 0f);
                    currentMiddleJumpPoint = MiddleJumpPoint[2].position;
                }
                else
                {
                    bossPos = bossArcherPos.MAXLEFT;
                    currentEndJumpPoint = EndJumpPoint[0].position + new Vector3(-1f, -1f, 0f);
                    currentMiddleJumpPoint = MiddleJumpPoint[1].position;
                }
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
        else if (coll.collider.CompareTag("BossHit") && Time.time - timeWhenLastHitted >= 2f && bossState != bossArcherIA.DEAD)
        {
            SoundManager.instance.PlaySingle(hit);
            timeWhenLastHitted = Time.time;
            transform.localEulerAngles = new Vector3(0f, 180f, 0f);
            animator.speed = 1f;
            --lives;
            if (lives == 0)
            {
                heart1.sprite = heartEmpty;
                SoundManager.instance.PlaySingle(scream);
                animator.SetTrigger("LastHitted");
                camera.ZoomOut();
                glitchDebris.ArcherDead();
                bossState = bossArcherIA.DEAD;
                glitchDialogue.SetActive(true);
                glitchCollider.SetActive(true);
                shake.enabled = false;
                if (BossDeadEvent != null)
                    BossDeadEvent();
            }
            else if (lives == 1)
            {
                heart2.sprite = heartEmpty;
                holesActives = true;
                camera.ZoomArcherIn();
                timeInPreShoot = 0f;
                timeInPostShoot = 0f;
                animator.SetTrigger("Hitted");
                bossState = bossArcherIA.HITTED;
            }
            else if (lives == 2)
            {
                heart3.sprite = heartEmpty;
                holesActives = true;
                camera.ZoomArcherIn();
                animator.SetTrigger("Hitted");
                bossState = bossArcherIA.HITTED;
            }
            else if (lives == 3)
            {
                shake.enabled = true;
                heart4.sprite = heartEmpty;
                holesActives = true;
                camera.ZoomArcherIn();
                timeInPreShoot = 1f;
                timeInPostShoot = 1f;
                glitchDebris.Fall();
                animator.SetTrigger("Hitted");
                bossState = bossArcherIA.HITTED;
            }
            else if (lives == 4)
            {
                heart5.sprite = heartEmpty;
                holesActives = true;
                camera.ZoomArcherIn();
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

        camera.ZoomArcherOut();
        holesActives = false;
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
        else if (movingRight)
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

        if (transform.position.x < -24f+9.21f)
            endXPosWhenDead = -32f + 9.21f;
        else if(transform.position.x < -7.5f + 9.21f)
            endXPosWhenDead = -17f + 9.21f;
        else if (transform.position.x < 8.5f + 9.21f)
            endXPosWhenDead = -8f + 9.21f;
        else
            endXPosWhenDead = 13f + 9.21f;

        startXPosWhenDead = transform.position.x;
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

    public void GlitchDead()
    {
        canShoot = false;
    }

    public void GlitchRevives()
    {
        canShoot = true;
    }

    public void GlitchArcher()
    {
        animator.SetTrigger("LastGlitched");
    }

    #endregion

    #region Shoot

    private void PrepareArrows(shootTypes shootType)
    {
        lastShootType = shootType;
        float distance = maxRight - maxLeft;
        float distanceBetweenArrows = distance / 30;
        bool shootDependingObjective = false;
        int numberOfArrows = 0;
        float offset;
        int j;
        int random;
        switch (shootType)
        {
            case shootTypes.NEW_SHOOT_TYPE_SMALL:
                random = Random.Range(-2, 3);
                for (int i = 0; i < 22; ++i)
                {
                    if (i < 5)
                    {
                        offset = 0f;
                        j = i;
                    }
                    else if (i < 9)
                    {
                        offset = 7f * distanceBetweenArrows;
                        j = i - 5;
                    }
                    else if (i < 13)
                    {
                        offset = 13 * distanceBetweenArrows;
                        j = i - 9;
                    }
                    else if (i < 17)
                    {
                        offset = 19f * distanceBetweenArrows;
                        j = i - 13;
                    }
                    else
                    {
                        offset = 25f * distanceBetweenArrows;
                        j = i - 17;
                    }

                    arrows[i].gameObject.SetActive(false);
                    arrows[i].position = new Vector3(maxLeft + random * distanceBetweenArrows + offset + j * distanceBetweenArrows, prepareArrowYPos, 0f);
                    arrows[i].localEulerAngles = new Vector3(0f, 180f, 0f);
                    arrowsScript[i].canMove = false;
                    arrows[i + 22].gameObject.SetActive(false);
                    arrows[i + 22].position = new Vector3(maxLeft + random * distanceBetweenArrows + offset + j * distanceBetweenArrows + distanceBetweenArrows / 2.0f, prepareArrowYPos, 0f);
                    arrows[i + 22].localEulerAngles = new Vector3(0f, 180f, 0f);
                    arrowsScript[i].canMove = false;
                    arrows[i + 44].gameObject.SetActive(false);
                    arrows[i + 44].position = new Vector3(maxLeft + random * distanceBetweenArrows + offset + j * distanceBetweenArrows + distanceBetweenArrows / 4.0f, prepareArrowYPos, 0f);
                    arrows[i + 44].localEulerAngles = new Vector3(0f, 180f, 0f);
                    arrowsScript[i].canMove = false;
                }
                break;

            case shootTypes.NEW_SHOOT_TYPE_NORMAL:
                random = Random.Range(-2, 3);
                for (int i = 0; i < 18; ++i)
                {
                    if (i < 3)
                    {
                        offset = 0f;
                        j = i;
                    }
                    else if (i < 7)
                    {
                        offset = 6f * distanceBetweenArrows;
                        j = i - 3;
                    }
                    else if (i < 11)
                    {
                        offset = 13 * distanceBetweenArrows;
                        j = i - 7;
                    }
                    else if (i < 15)
                    {
                        offset = 20f * distanceBetweenArrows;
                        j = i - 11;
                    }
                    else
                    {
                        offset = 27f * distanceBetweenArrows;
                        j = i - 15;
                    }

                    arrows[i].gameObject.SetActive(false);
                    arrows[i].position = new Vector3(maxLeft + random * distanceBetweenArrows + offset + j * distanceBetweenArrows, prepareArrowYPos, 0f);
                    arrows[i].localEulerAngles = new Vector3(0f, 180f, 0f);
                    arrowsScript[i].canMove = false;
                    arrows[i + 18].gameObject.SetActive(false);
                    arrows[i + 18].position = new Vector3(maxLeft + random * distanceBetweenArrows + offset + j * distanceBetweenArrows + distanceBetweenArrows / 2.0f, prepareArrowYPos, 0f);
                    arrows[i + 18].localEulerAngles = new Vector3(0f, 180f, 0f);
                    arrowsScript[i].canMove = false;
                    arrows[i + 36].gameObject.SetActive(false);
                    arrows[i + 36].position = new Vector3(maxLeft + random * distanceBetweenArrows + offset + j * distanceBetweenArrows + distanceBetweenArrows / 4.0f, prepareArrowYPos, 0f);
                    arrows[i + 36].localEulerAngles = new Vector3(0f, 180f, 0f);
                    arrowsScript[i].canMove = false;
                }
                break;

            case shootTypes.NEW_SHOOT_TYPE_BIG:
                random = Random.Range(-2, 3);
                for (int i = 0; i < 14; ++i)
                {
                    if (i < 2)
                    {
                        offset = 0f;
                        j = i;
                    }
                    else if (i < 5)
                    {
                        offset = 6f * distanceBetweenArrows;
                        j = i - 2;
                    }
                    else if (i < 8)
                    {
                        offset = 13 * distanceBetweenArrows;
                        j = i - 5;
                    }
                    else if (i < 11)
                    {
                        offset = 20f * distanceBetweenArrows;
                        j = i - 8;
                    }
                    else
                    {
                        offset = 27f * distanceBetweenArrows;
                        j = i - 11;
                    }

                    arrows[i].gameObject.SetActive(false);
                    arrows[i].position = new Vector3(maxLeft + random * distanceBetweenArrows + offset + j * distanceBetweenArrows, prepareArrowYPos, 0f);
                    arrows[i].localEulerAngles = new Vector3(0f, 180f, 0f);
                    arrowsScript[i].canMove = false;
                    arrows[i + 14].gameObject.SetActive(false);
                    arrows[i + 14].position = new Vector3(maxLeft + random * distanceBetweenArrows + offset + j * distanceBetweenArrows + distanceBetweenArrows / 2.0f, prepareArrowYPos, 0f);
                    arrows[i + 14].localEulerAngles = new Vector3(0f, 180f, 0f);
                    arrowsScript[i].canMove = false;
                    arrows[i + 28].gameObject.SetActive(false);
                    arrows[i + 28].position = new Vector3(maxLeft + random * distanceBetweenArrows + offset + j * distanceBetweenArrows + distanceBetweenArrows / 4.0f, prepareArrowYPos, 0f);
                    arrows[i + 28].localEulerAngles = new Vector3(0f, 180f, 0f);
                    arrowsScript[i].canMove = false;
                }
                break;

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
                        arrows[i].position = new Vector3(maxRight - (i - arrows.Length / 2 + 0.5f) * distanceBetweenArrows, prepareArrowYPos, 0f);
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
                    arrows[i + numberOfArrows].position = new Vector3(objectiveTransform.position.x - (i / 2) * distanceBetweenArrows + distanceBetweenArrows / 2f, prepareArrowYPos + 1f, 0f);
                    arrows[i + numberOfArrows * 2].position = new Vector3(objectiveTransform.position.x - (i / 2) * distanceBetweenArrows, prepareArrowYPos + 2f, 0f);
                }
                else
                {
                    arrows[i].position = new Vector3(objectiveTransform.position.x + (i / 2 + 1) * distanceBetweenArrows, prepareArrowYPos, 0f);
                    arrows[i + numberOfArrows].position = new Vector3(objectiveTransform.position.x + (i / 2 + 1) * distanceBetweenArrows + distanceBetweenArrows / 2f, prepareArrowYPos + 1f, 0f);
                    arrows[i + numberOfArrows * 2].position = new Vector3(objectiveTransform.position.x + (i / 2 + 1) * distanceBetweenArrows, prepareArrowYPos + 2f, 0f);
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
        if(bossState != bossArcherIA.DEAD)
            bossState = bossArcherIA.POSTSHOOT;
        timeSinceStateChanged = 0.0f;
        animator.speed = 1f;
        int random;
        if (lives == 5)
        {
            PrepareArrows(shootTypes.FIVE_NEAR_GLITCH);
        }
        else if (lives == 4)
        {
            random = Random.Range(1, 3);
            switch (random)
            {
                case 1:
                    PrepareArrows(shootTypes.NINE_NEAR_GLITCH);
                    break;
                case 2:
                    PrepareArrows(shootTypes.NEW_SHOOT_TYPE_BIG);
                    break;
            }
        }
        else if (lives == 3)
        {
            random = Random.Range(1, 5);
            switch (random)
            {
                case 1:
                    PrepareArrows(shootTypes.ELEVEN_NEAR_GLITCH);
                    break;
                case 2:
                    PrepareArrows(shootTypes.NEW_SHOOT_TYPE_NORMAL);
                    break;
                case 3:
                    PrepareArrows(shootTypes.LEFT_TO_RIGHT);
                    break;
                case 4:
                    PrepareArrows(shootTypes.RIGHT_TO_LEFT);
                    break;
            }
        }
        else if (lives == 2)
        {
            random = Random.Range(1, 5);
            switch (random)
            {
                case 1:
                    PrepareArrows(shootTypes.NEW_SHOOT_TYPE_SMALL);
                    break;
                case 2:
                    PrepareArrows(shootTypes.LEFT_TO_RIGHT);
                    break;
                case 3:
                    PrepareArrows(shootTypes.RIGHT_TO_LEFT);
                    break;
                case 4:
                    PrepareArrows(shootTypes.ULTRA_DIFFICULT_ULTRA);
                    break;
            }
        }
        else if (lives <= 1)
        {
            random = Random.Range(1, 6);
            switch (random)
            {
                case 1:
                    PrepareArrows(shootTypes.NEW_SHOOT_TYPE_SMALL);
                    break;
                case 2:
                    PrepareArrows(shootTypes.LEFT_TO_RIGHT);
                    break;
                case 3:
                    PrepareArrows(shootTypes.RIGHT_TO_LEFT);
                    break;
                case 4:
                    PrepareArrows(shootTypes.ELEVEN_NEAR_GLITCH);
                    break;
                case 5:
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
        bool newShoot = false;
        int i;
        int numberOfArrows = 0;
        int arrowsAtTheSameTime = 5;
        switch (lastShootType)
        {
            case shootTypes.LEFT_TO_RIGHT:
            case shootTypes.RIGHT_TO_LEFT:
            case shootTypes.SIDES_TO_MIDDLE:
                newShoot = true;
                numberOfArrows = 30;
                arrowsAtTheSameTime = 5;
                break;

            case shootTypes.NEW_SHOOT_TYPE_BIG:
                newShoot = true;
                numberOfArrows = 42;
                arrowsAtTheSameTime = 14;
                break;

            case shootTypes.NEW_SHOOT_TYPE_NORMAL:
                newShoot = true;
                numberOfArrows = 54;
                arrowsAtTheSameTime = 18;
                break;

            case shootTypes.NEW_SHOOT_TYPE_SMALL:
                newShoot = true;
                numberOfArrows = 66;
                arrowsAtTheSameTime = 22;
                break;

            case shootTypes.ULTRA_DIFFICULT_ULTRA:
                newShoot = true;
                numberOfArrows = 60;
                arrowsAtTheSameTime = 5;
                break;

            case shootTypes.THREE_NEAR_GLITCH:
                numberOfArrows = 3 * 3;
                break;

            case shootTypes.FIVE_NEAR_GLITCH:
                numberOfArrows = 5 * 3;
                break;

            case shootTypes.SEVEN_NEAR_GLITCH:
                numberOfArrows = 7 * 3;
                break;

            case shootTypes.NINE_NEAR_GLITCH:
                numberOfArrows = 9 * 3;
                break;

            case shootTypes.ELEVEN_NEAR_GLITCH:
                numberOfArrows = 11 * 3;
                break;
        }
        i = 0;
        if (newShoot)
        {
            while (i < numberOfArrows)
            {
                timeShooting += Time.deltaTime;
                if (timeShooting >= (i * timeToShootArrows / arrows.Length))
                {
                    for (int j = i; j < i + arrowsAtTheSameTime; ++j)
                    {
                        arrowsScript[j].ShootArrow();
                        arrows[j].gameObject.SetActive(true);
                    }
                    i += arrowsAtTheSameTime;
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
                    if (numberOfArrows % 2 != 0 && (i % (numberOfArrows / 3.0f) == 0))
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