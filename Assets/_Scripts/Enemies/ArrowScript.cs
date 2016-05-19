using UnityEngine;
using System.Collections;

public class ArrowScript : MonoBehaviour {

    public float speed = 10.0f;
    public Player player;
    public bool isInLeft = false;
    public World world;
    public BoxCollider arrowCollider;

    private float timeAlive = 3.0f;
    private float damage = 1.0f;

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.gameObject.CompareTag("Player"))
        {
            player.DecrementLives(damage);
            player.ReactToAttack(transform.position.x);
        }
        ResetArrow();
    }
	
	void Update () 
    {
        if (world.doUpdate)
        {
            transform.Translate(Vector2.down * world.lag * speed);
            arrowCollider.enabled = true;
            timeAlive -= world.lag;
            if (timeAlive <= 0)
            {
                timeAlive = 3.0f;
                ResetArrow();
            }
        }
	}

   private void ResetArrow()
    {
        if (isInLeft)
        {
            transform.eulerAngles = new Vector3(0.0f, 180.0f, 90.0f);
        }
        else
        {
            transform.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
        }
        gameObject.SetActive(false);
    }
}
