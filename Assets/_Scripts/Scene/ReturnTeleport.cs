using UnityEngine;
using System.Collections;

public class ReturnTeleport : MonoBehaviour {

    public TeleportScript tp;

    void OnTriggerEnter(Collider coll)
    {
        //If the player has gone trought the collider and is exiting...
        if (coll.gameObject.CompareTag("Player"))
        {
            tp.allowTeleport = true;
            GameObject.Destroy(this.gameObject);
        }

    }
}
