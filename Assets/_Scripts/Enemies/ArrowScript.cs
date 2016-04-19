using UnityEngine;
using System.Collections;

public class ArrowScript : MonoBehaviour {

    public float speed = 10.0f;
    public Player player;
    public bool isInLeft = false;

    private float timeAlive = 5.0f;
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
        transform.Translate(Vector2.down * Time.deltaTime*speed);

        timeAlive -= Time.deltaTime;
        if (timeAlive <= 0)
        {
            timeAlive = 5.0f;
            ResetArrow();
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
