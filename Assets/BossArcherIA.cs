using UnityEngine;
using System.Collections;

public class BossArcherIA : MonoBehaviour {

    public enum bossArcherIA
    {
        IDLE,
        SHOOTING,
        PRESHOOT,
        POSTSHOOT,
        MOVING,
        JUMPING
    }

    public enum bossArcherPos
    {
        MAXLEFT,
        MEDIUMLEFT,
        MEDIUMRIGHT,
        MAXRIGHT
    }

    public float horizontalVelocity = 1.0f;
    public float jumpForceM2M = 1000.0f;
    public float jumpForceE2M = 1500.0f;
    private float _jumpForce;

    public float timeInWait = 1.0f;
    private float _timeSinceStateChanged;
    private bool _movingRight = true;

    [SerializeField]
    private bossArcherIA _bossState;
    private bossArcherPos _bossPos;
    private Rigidbody _rigidbody;
    private BoxCollider _boxCollider;

	// Use this for initialization
	void Start () {
        _bossState = bossArcherIA.IDLE;
        _rigidbody = transform.GetComponent<Rigidbody> ();
        _boxCollider = transform.GetComponent<BoxCollider> ();
        _timeSinceStateChanged = 0.0f;
        _bossPos = bossArcherPos.MEDIUMRIGHT;
        _jumpForce = jumpForceM2M;
	}
	
	// Update is called once per frame
	void Update () {
	    switch(_bossState)
        {
            case bossArcherIA.IDLE:
                _timeSinceStateChanged += Time.deltaTime;
                int random;
               if(_timeSinceStateChanged > timeInWait)
                {
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
                            _jumpForce = jumpForceM2M;
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
                            _jumpForce = jumpForceE2M;
                            break;
                    }
                    _bossState = bossArcherIA.MOVING;
                }
                break;

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

            case bossArcherIA.JUMPING:
                if (IsGrounded() && _rigidbody.velocity.y < 0)
                    _bossState = bossArcherIA.MOVING;
                break;
        }
	}

    public void OnTriggerEnter(Collider coll)
    {
        if(coll.transform.name == "JumpPoint" && _bossState == bossArcherIA.MOVING)
        {
            _rigidbody.AddForce(new Vector3(0f,_jumpForce,0f));
            _bossState = bossArcherIA.JUMPING;
        }
        if(coll.transform.name == "StopPoint")
        {
            _bossState = bossArcherIA.IDLE;
            _rigidbody.velocity = new Vector3(0.0f, _rigidbody.velocity.y, 0.0f);
            _timeSinceStateChanged = 0.0f;
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, _boxCollider.bounds.extents.y + 0.1f,  1, QueryTriggerInteraction.Ignore);
    }
}
