using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class BossDialoguePoint : MonoBehaviour {

    public BossArcherIA boss;
    public BossStageCamera camera;
	public DialogueScript dialogueScript;
    public PlayerController player;
    public Door door;
	public int sceneNum;

	private bool used = false;
    private bool readyForBattle = false;

	void OnTriggerEnter(Collider coll)
    {
		if((coll.gameObject.CompareTag("Player")) && !used)
		{
			dialogueScript.callScene(sceneNum);
			used = true;
		}
	}

    void Update()
    {
        if (used)
        {
            if (player.allowMovement)
            {
                readyForBattle = true;
                used = false;
            }
        }

        if (readyForBattle)
        {
            door.enabled = true;
            boss.start = true;
            Invoke("ZoomCamera", 1.0f);
            readyForBattle = false;
        }
    }

    void ZoomCamera()
    {
        camera.ZoomIn();
        gameObject.SetActive(false);
    }
}
