using UnityEngine;
using System.Collections;

public class ArcherDead : MonoBehaviour {

    public ArcherAI archer;

    void OnTriggerEnter(Collider coll)
    {
        if ((archer.states != ArcherAI.enemy_states.DEATH) && (coll.gameObject.CompareTag("Player")))
        {
            archer.Defeated();
        }
    }
}
