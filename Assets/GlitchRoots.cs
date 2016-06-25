using UnityEngine;
using System.Collections;
using InControl;

public class GlitchRoots : MonoBehaviour {
    public GameObject rootAssociated;
    public Shader glitchShader;
    public Shader plusShader;
    public RootsManager manager;
    public GameObject debris;
    public Player player;

    private BreakableRock[] debrisScript;
    private Shader rootShader;
    private Renderer rootRender;
    private Rigidbody[] rocksRigids;
    private Rigidbody rigidDebris;

    void Start()
    {
        rigidDebris = debris.GetComponent<Rigidbody>();
        rocksRigids = new Rigidbody[debris.transform.childCount];
        debrisScript = new BreakableRock[debris.transform.childCount];
        for (int i = 0; i < debris.transform.childCount; ++i)
        {
            debrisScript[i] = debris.transform.GetChild(i).GetComponent<BreakableRock>();
            rocksRigids[i] = debris.transform.GetChild(i).GetComponent<Rigidbody>();
        }
        rootRender = rootAssociated.GetComponent<Renderer>();
        rootShader = rootRender.material.shader;
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
            player.boxUIActivated.SetActive(true);
            if (InputManager.ActiveDevice.Action4.IsPressed)
            {
                rootRender.material.shader = glitchShader;
                StartCoroutine(DeactivateGlitch(2.0f));
                rocksRigids[0].isKinematic = false;
                rocksRigids[1].isKinematic = false;
                rocksRigids[2].isKinematic = false;
                rocksRigids[3].isKinematic = false;
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
        if (coll.CompareTag("Player"))
        {
            player.DecreaseActivableBox();
        }
    }

    // Coroutine to deactivate glitch effect
    IEnumerator DeactivateGlitch(float wait)
    {
        yield return new WaitForSeconds(wait);
        manager.AllGlitched();
    }

    public void RootGlitched()
    {
        rootRender.material.shader = plusShader;
    }

    public void TurnToNormality()
    {
        rootRender.material.shader = rootShader;
        rocksRigids[0].isKinematic = true;
        rocksRigids[1].isKinematic = true;
        rocksRigids[2].isKinematic = true;
        rocksRigids[3].isKinematic = true;
        debrisScript[0].Restart();
        debrisScript[1].Restart();
        debrisScript[2].Restart();
        debrisScript[3].Restart();
    }
}
