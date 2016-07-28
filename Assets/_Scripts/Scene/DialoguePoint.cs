using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class DialoguePoint : MonoBehaviour
{

    public DialogueScript dialogueScript;
    public int sceneNum;
    private bool used = false;

    void OnTriggerEnter(Collider coll)
    {
        if ((coll.gameObject.CompareTag("Player")) && !used)
        {
            dialogueScript.callScene(sceneNum);
            used = true;
        }
    }
}