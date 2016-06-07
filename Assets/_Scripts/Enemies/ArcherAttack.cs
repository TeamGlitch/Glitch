using UnityEngine;
using System.Collections;

public class ArcherAttack : MonoBehaviour {

    public ArcherAI archer;

    void OnTriggerEnter(Collider coll)
    {

        // If is attacking hurts player
        if ((archer.states == ArcherAI.enemy_states.MELEE_ATTACK) && (coll.gameObject.CompareTag("Player")))
        {
            archer.Kick();
        }
    }
}
