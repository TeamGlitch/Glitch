using UnityEngine;
using System.Collections;

public class StopElevator : MonoBehaviour {

    public Elevator elevator;
    public GameObject sideCollider1;
    public GameObject sideCollider2;
    public Transform player;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Elevator"))
        {
            player.parent = null;
            elevator.enabled = false;
            sideCollider1.SetActive(false);
            sideCollider2.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
