using UnityEngine;
using System.Collections;

public class CubeTrap : MonoBehaviour {

    public ElevatorTrap elevator;
    public Rigidbody rigid;

    void OnCollisionEnter(Collision coll)
    {
        if (!coll.gameObject.CompareTag("Player"))
        {
            elevator.impact = true;
            rigid.isKinematic = true;
        }
    }
}
