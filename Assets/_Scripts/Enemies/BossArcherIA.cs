using UnityEngine;
using System.Collections;

public class BossArcherIA : MonoBehaviour {

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

    public float horizontalVelocity = 1.0f;
    public float jumpForceM2M = 1000.0f;
    public float jumpForceM2E = 1400.0f;
    public float jumpForceE2M = 1500.0f;
    public float jumpForceZ = 100f;
    private float _jumpForce;

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
    private Rigidbody[] _arrowsRigidBody;

    private float _maxLeft = -32.5f;
    private float _maxRight = 32.5f;
    public float prepareArrowYPos = 12f;

    public float timeToShootArrows = 3f;

    private shootTypes _lastShootType;

    public Transform objectiveTransform;

    private Animator _animator;

    private int _layerMask = (~((1 << 13) | (1 << 2) | (1 << 11) | (1 << 8))) | (1 << 9) | (1 << 0);

    private bool _fallingJump = false;
    private bool _areArrowsReady = false;
    private bool _firstStopPoint = true;

    private int lives = 3;

    private float _currentSpecialSpeed = 1f;

    #endregion

    #region Init & Update

    // Use this for initialization
    void Start () {

        _rigidbody = transform.GetComponent<Rigidbody> ();
        _boxCollider = transform.GetComponent<BoxCollider> ();
        _timeSinceStateChanged = 0.0f;
        _bossPos = bossArcherPos.MEDIUMRIGHT;
        _bossState = bossArcherIA.PRESHOOT;
        _timeSinceStateChanged = 0.0f;
        _jumpForce = jumpForceM2M;

        _arrows = new Transform[arrowPool.childCount];
        for(int i=0; i < arrowPool.childCount; ++i)
        {
            _arrows[i] = arrowPool.GetChild(i);
        }
        _arrowsRigidBody = new Rigidbody[arrowPool.childCount];
        for (int i = 0; i < _arrows.Length; ++i)
        {
            _arrowsRigidBody[i] = _arrows[i].GetComponent<Rigidbody>();
        }
        _animator = transform.GetComponent<Animator>();
        _firstStopPoint = true;
    }

    // Update is called once per frame
    void Update () {
	    switch(_bossState)
        {
            case bossArcherIA.MOVING:
                if(_movingRight)
                {
                    _rigidbody.velocity = new Vector3(horizontalVelocity, 0f, 0f);
                }
                else
                {
                    _rigidbody.velocity = new Vector3(-horizontalVelocity, 0f, 0f);
                }
                break;

            case bossArcherIA.FALLING_JUMP:
                _rigidbody.velocity = new Vector3(0f, 0f, 0f);
                if (_fallingJump)
                    _bossState = bossArcherIA.MOVING;
                break;

            case bossArcherIA.JUMPING:
                if (IsGrounded() && _rigidbody.velocity.y < 0)
                {
                    _bossState = bossArcherIA.FALLING_JUMP;
                }
                break;

            case bossArcherIA.PRESHOOT:
                _timeSinceStateChanged += Time.deltaTime;
                if(_timeSinceStateChanged >= timeInPreShoot)
                {
                    _bossState = bossArcherIA.SHOOTING;
                    _animator.speed = _currentSpecialSpeed;
                    _animator.SetTrigger("Attack");
                    _areArrowsReady = false;
                }
                break;

            case bossArcherIA.SHOOTING:
                if (!_areArrowsReady)
                {
                    _areArrowsReady = true;
                    int random = Random.Range(1, 10);
                    random = 9;
                    switch (random)
                    {
                        case 1:
                            PrepareArrows(shootTypes.LEFT_TO_RIGHT);
                            break;
                        case 2:
                            PrepareArrows(shootTypes.RIGHT_TO_LEFT);
                            break;
                        case 3:
                            PrepareArrows(shootTypes.THREE_NEAR_GLITCH);
                            break;
                        case 4:
                            PrepareArrows(shootTypes.FIVE_NEAR_GLITCH);
                            break;
                        case 5:
                            PrepareArrows(shootTypes.SEVEN_NEAR_GLITCH);
                            break;
                        case 6:
                            PrepareArrows(shootTypes.NINE_NEAR_GLITCH);
                            break;
                        case 7:
                            PrepareArrows(shootTypes.ELEVEN_NEAR_GLITCH);
                            break;
                        case 8:
                            PrepareArrows(shootTypes.SIDES_TO_MIDDLE);
                            break;
                        case 9:
                            PrepareArrows(shootTypes.ULTRA_DIFFICULT_ULTRA);
                            break;
                    }
                }
                break;

            case bossArcherIA.POSTSHOOT:
                _timeSinceStateChanged += Time.deltaTime;
                if(_timeSinceStateChanged >= timeInPostShoot)
                {
                    int random;
                    switch(_bossPos)
                    {
                        case bossArcherPos.MAXLEFT:
                            _movingRight = true;
                            _bossPos = bossArcherPos.MEDIUMLEFT;
                            _jumpForce = jumpForceE2M;
                            break;

                        case bossArcherPos.MEDIUMLEFT:
                            random = Random.Range(1, 3);
                            _jumpForce = jumpForceM2M;
//                            if (random == 1)
//                            {
                                _movingRight = true;
                                _jumpForce = jumpForceM2M;
                                _bossPos = bossArcherPos.MEDIUMRIGHT;
/*                            }
                            else
                            {
                                _movingRight = false;
                                _jumpForce = jumpForceM2E;
                                _bossPos = bossArcherPos.MAXLEFT;
                            }*/
                            break;
                        case bossArcherPos.MEDIUMRIGHT:
                            random = Random.Range(1, 3);
                            _jumpForce = jumpForceM2M;
/*                            if (random == 1)
                            {
                                _movingRight = true;
                                _jumpForce = jumpForceM2E;
                                _bossPos = bossArcherPos.MAXRIGHT;
                            }
                            else
                            {*/
                                _movingRight = false;
                                _jumpForce = jumpForceM2M;
                                _bossPos = bossArcherPos.MEDIUMLEFT;
                            //}
                            break;
                        case bossArcherPos.MAXRIGHT:
                            _movingRight = false;
                            _bossPos = bossArcherPos.MEDIUMRIGHT;
                            _jumpForce = jumpForceE2M;
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
                    _rigidbody.velocity = new Vector3(0f, 0f, 0f);
                    break;

            case bossArcherIA.HITTED:
                break;

        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            HitArcher();
        }
    }

    #endregion

    #region Colliders

    public void OnTriggerEnter(Collider coll)
    {
        if(coll.transform.name == "JumpPoint" && _bossState == bossArcherIA.MOVING)
        {
            if(_bossPos == bossArcherPos.MAXRIGHT)
            {
                _rigidbody.AddForce(new Vector3(0f, _jumpForce, -jumpForceZ));                
            }
            else if (_bossPos == bossArcherPos.MAXLEFT)
            {
                _rigidbody.AddForce(new Vector3(0f, _jumpForce, -jumpForceZ));
            }
            else if (_bossPos == bossArcherPos.MEDIUMLEFT && _movingRight)
            {
                _rigidbody.AddForce(new Vector3(0f, _jumpForce, jumpForceZ));
            }
            else if (_bossPos == bossArcherPos.MEDIUMRIGHT && !_movingRight)
            {
                _rigidbody.AddForce(new Vector3(0f, _jumpForce, jumpForceZ));
            }
            else
            {
                _rigidbody.AddForce(new Vector3(0f, _jumpForce, 0f));                
            }
            _bossState = bossArcherIA.JUMPING;
            _fallingJump = false;
            _animator.SetTrigger("Jump");
        }
        else if (coll.transform.name == "StopPoint" && _firstStopPoint)
        {
            _firstStopPoint = false;
        }
        else if (coll.transform.name == "StopPoint" && !_firstStopPoint)
        {
            if (_bossPos == bossArcherPos.MEDIUMLEFT || _bossPos == bossArcherPos.MEDIUMRIGHT)
                transform.position = new Vector3(transform.position.x, transform.position.y, 17.5f-5.34f);
            _rigidbody.velocity = new Vector3(0.0f, 0.0f, 0.0f);
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
        if(_bossState == bossArcherIA.DEAD && !coll.collider.CompareTag("BossHit"))
        {
            _animator.speed = 1f;
            _animator.SetTrigger("GroundHitted");
        }
        else if (coll.collider.CompareTag("BossHit"))
        {
            transform.localEulerAngles = new Vector3(0f, 180f, 0f);
            _animator.speed = 1f;
            --lives;
            if (lives == 0)
            {
                _animator.SetTrigger("LastHitted");
                _bossState = bossArcherIA.DEAD;
            }
            else if(lives == 1)
            {
                timeToShootArrows = 1;
                timeInPreShoot = 0f;
                timeInPostShoot = 0f;
                _animator.SetTrigger("Hitted");
                _rigidbody.velocity = new Vector3(0f, _rigidbody.velocity.y, 0f);
                _bossState = bossArcherIA.HITTED;
            }
            else if(lives == 2)
            {
                timeToShootArrows = 2;
                timeInPreShoot = 1f;
                timeInPostShoot = 1f;
                _animator.SetTrigger("Hitted");
                _rigidbody.velocity = new Vector3(0f, _rigidbody.velocity.y, 0f);
                _bossState = bossArcherIA.HITTED;
            }
        }
    }

    public void HitArcher()
    {
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
            _rigidbody.velocity = new Vector3(0f, _rigidbody.velocity.y, 0f);
            _bossState = bossArcherIA.HITTED;
        }
        else if (lives == 2)
        {
            timeToShootArrows = 2;
            timeInPreShoot = 1f;
            timeInPostShoot = 1f;
            _animator.SetTrigger("Hitted");
            _rigidbody.velocity = new Vector3(0f, _rigidbody.velocity.y, 0f);
            _bossState = bossArcherIA.HITTED;
        }
    }

    public void InsultingAnimationEnded()
    {
        if (lives == 2)
            _currentSpecialSpeed = 2f;
        else if (lives == 1)
            _currentSpecialSpeed = 3f;

        _bossState = bossArcherIA.PRESHOOT;
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
        if(_bossState == bossArcherIA.TURNING_LEFT_TO_RUN)
        {
            transform.localEulerAngles = new Vector3(0f, 90f, 0f);
            _bossState = bossArcherIA.MOVING;
        }
        else if(_bossState == bossArcherIA.TURNING_RIGHT_TO_RUN)
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
        transform.position = auxPos;
    }

    #endregion

    #region Shoot

    private void PrepareArrows(shootTypes shootType)
    {
        _lastShootType = shootType;
        float distance = _maxRight - _maxLeft;
        float distanceBetweenArrows = distance / (_arrows.Length/2);
        bool shootDependingObjective = false;
        int numberOfArrows = 0;
        switch (shootType)
        {
            case shootTypes.LEFT_TO_RIGHT:
                for (int i = 0; i < _arrows.Length; ++i)
                {
                    _arrows[i].gameObject.SetActive(false);
                    _arrows[i].position = new Vector3(_maxLeft + i * distanceBetweenArrows, prepareArrowYPos, 0f);
                    _arrows[i].rotation = new Quaternion(0f,0f,0f,0f);
                    _arrowsRigidBody[i].velocity = Vector3.zero;
                    _arrowsRigidBody[i].angularVelocity = Vector3.zero;
                }
                break;

            case shootTypes.RIGHT_TO_LEFT:
                for (int i = 0; i < _arrows.Length; ++i)
                {
                    _arrows[i].gameObject.SetActive(false);
                    _arrows[i].position = new Vector3(_maxRight - i * distanceBetweenArrows, prepareArrowYPos, 0f);
                    _arrows[i].rotation = new Quaternion(0f, 0f, 0f, 0f);
                    _arrowsRigidBody[i].velocity = Vector3.zero;
                    _arrowsRigidBody[i].angularVelocity = Vector3.zero;
                }
                break;

            case shootTypes.SIDES_TO_MIDDLE:
                for (int i = 0; i < _arrows.Length; ++i)
                {
                    _arrows[i].gameObject.SetActive(false);
                    if (i % 2 == 0)
                    {
                        _arrows[i].position = new Vector3(_maxRight - i/2 * distanceBetweenArrows, prepareArrowYPos, 0f);
                    }
                    else
                    {
                        _arrows[i].position = new Vector3(_maxLeft + (i / 2 + 1) * distanceBetweenArrows, prepareArrowYPos, 0f);
                    }
                    _arrows[i].rotation = new Quaternion(0f, 0f, 0f, 0f);
                    _arrowsRigidBody[i].velocity = Vector3.zero;
                    _arrowsRigidBody[i].angularVelocity = Vector3.zero;
                }
                break;

            case shootTypes.ULTRA_DIFFICULT_ULTRA:
                for (int i = 0; i < _arrows.Length; ++i)
                {
                    _arrows[i].gameObject.SetActive(false);
                    if(i < (_arrows.Length/2))
                    {
                        _arrows[i].position = new Vector3(_maxLeft + i * distanceBetweenArrows, prepareArrowYPos, 0f);
                    }
                    else
                    {
                        _arrows[i].position = new Vector3(_maxRight - (i-_arrows.Length) * distanceBetweenArrows, prepareArrowYPos, 0f);
                    }
                    _arrows[i].rotation = new Quaternion(0f, 0f, 0f, 0f);
                    _arrowsRigidBody[i].velocity = Vector3.zero;
                    _arrowsRigidBody[i].angularVelocity = Vector3.zero;
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

        if(shootDependingObjective)
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
                _arrowsRigidBody[i].velocity = Vector3.zero;
                _arrowsRigidBody[i].angularVelocity = Vector3.zero;
            }
            for (int i = numberOfArrows; i < _arrows.Length; ++i)
            {
                _arrows[i].gameObject.SetActive(false);
            }

        }

    }

    public void ShootArrows()
    {
        _bossState = bossArcherIA.POSTSHOOT;
        _timeSinceStateChanged = 0.0f;
        _animator.speed = 1f;
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
                numberOfArrows = _arrows.Length/2;
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
                    for (int j = i; j < i + 5; ++j )
                    {
                        _arrowsRigidBody[j].detectCollisions = true;
                        _arrowsRigidBody[j].isKinematic = false;
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
                        _arrowsRigidBody[i].isKinematic = false;
                        _arrowsRigidBody[i].detectCollisions = true;
                    }
                    else
                    {
                        _arrows[i].gameObject.SetActive(true);
                        _arrowsRigidBody[i].detectCollisions = true;
                        _arrowsRigidBody[i].isKinematic = false;
                        _arrows[i + 1].gameObject.SetActive(true);
                        _arrowsRigidBody[i+1].detectCollisions = true;
                        _arrowsRigidBody[i+1].isKinematic = false;
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