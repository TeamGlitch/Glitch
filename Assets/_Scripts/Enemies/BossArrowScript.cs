using UnityEngine;
using System.Collections;

public class BossArrowScript : MonoBehaviour {

    public Player player;
    public World world;
    private Rigidbody rigidBody;
    public float speed = 10f;
    public bool canMove;

    void Start()
    {
        rigidBody = transform.GetComponent<Rigidbody>();
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
		if(rigidBody != null)
	        rigidBody.detectCollisions = true;
        canMove = true;
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.transform.CompareTag("Player"))
        {
            player.DecrementLives(1);
        }
        else if(!coll.transform.CompareTag("Archer"))
        {
            canMove = false;
            rigidBody.detectCollisions = false;
        }
    }

}
