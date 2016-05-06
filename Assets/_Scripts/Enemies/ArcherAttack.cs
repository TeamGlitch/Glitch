using UnityEngine;
using System.Collections;

public class ArcherAttack : MonoBehaviour {

    public ArcherAI archer;

    void OnTriggerEnter(Collider coll)
    {
        if ((archer.states != ArcherAI.enemy_states.DEATH) && (coll.gameObject.CompareTag("Player")))
        {
            // If is attacking hurts player
            if (archer.states == ArcherAI.enemy_states.MELEE_ATTACK)
            {
                archer.Kick();
            }
        }
    }
}
