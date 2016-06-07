using UnityEngine;
using System.Collections;

public class BerserkerAttack : MonoBehaviour {

    public BerserkerAI berserker;

    void OnTriggerEnter(Collider coll)
    {
        // If is attacking hurts player
        if ((berserker.states == BerserkerAI.enemy_states.ATTACK) && (coll.gameObject.CompareTag("Player")))
        {
            berserker.Attack();
        }
    }
}
