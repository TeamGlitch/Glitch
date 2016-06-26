using UnityEngine;
using System.Collections;

public class BossArrowScript : MonoBehaviour {

    public Player player;
    public World world;
    private Rigidbody _rigidBody;
    public float speed = 10f;
    public bool canMove;

    void Start()
    {
        _rigidBody = transform.GetComponent<Rigidbody>();
        canMove = false;
    }


    void Update()
    {
        // Control fps
        if (world.doUpdate && canMove)
        {
            transform.Translate(Vector2.down * world.lag * speed);
        }
    }

    public void ShootArrow()
    {
        _rigidBody.detectCollisions = true;
        canMove = true;
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.transform.CompareTag("Player"))
        {
            player.Death();
        }
        else if(!coll.transform.CompareTag("Archer"))
        {
            canMove = false;
            _rigidBody.detectCollisions = false;
        }
    }

}
