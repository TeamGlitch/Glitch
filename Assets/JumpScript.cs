using UnityEngine;
using System.Collections;

public class JumpScript : MonoBehaviour {

    public float upForce = 0.0f;
    public float forwardForce = 0.0f;
    public bool fromLeft = false;

    private RaycastHit hit;

    void OnTriggerStay(Collider coll)
    {
        Ray ray = new Ray(transform.position, coll.transform.position - transform.position);
        Physics.Raycast(ray, out hit);

        // The ray is from knight to player, then collides down of player (-transform.up = down)
        if (((hit.normal == -transform.right) && (fromLeft == false)) || ((hit.normal == transform.right) && (fromLeft == true)))
        {
            if (coll.gameObject.CompareTag("Archer"))
            {
                coll.transform.Translate((Vector3.forward.x*forwardForce), upForce, 0.0f);
            }
        }
    }
}
