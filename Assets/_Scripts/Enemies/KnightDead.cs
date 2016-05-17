﻿using UnityEngine;
using System.Collections;

public class KnightDead : MonoBehaviour {

    public KnightAI knight;
    public BoxCollider headCollider;

    void OnTriggerEnter(Collider coll)
    {

        if ((knight.states != KnightAI.enemy_states.DEATH) && (coll.gameObject.CompareTag("Player")))
        {
            if (((knight.player.transform.position.y - coll.bounds.extents.y) >= (transform.position.y + headCollider.bounds.extents.y)) && (knight.attacked == false))
            {
                knight.Attacked();
            }
        }
    }
}
