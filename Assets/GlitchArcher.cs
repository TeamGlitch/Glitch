using UnityEngine;
using System.Collections;
using InControl;

public class GlitchArcher : MonoBehaviour {

    public Player player;
    public Shader distorsion;
    public Renderer archerRenderer;
    public Door door;
    public GameObject endDialogue;

    private Shader previousShader;
    private BoxCollider boxCollider;
    private float random;

    public delegate void BossGlitchedDelegate();
    public event BossGlitchedDelegate BossGlitchedEvent;

    void Start()
    {
        previousShader = archerRenderer.material.shader;
        boxCollider = GetComponent<BoxCollider>();
    }

    void OnTriggerStay(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            player.boxUIActivated.SetActive(true);
            if (InputManager.ActiveDevice.Action4.IsPressed)
            {
                Invoke("GlitchOn", 0.0f);
                boxCollider.enabled = false;
                door.OpenDoor();
                endDialogue.SetActive(true);
                if (BossGlitchedEvent != null)
                    BossGlitchedEvent();
            }
        }
        else
        {
            player.boxUIActivated.SetActive(false);
        }
    }

    public void GlitchOn()
    {
        archerRenderer.material.shader = distorsion;
        random = Random.Range(0.0f, 3.0f);
        Invoke("GlitchOff", random);
    }

    public void GlitchOff()
    {
        archerRenderer.material.shader = previousShader;
        random = Random.Range(0.0f, 3.0f);
        Invoke("GlitchOn", random);
    }
}
