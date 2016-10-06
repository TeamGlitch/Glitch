using UnityEngine;
using System.Collections;

public class ArrowScript : MonoBehaviour {

    public float speed = 10.0f;
    public Player player;
    public bool isInLeft = false;
    public World world;
    public BoxCollider arrowCollider;

    private float timeAlive = 3.0f;
    private int damage = 1;

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.gameObject.CompareTag("Player"))
        {
            player.DecrementLives(damage);
        }
        timeAlive = 3.0f;
        ResetArrow();
    }
	
	void Update () 
    {
        // Control fps
        if (world.doUpdate)
        {
            transform.Translate(Vector2.down * world.lag * speed);
            arrowCollider.enabled = true;
            timeAlive -= world.lag;

            // Arrow time of live
            if (timeAlive <= 0)
            {
                timeAlive = 3.0f;
                ResetArrow();
            }
        }
	}

    // Resets arrow position
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
