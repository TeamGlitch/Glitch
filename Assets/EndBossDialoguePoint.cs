using UnityEngine;
using System.Collections;

public class EndBossDialoguePoint : MonoBehaviour {

    public BossStageCamera camera;
    public DialogueScript dialogueScript;
    public PlayerController player;
    public int sceneNum;

    private bool used = false;

    void Update()
    {
        if (used)
        {
            if (player.allowMovement)
            {
                camera.FinalZoomArcherOut();
                enabled = false;
                used = false;
            }
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if ((coll.gameObject.CompareTag("Player")) && !used)
        {
            camera.FinalZoomArcherIn();
            Invoke("Dialogue", 0.7f);
        }
    }

    public void Dialogue()
    {
        dialogueScript.callScene(sceneNum);
        used = true;
    }
}
