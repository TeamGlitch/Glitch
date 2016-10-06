using UnityEngine;
using System.Collections;

public class StartElevator : MonoBehaviour {

    public Elevator elevator;
    public GameObject sideCollider1;
    public GameObject sideCollider2;
    public Transform player;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            player.parent = elevator.transform;
            elevator.enabled = true;
            sideCollider1.SetActive(true);
            sideCollider2.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
