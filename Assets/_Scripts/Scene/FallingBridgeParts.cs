using UnityEngine;
using System.Collections;

public class FallingBridgeParts : MonoBehaviour {

    public AudioClip glitch;
    public Rigidbody[] rigidbodyBridgeParts;
    private Rigidbody myRigidbody;

    public float timeToFall = 0.25f;

    public Renderer[] bridgeRenders;
    public Shader distorsionShader;

    void Start()
    {
        myRigidbody = transform.GetComponent<Rigidbody> ();
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            SoundManager.instance.PlaySingle(glitch);
            for (int i = 0; i < bridgeRenders.Length; ++i)
            {
                bridgeRenders[i].material.shader = distorsionShader;
            }
            Invoke("Fall", timeToFall);
        }
    }

    public void Fall()
    {
        for (int i = 0; i < rigidbodyBridgeParts.Length; ++i)
        {
            rigidbodyBridgeParts[i].isKinematic = false;
        }
        myRigidbody.isKinematic = false;
    }

}
