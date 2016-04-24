using UnityEngine;
using System.Collections;

public class KnightDead : MonoBehaviour {

    public KnightAI knight;

    void OnTriggerEnter(Collider coll)
    {
        if ((knight.states != KnightAI.enemy_states.DEATH) && (coll.gameObject.CompareTag("Player")))
        {
            knight.Attacked();
        }
    }
}
