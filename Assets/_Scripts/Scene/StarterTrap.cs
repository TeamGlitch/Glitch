using UnityEngine;
using System.Collections;

public class StarterTrap : MonoBehaviour {

    public ElevatorTrap elevator;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            elevator.enabled = true;
        }
    }
}
