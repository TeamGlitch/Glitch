using UnityEngine;
using System.Collections;

public class KnightDead : MonoBehaviour {

    public KnightAI knight;

    void OnTriggerEnter(Collider coll)
    {

        if ((knight.states != KnightAI.enemy_states.DEATH) && (coll.gameObject.CompareTag("Player")))
        {
            if (knight.player.transform.position.y > (transform.position.y + transform.lossyScale.y/2))
            {
                knight.Attacked();
            }
            else
            {
                if (knight.sight == true)
                {
                    knight.Attack();
                }
            }
        }
    }
}
