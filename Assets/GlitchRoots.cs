using UnityEngine;
using System.Collections;
using InControl;

public class GlitchRoots : MonoBehaviour {
    public GameObject rootAssociated;
    public Shader glitchShader;
    public Shader plusShader;
    public RootsManager manager;
    public GameObject debris;

    private Shader rootShader;
    private Renderer rootRender;
    private Rigidbody[] rocksRigids;
    private Rigidbody rigidDebris;

    void Start()
    {
        rigidDebris = debris.GetComponent<Rigidbody>();
        rocksRigids = new Rigidbody[4];
        for (int i = 0; i < debris.transform.childCount; ++i)
        {
            rocksRigids[i] = debris.transform.GetChild(i).GetComponent<Rigidbody>();
        }
        rootRender = rootAssociated.GetComponent<Renderer>();
        rootShader = rootRender.material.shader;
    }

    void OnTriggerStay(Collider coll)
    {
        if (manager.isActivable && InputManager.ActiveDevice.Action4.IsPressed && coll.CompareTag("Player"))
        {
            rootRender.material.shader = glitchShader;
            StartCoroutine(DeactivateGlitch(2.0f));
            rigidDebris.isKinematic = false;
            rocksRigids[0].isKinematic = false;
            rocksRigids[1].isKinematic = false;
            rocksRigids[2].isKinematic = false;
            rocksRigids[3].isKinematic = false;
            manager.isActivable = false;
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
        rigidDebris.isKinematic = true;
        rocksRigids[0].isKinematic = true;
        rocksRigids[1].isKinematic = true;
        rocksRigids[2].isKinematic = true;
        rocksRigids[3].isKinematic = true;
    }
}
