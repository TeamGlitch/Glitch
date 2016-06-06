using UnityEngine;
using System.Collections;

public class KnightAttack : MonoBehaviour {

    public KnightAI knight;

    void OnTriggerEnter(Collider coll)
    {
        // If is attacking hurts player
        if ((knight.states == KnightAI.enemy_states.ATTACK) && (coll.gameObject.CompareTag("Player")))
        {
            knight.Attack();
        }
    }
}
