using UnityEngine;
using System.Collections;

public class BerserkerDead : MonoBehaviour {

    public BerserkerAI berserker;
    public BoxCollider headCollider;

    void OnTriggerEnter(Collider coll)
    {
        // If collides with player and player is upper then hurts berserker
        // Else if berserker is chasing and impacts with something change state to impact
        if (coll.gameObject.CompareTag("Player"))
        {
            if (berserker.player.transform.position.y >= (transform.position.y + headCollider.bounds.extents.y))
            {
                berserker.Attacked();
            }
        }
        else
        {
            if ((!coll.gameObject.CompareTag("PatrolPoint")) && (!coll.gameObject.CompareTag("LimitPoint")) && (berserker.states == BerserkerAI.enemy_states.CHASE))
            {
                berserker.states = BerserkerAI.enemy_states.IMPACT;
            }
        }
    }
}
