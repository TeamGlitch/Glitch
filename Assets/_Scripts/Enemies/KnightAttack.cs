using UnityEngine;
using System.Collections;

public class KnightAttack : MonoBehaviour {

    public KnightAI knight;

    void OnTriggerEnter(Collider coll)
    {
        if ((knight.states != KnightAI.enemy_states.DEATH) && (coll.gameObject.CompareTag("Player")))
        {
            // If is attacking hurts player
            if (knight.states == KnightAI.enemy_states.ATTACK)
            {
                knight.Attack();
            }
        }
    }
}
