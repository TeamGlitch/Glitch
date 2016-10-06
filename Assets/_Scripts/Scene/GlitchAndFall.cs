using UnityEngine;
using System.Collections;

public class GlitchAndFall : MonoBehaviour {

    public AudioClip glitch;
    public float timeBeforeFall = 1.0f;
    public float timeBeforeDisappear = 1.0f;
    public Player playerScript;
    public Renderer structureRenderer;
    public Shader distorsionShader;
    public Shader transparentShader;
    public Material blackMaterial;

    private float timeSinceColliderTouched = 0.0f;
    private float timeAfterFall = 0.0f;
    private Rigidbody rb;
    private bool colliderTouched;

    private Vector3 initPos;
    private Vector3 initRot;

    private Shader structureInitShader;
    private Material structureInitMaterial;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initPos = transform.position;
        initRot = transform.eulerAngles;
        structureInitShader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
        structureInitMaterial = structureRenderer.material;
    }
    void Update()
    {
        if (colliderTouched)
        {
            timeSinceColliderTouched += Time.deltaTime;
            if (timeSinceColliderTouched > timeBeforeFall)
            {
                structureRenderer.material = blackMaterial;
                structureRenderer.material.shader = transparentShader;
                timeAfterFall += Time.deltaTime;
                rb.isKinematic = false;
                float alpha = Mathf.Lerp(1.0f, 0.0f, timeAfterFall / timeBeforeDisappear);
                structureRenderer.material.color = new Vector4(structureRenderer.material.color.r, structureRenderer.material.color.g, structureRenderer.material.color.b, alpha);
                if (timeAfterFall > timeBeforeDisappear)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("Player"))
        {
            SoundManager.instance.PlaySingle(glitch);
            colliderTouched = true;
            timeSinceColliderTouched = 0.0f;
            structureRenderer.material.shader = distorsionShader;
        }
    }
}
