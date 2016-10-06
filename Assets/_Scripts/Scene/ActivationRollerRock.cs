using UnityEngine;
using System.Collections;

public class ActivationRollerRock : MonoBehaviour {

    public GameObject rollerRock;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            rollerRock.SetActive(true);
        }
    }
}
