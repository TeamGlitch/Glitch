using UnityEngine;
using System.Collections;

public class StartBattle : MonoBehaviour {

    public BossArcherIA boss;
    public BossStageCamera camera;

    void OnTriggerExit(Collider coll)
    {
        boss.start = true;
        GetComponent<Collider>().enabled = false;
        camera.ZoomIn();
    }
}
