using UnityEngine;
using System.Collections;

public class LimitPoint : MonoBehaviour {

    public bool fall;
    public BoxCollider collider;

    public void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("Berserker"))
        {
            collider.isTrigger = true;
        }
    }

    public void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag("Berserker"))
        {
            collider.isTrigger = false;
        }
    }
}
