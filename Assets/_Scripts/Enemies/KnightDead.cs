using UnityEngine;
using System.Collections;

public class KnightDead : MonoBehaviour {

    public KnightAI knight;
    public BoxCollider headCollider;

    void OnTriggerEnter(Collider coll)
    {
        // If Glitch contacts knight upper, archer dies
        if ((knight.player.transform.position.y >= (transform.position.y + headCollider.bounds.extents.y)) && (coll.gameObject.CompareTag("Player")))
        {
            knight.Attacked();
            knight.rigid.isKinematic = true;
            knight.collider.enabled = false;
            headCollider.enabled = false;
        }
    }
}
