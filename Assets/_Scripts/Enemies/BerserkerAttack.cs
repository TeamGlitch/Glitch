using UnityEngine;
using System.Collections;

public class BerserkerAttack : MonoBehaviour {

    public BerserkerAI berserker;

    void OnTriggerEnter(Collider coll)
    {
        if ((berserker.states != BerserkerAI.enemy_states.DEATH) && (coll.gameObject.CompareTag("Player")))
        {
            // If is attacking hurts player
            if (berserker.states == BerserkerAI.enemy_states.ATTACK)
            {
                berserker.Attack();
            }
        }
    }
}
