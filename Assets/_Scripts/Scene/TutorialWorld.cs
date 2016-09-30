using UnityEngine;
using System.Collections;

public class TutorialWorld : MonoBehaviour {

    public Player player;
    public TeleportScript teleportScript;
    public DynamicCamera DynamicCamera;
    public Camera mainCamera;
    public Canvas background;
    public GameObject[] activate;

    void Awake(){
        for (int i = 0; i < activate.Length; i++)
            activate[i].SetActive(false);
        player.godmode = true;
        teleportScript.allowTeleport = false;
    }

	// Update is called once per frame
    void Update () {
        if (!DynamicCamera.gameObject.activeInHierarchy)
        {
            for (int i = 0; i < activate.Length; i++)
                activate[i].SetActive(true);
            background.worldCamera = mainCamera;
            this.enabled = false;
        }
	}
}
