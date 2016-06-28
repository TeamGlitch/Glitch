using UnityEngine;
using System.Collections;

public class StartBattle : MonoBehaviour {

    public BossArcherIA boss;

    void OnTriggerExit(Collider coll)
    {
        boss.start = true;
        GetComponent<Collider>().enabled = false;
    }
}
