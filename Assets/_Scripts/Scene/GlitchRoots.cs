using UnityEngine;
using System.Collections;
using InControl;

public class GlitchRoots : MonoBehaviour {
    public AudioClip fall;
    public AudioClip glitch;
    public GameObject rootAssociated;
    public Shader glitchShader;
    public Material preGlitchMaterial;
    public Shader plusShader;
    public RootsManager manager;
    public GameObject debris;
    public Player player;

    private Debris debrisScript;
    private Shader rootShader;
    private Material rootInitMaterial;
    private Renderer rootRender;
    private Rigidbody rigidDebris;

    void Start()
    {
        debrisScript = debris.GetComponent<Debris>();
        rigidDebris = debris.GetComponent<Rigidbody>();
        rootRender = rootAssociated.GetComponent<Renderer>();
        rootInitMaterial = rootRender.material;
        rootShader = rootRender.material.shader;
        player.PlayerDeadEvent += TurnToNormality;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            player.IncreaseActivableBox();
        }
    }

    void OnTriggerStay(Collider coll)
    {
        if (manager.isActivable && coll.CompareTag("Player"))
        {
            rootRender.material = preGlitchMaterial;
            player.boxUIActivated.SetActive(true);
            if (InputManager.ActiveDevice.Action4.IsPressed)
            {
                SoundManager.instance.PlaySingle(fall);
                rootRender.material.shader = glitchShader;
                StartCoroutine(DeactivateGlitch(2.0f));
                debrisScript.Fall();
                manager.isActivable = false;
            }
        }
        else
        {
            player.boxUIActivated.SetActive(false);
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (manager.isActivable && coll.CompareTag("Player"))
        {
            player.DecreaseActivableBox();
            TurnToNormality();
        }
    }

    // Coroutine to deactivate glitch effect
    IEnumerator DeactivateGlitch(float wait)
    {
        yield return new WaitForSeconds(wait);
        SoundManager.instance.PlaySingle(glitch);
        manager.AllGlitched();
    }

    public void RootGlitched()
    {
        rootRender.material.shader = plusShader;
    }

    public void TurnToNormality()
    {
        rootRender.material = rootInitMaterial;
        rootRender.material.shader = rootShader;
    }
}
