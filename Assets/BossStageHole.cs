using UnityEngine;
using System.Collections;

public class BossStageHole : MonoBehaviour
{

    public PlayerController player;
    public Transform left;
    public Transform right;
    public BossArcherIA boss;

    void OnTriggerStay(Collider coll)
    {
        if (coll.CompareTag("Player") && boss.holesActivated)
        {
            if (player.playerMovingType == PlayerController.moving_type.GOING_LEFT)
            {
                player.transform.position = left.position;
            }
            else
            {
                player.transform.position = right.position;
            }
        }
    }
}