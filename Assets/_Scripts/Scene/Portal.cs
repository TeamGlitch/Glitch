using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {

    public Transform portalSprite;
    public float rotationSpeed;

    private float timeEntered = -1;

    public PlayerController PC;

    public DialogueScript dialogueScript;
    public DialoguePoint finalDialoguePoint;

    public MainCamera mainCamera;
    private bool cameraBlocked = false;

    public EndPointScript endpoint;
    private bool endpointActivated = false;

    public MeshRenderer portalRenderer;
    public GameObject dontGoBackMesh;

	// Update is called once per frame
	void Update () {

        if (!cameraBlocked && finalDialoguePoint.used && dialogueScript.state == DialogueScript.dialogueBoxState.OFF)
        {
            mainCamera.move = false;
            cameraBlocked = true;
            dontGoBackMesh.SetActive(true);
        }

        if(timeEntered == -1 || Time.time - timeEntered > 3f)
            portalSprite.Rotate(new Vector3(0, 0, 1), rotationSpeed * Time.deltaTime);
        else
        {
            float extraPush = (1-((Time.time - timeEntered) / 3f)) * 15 * rotationSpeed;
            portalSprite.Rotate(new Vector3(0, 0, 1), (extraPush + rotationSpeed) * Time.deltaTime);
        }

        if (timeEntered != -1 && !endpointActivated && Time.time - timeEntered > 2.0f)
        {
            endpoint.EndLevel(PC);
            endpointActivated = true;
        }

        if (timeEntered != -1)
        {
            float percent = (Time.time - timeEntered) / 5f;

            if (percent > 1)
                percent = 1;

            portalRenderer.material.SetFloat("_Scale", (472f * percent) + 28f);
            portalRenderer.material.SetFloat("_Speed", (-12.5f * percent) + 34.5f);
        }

	}

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            PC.transform.FindChild("Sprite").GetComponent<SpriteRenderer>().enabled = false;
            PC.allowMovement = false;
            timeEntered = Time.time;
            portalRenderer.material.SetFloat("_Position", 0.5f);
        }
    }
}
