using UnityEngine;
using System.Collections;

public class ArcherDead : MonoBehaviour {

    public ArcherAI archer;
    public BoxCollider headCollider;

    void OnTriggerEnter(Collider coll)
    {
        // If Glitch contacts archer upper, archer dies
        if (archer.player.transform.position.y >= (transform.position.y + headCollider.bounds.extents.y) && (coll.gameObject.CompareTag("Player")))
        {
            archer.Defeated();
        }
    }
}
