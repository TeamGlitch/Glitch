using UnityEngine;
using System.Collections;

public class ArrowPointScript : MonoBehaviour {

    public GoArrowsScript arrowScript;
    public Vector3 direction_1;
    public Vector3 direction_2;

    private bool active = true;
    void OnTriggerEnter(Collider coll)
    {

        //If there's a collision with the player
        if ((coll.gameObject.CompareTag("Player")) && active)
        {
            arrowScript.activate(direction_1, direction_2);
            active = false;
        }

    }
}
