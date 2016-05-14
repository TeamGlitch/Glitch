using UnityEngine;
using System.Collections;

public class BerserkerDead : MonoBehaviour {

    public BerserkerAI berserker;
    public BoxCollider headCollider;

    void OnTriggerEnter(Collider coll)
    {

        if (berserker.states != BerserkerAI.enemy_states.DEATH)
        {
            if (coll.gameObject.CompareTag("Player"))
            {
                if (berserker.player.transform.position.y >= (transform.position.y + headCollider.bounds.extents.y))
                {
                    berserker.Attacked();
                }
                else
                {
                    if (berserker.sight == true)
                    {
                        berserker.Attack();
                    }
                }
            }
            else
            {
                if ((!coll.gameObject.CompareTag("PatrolPoint")) && (!coll.gameObject.CompareTag("LimitPoint")))
                {
                    berserker.states = BerserkerAI.enemy_states.IMPACT;
                }
            }
        }
    }
}
