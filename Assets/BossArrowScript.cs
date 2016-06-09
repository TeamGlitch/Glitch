using UnityEngine;
using System.Collections;

public class BossArrowScript : MonoBehaviour {

    public Player player;
    private Rigidbody _rigidBody;

    void Start()
    {
        _rigidBody = transform.GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.transform.CompareTag("Player"))
        {
            player.Death();
        }
        else
        {
            _rigidBody.detectCollisions = false;
            _rigidBody.isKinematic = true;
        }
    }

}
