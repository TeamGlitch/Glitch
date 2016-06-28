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

    public float horizontalVelocity = 10.0f;
    public Door door;
    public float timeInPreShoot = 2.0f;
    public float timeInPostShoot = 2.0f;
    private float _timeSinceStateChanged;
    private bool _movingRight = true;

    [SerializeField]
    private bossArcherIA _bossState;
    private bossArcherPos _bossPos;
    private Rigidbody _rigidbody;
    private BoxCollider _boxCollider;

    public Transform arrowPool;
    private Transform[] _arrows;
    private BossArrowScript[] _arrowsScript;

    private float _maxLeft = -36f;
    private float _maxRight = 36f;
    public float prepareArrowYPos = 12f;

    public float timeToShootArrows = 3f;

    private shootTypes _lastShootType;

    public Transform objectiveTransform;

    private Animator _animator;

    private int _layerMask = (~((1 << 13) | (1 << 2) | (1 << 11) | (1 << 8))) | (1 << 9) | (1 << 0);

    private bool _fallingJump = false;
    private bool _firstStopPoint = true;

    private int lives = 3;

    private float _currentSpecialSpeed = 1f;

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
    private bool _fallingDead = false;

    public World world;

    private bool slowFPSactivated = false;
    public SlowFPS slowFPS;

    private float timeWhenLastHitted;

    public BossArrowScript upArrow;

	private bool shootInThisPlatform = false;

    #endregion

    #region Init & Update

    // Use this for initialization
    void Start()
    {

        _rigidbody = transform.GetComponent<Rigidbody>();
        _boxCollider = transform.GetComponent<BoxCollider>();
        _timeSinceStateChanged = 0.0f;
        _bossPos = bossArcherPos.MEDIUMRIGHT;
        _bossState = bossArcherIA.IDLE;
        _timeSinceStateChanged = 0.0f;
        timeJumping = 0.0f;

        _arrows = new Transform[arrowPool.childCount];
        for (int i = 0; i < arrowPool.childCount; ++i)
        {
            _arrows[i] = arrowPool.GetChild(i);
        }
        _arrowsScript = new BossArrowScript[arrowPool.childCount];
        for (int i = 0; i < _arrows.Length; ++i)
        {
            _arrowsScript[i] = _arrows[i].GetComponent<BossArrowScript>();
        }
        _animator = transform.GetComponent<Animator>();
        _firstStopPoint = true;
        slowFPS.SlowFPSChangedStatusEvent += SlowFPSStateChanged;
        timeWhenLastHitted = Time.time;

        upArrow.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (world.doUpdate)
        {
            if (!_fallingDead)
                _rigidbody.velocity = Vector3.zero;
            switch (_bossState)
            {
                case bossArcherIA.MOVING:
                    transform.Translate(Vector3.forward * world.lag * horizontalVelocity);
                    break;

                case bossArcherIA.FALLING_JUMP:
                    if (_fallingJump)
                        _bossState = bossArcherIA.MOVING;
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
                        _bossState = bossArcherIA.FALLING_JUMP;
                    }
                    break;

                case bossArcherIA.PRESHOOT:
                    _timeSinceStateChanged += world.lag;
                    if (_timeSinceStateChanged >= timeInPreShoot)
                    {
                        _bossState = bossArcherIA.SHOOTING;
                        _animator.speed = _currentSpecialSpeed;
                        _animator.SetTrigger("Attack");
                    }
                    break;

                case bossArcherIA.SHOOTING:
                    break;

                case bossArcherIA.POSTSHOOT:
                    _timeSinceStateChanged += world.lag;
                    if (_timeSinceStateChanged >= timeInPostShoot)
                    {
                        int random;
                        switch (_bossPos)
                        {
                            case bossArcherPos.MAXLEFT:
                                _movingRight = true;
                                _bossPos = bossArcherPos.MEDIUMLEFT;
                                break;

                            case bossArcherPos.MEDIUMLEFT:
                                random = Random.Range(1, 3);
                                if (random == 1)
                                {
                                    _movingRight = true;
                                    _bossPos = bossArcherPos.MEDIUMRIGHT;
                                }
                                else
                                {
                                    _movingRight = false;
                                    _bossPos = bossArcherPos.MAXLEFT;
                                }
                                break;
                            case bossArcherPos.MEDIUMRIGHT:
                                random = Random.Range(1, 3);
                                if (random == 1)
                                {
                                    _movingRight = true;
                                    _bossPos = bossArcherPos.MAXRIGHT;
                                }
                                else
                                {
                                    _movingRight = false;
                                    _bossPos = bossArcherPos.MEDIUMLEFT;
                                }
                                break;
                            case bossArcherPos.MAXRIGHT:
                                _movingRight = false;
                                _bossPos = bossArcherPos.MEDIUMRIGHT;
                                break;
                        }
                        if (_movingRight)
                        {
                            _bossState = bossArcherIA.TURNING_LEFT_TO_RUN;
                            _animator.SetBool("Run", true);
                            _animator.speed = _currentSpecialSpeed;
                            _animator.SetTrigger("TurnLeft");
                        }
                        else
                        {
                            _bossState = bossArcherIA.TURNING_RIGHT_TO_RUN;
                            _animator.SetBool("Run", true);
                            _animator.speed = _currentSpecialSpeed;
                            _animator.SetTrigger("TurnRight");
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
                    if (Input.GetKeyDown(KeyCode.P))
                        _bossState = bossArcherIA.PRESHOOT;
                        break;

                case bossArcherIA.DEAD:
                    if (_fallingDead)
                    {
                        timeFalling += world.lag;
                        float percentage = timeFalling / timeToMoveZWhileFall;
                        if (percentage >= 1.0f)
                            percentage = 1.0f;
                        float xPos = Mathf.Lerp(startXPosWhenDead, endXPosWhenDead, percentage);
                        float zPos = Mathf.Lerp(startZPosWhenDead, endZPosWhenDead, percentage);
                        transform.position = new Vector3(xPos, transform.position.y, zPos);
                        door.OpenDoor();
                    }
                    break;

            }
        }
    }

    #endregion

    #region Colliders

    public void OnTriggerEnter(Collider coll)
    {
        if (coll.transform.name == "JumpPoint" && _bossState == bossArcherIA.MOVING && _bossState != bossArcherIA.DEAD)
        {
            currentStartJumpPoint = transform.position;
            timeJumping = 0.0f;
            if (_bossPos == bossArcherPos.MAXRIGHT)
            {
                currentEndJumpPoint = EndJumpPoint[5].position + new Vector3(1f, -1f, 0f);
                currentMiddleJumpPoint = MiddleJumpPoint[2].position;
            }
            else if (_bossPos == bossArcherPos.MAXLEFT)
            {
                currentEndJumpPoint = EndJumpPoint[0].position + new Vector3(-1f, -1f, 0f);
                currentMiddleJumpPoint = MiddleJumpPoint[0].position;
            }
            else if (_bossPos == bossArcherPos.MEDIUMLEFT && _movingRight)
            {
                currentEndJumpPoint = EndJumpPoint[1].position + new Vector3(1f, -1f, 0f);
                currentMiddleJumpPoint = MiddleJumpPoint[0].position;
            }
            else if (_bossPos == bossArcherPos.MEDIUMLEFT && !_movingRight)
            {
                currentEndJumpPoint = EndJumpPoint[2].position + new Vector3(-1f, -1f, 0f);
                currentMiddleJumpPoint = MiddleJumpPoint[1].position;
            }
            else if (_bossPos == bossArcherPos.MEDIUMRIGHT && _movingRight)
            {
                currentEndJumpPoint = EndJumpPoint[3].position + new Vector3(1f, -1f, 0f);
                currentMiddleJumpPoint = MiddleJumpPoint[1].position;
            }
            else if (_bossPos == bossArcherPos.MEDIUMRIGHT && !_movingRight)
            {
                currentEndJumpPoint = EndJumpPoint[4].position + new Vector3(-1f, -1f, 0f);
                currentMiddleJumpPoint = MiddleJumpPoint[2].position;
            }
            else
            {
                Debug.Log("HEHE; ERROR");
            }
            _bossState = bossArcherIA.JUMPING;
            _fallingJump = false;
            _animator.SetTrigger("Jump");
        }
        else if (coll.transform.name == "StopPoint" && _firstStopPoint && _bossState != bossArcherIA.DEAD && !shootInThisPlatform)
        {
            _firstStopPoint = false;
        }
        else if (coll.transform.name == "StopPoint" && !_firstStopPoint && _bossState != bossArcherIA.DEAD && !shootInThisPlatform)
        {
            if (_bossPos == bossArcherPos.MEDIUMLEFT || _bossPos == bossArcherPos.MEDIUMRIGHT)
                transform.position = new Vector3(transform.position.x, transform.position.y, 17.5f - 5.34f);
            if (_movingRight)
            {
                _bossState = bossArcherIA.TURNING_RIGHT_TO_STOP;
                _animator.speed = _currentSpecialSpeed;
                _animator.SetTrigger("TurnRight");
            }
            else
            {
                _bossState = bossArcherIA.TURNING_LEFT_TO_STOP;
                _animator.speed = _currentSpecialSpeed;
                _animator.SetTrigger("TurnLeft");
            }
            _animator.SetBool("Run", false);
        }
    }

    public void OnCollisionEnter(Collision coll)
    {
        if (_bossState == bossArcherIA.DEAD && !coll.collider.CompareTag("BossHit") && Time.time - timeWhenLastHitted >= 1f)
        {
            _animator.speed = 1f;
            _animator.SetTrigger("GroundHitted");
        }
        else if (coll.collider.CompareTag("BossHit") && Time.time - timeWhenLastHitted >= 1f && _bossState != bossArcherIA.DEAD)
        {
            timeWhenLastHitted = Time.time;
            transform.localEulerAngles = new Vector3(0f, 180f, 0f);
            _animator.speed = 1f;
            --lives;
            if (lives == 0)
            {
                _animator.SetTrigger("LastHitted");
                _bossState = bossArcherIA.DEAD;
            }
            else if (lives == 1)
            {
                timeToShootArrows = 1;
                timeInPreShoot = 0f;
                timeInPostShoot = 0f;
                _animator.SetTrigger("Hitted");
                _bossState = bossArcherIA.HITTED;
            }
            else if (lives == 2)
            {
                timeToShootArrows = 2;
                timeInPreShoot = 1f;
                timeInPostShoot = 1f;
                _animator.SetTrigger("Hitted");
                _bossState = bossArcherIA.HITTED;
            }
        }
    }

    public void InsultingAnimationEnded()
    {
        if (lives == 2)
            _currentSpecialSpeed = 2f;
        else if (lives == 1)
            _currentSpecialSpeed = 3f;

		if(!shootInThisPlatform)
	        _bossState = bossArcherIA.PRESHOOT;
		else
			_bossState = bossArcherIA.MOVING;
        _timeSinceStateChanged = 0.0f;
    }

    #endregion

    #region Utils

    private bool IsGrounded()
    {
        bool result;
        result = Physics.Raycast(new Vector3(transform.position.x, transform.position.y + _boxCollider.center.y, transform.position.z), -Vector3.up, _boxCollider.size.y + 0.1f, 1, QueryTriggerInteraction.Ignore);
        return result;
    }

    public void EndTurning()
    {
        if (_bossState == bossArcherIA.TURNING_LEFT_TO_RUN)
        {
            transform.localEulerAngles = new Vector3(0f, 90f, 0f);
            _bossState = bossArcherIA.MOVING;
        }
        else if (_bossState == bossArcherIA.TURNING_RIGHT_TO_RUN)
        {
            transform.localEulerAngles = new Vector3(0f, 270f, 0f);
            _bossState = bossArcherIA.MOVING;
        }
        else if (_bossState == bossArcherIA.TURNING_LEFT_TO_STOP)
        {
            transform.localEulerAngles = new Vector3(0f, 180f, 0f);
            _bossState = bossArcherIA.PRESHOOT;
        }
        else if (_bossState == bossArcherIA.TURNING_RIGHT_TO_STOP)
        {
            transform.localEulerAngles = new Vector3(0f, 180f, 0f);
            _bossState = bossArcherIA.PRESHOOT;
        }
        _animator.speed = 1;
    }

    public void EndJump()
    {
        _animator.speed = _currentSpecialSpeed;
        _fallingJump = true;
    }

    public void StartFallingAnimationEnded()
    {
        Vector3 auxPos = transform.position + new Vector3(-2f, -2.2f, 0f);
        _rigidbody.useGravity = true;
        startXPosWhenDead = endXPosWhenDead = transform.position.x;
        if (transform.position.x >= 16f)
        {
            endXPosWhenDead = startXPosWhenDead + 2f;
        }
        startZPosWhenDead = transform.position.z;
        timeFalling = 0.0f;
        transform.position = auxPos;
        _fallingDead = true;
    }

    public void SlowFPSStateChanged()
    {
        if (slowFPSactivated)
        {
            slowFPSactivated = false;
            _animator.speed = 1f;
        }
        else
        {
            slowFPSactivated = true;
            _animator.speed = 0.5f;
        }
    }

    #endregion

    #region Shoot

    private void PrepareArrows(shootTypes shootType)
    {
        _lastShootType = shootType;
        float distance = _maxRight - _maxLeft;
        float distanceBetweenArrows = distance / (_arrows.Length / 2);
        bool shootDependingObjective = false;
        int numberOfArrows = 0;
        switch (shootType)
        {
            case shootTypes.LEFT_TO_RIGHT:
                for (int i = 0; i < _arrows.Length; ++i)
                {
                    _arrows[i].gameObject.SetActive(false);
                    _arrows[i].position = new Vector3(_maxLeft + i * distanceBetweenArrows, prepareArrowYPos, 0f);
                    _arrows[i].localEulerAngles = new Vector3(0f, 180f, 0f);
                    _arrowsScript[i].canMove = false;
                }
                break;

            case shootTypes.RIGHT_TO_LEFT:
                for (int i = 0; i < _arrows.Length; ++i)
                {
                    _arrows[i].gameObject.SetActive(false);
                    _arrows[i].position = new Vector3(_maxRight - i * distanceBetweenArrows, prepareArrowYPos, 0f);
                    _arrows[i].localEulerAngles = new Vector3(0f, 180f, 0f);
                    _arrowsScript[i].canMove = false;
                }
                break;

            case shootTypes.SIDES_TO_MIDDLE:
                for (int i = 0; i < _arrows.Length; ++i)
                {
                    _arrows[i].gameObject.SetActive(false);
                    if (i % 2 == 0)
                    {
                        _arrows[i].position = new Vector3(_maxRight - i / 2 * distanceBetweenArrows, prepareArrowYPos, 0f);
                    }
                    else
                    {
                        _arrows[i].position = new Vector3(_maxLeft + (i / 2 + 1) * distanceBetweenArrows, prepareArrowYPos, 0f);
                    }
                    _arrows[i].localEulerAngles = new Vector3(0f, 180f, 0f);
                    _arrowsScript[i].canMove = false;
                }
                break;

            case shootTypes.ULTRA_DIFFICULT_ULTRA:
                for (int i = 0; i < _arrows.Length; ++i)
                {
                    _arrows[i].gameObject.SetActive(false);
                    if (i < (_arrows.Length / 2))
                    {
                        _arrows[i].position = new Vector3(_maxLeft + i * distanceBetweenArrows, prepareArrowYPos, 0f);
                    }
                    else
                    {
                        _arrows[i].position = new Vector3(_maxRight - (i - _arrows.Length/2) * distanceBetweenArrows, prepareArrowYPos, 0f);
                    }
                    _arrows[i].localEulerAngles = new Vector3(0f, 180f, 0f);
                    _arrowsScript[i].canMove = false;

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
                _arrows[i].gameObject.SetActive(false);
                if (i % 2 == 0)
                {
                    _arrows[i].position = new Vector3(objectiveTransform.position.x - (i / 2) * distanceBetweenArrows, prepareArrowYPos, 0f);
                }
                else
                {
                    _arrows[i].position = new Vector3(objectiveTransform.position.x + (i / 2 + 1) * distanceBetweenArrows, prepareArrowYPos, 0f);
                }
                _arrows[i].rotation = new Quaternion(0f, 0f, 0f, 0f);
                _arrowsScript[i].canMove = false;

            }
            for (int i = numberOfArrows; i < _arrows.Length; ++i)
            {
                _arrows[i].gameObject.SetActive(false);
                _arrowsScript[i].canMove = false;
            }

        }

    }

    public void ShootUpArrow()
    {
        upArrow.gameObject.SetActive(true);
        upArrow.transform.position = new Vector3(transform.position.x + 0.18f, transform.position.y + 3.2f, transform.position.z - 0.07f);
        upArrow.ShootArrow();
    }

    public void ShootArrows()
    {
        upArrow.gameObject.SetActive(false);
        upArrow.canMove = false;
        _bossState = bossArcherIA.POSTSHOOT;
        _timeSinceStateChanged = 0.0f;
        _animator.speed = 1f;
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
        switch (_lastShootType)
        {
            case shootTypes.LEFT_TO_RIGHT:
            case shootTypes.RIGHT_TO_LEFT:
            case shootTypes.SIDES_TO_MIDDLE:
                oneByOne = true;
                numberOfArrows = _arrows.Length / 2;
                break;

            case shootTypes.ULTRA_DIFFICULT_ULTRA:
                oneByOne = true;
                numberOfArrows = _arrows.Length;
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
                if (timeShooting >= (i * timeToShootArrows / _arrows.Length))
                {
                    for (int j = i; j < i + 5; ++j)
                    {
                        _arrowsScript[j].ShootArrow();
                        _arrows[j].gameObject.SetActive(true);
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
                if (timeShooting >= (i * timeToShootArrows / (_arrows.Length * 2)))
                {
                    if (numberOfArrows % 2 != 0 && i == 0)
                    {
                        _arrows[i].gameObject.SetActive(true);
                        _arrowsScript[i].ShootArrow();
                    }
                    else
                    {
                        _arrows[i].gameObject.SetActive(true);
                        _arrowsScript[i].ShootArrow();
                        _arrows[i + 1].gameObject.SetActive(true);
                        _arrowsScript[i + 1].ShootArrow();
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