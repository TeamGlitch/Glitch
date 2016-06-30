using UnityEngine;
using System.Collections;

public class FallingTreeLeaf : MonoBehaviour {

    public AudioClip glitch;
	public float timeBeforeFall = 1.0f;
    public float timeBeforeDisappear = 1.0f;
    public Player playerScript;
    public Renderer branchRenderer;
    public Renderer leafRenderer;
    public Shader distorsionShader;
    public Shader transparentShader;
    public Material blackMaterial;
	
    private float timeSinceColliderTouched = 0.0f;
    private float timeAfterFall = 0.0f;
	private Rigidbody rb;
	private bool colliderTouched;

    private Vector3 initPos;
    private Vector3 initRot;

    private Shader leafInitShader;
    private Shader brachInitShader;
    private Material branchInitMaterial;
    private Material leafInitMaterial;

    // Use this for initialization
    void Start () {
		rb = GetComponent<Rigidbody> ();
        initPos = transform.position;
        initRot = transform.eulerAngles;
        leafInitShader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
        brachInitShader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
        leafInitMaterial = leafRenderer.material;
        branchInitMaterial = branchRenderer.material;
    }
    void Update() {
        if (colliderTouched)
        {
            timeSinceColliderTouched += Time.deltaTime;
            if (timeSinceColliderTouched > timeBeforeFall)
            {
                branchRenderer.material = blackMaterial;
                leafRenderer.material = blackMaterial;
                branchRenderer.material.shader = transparentShader;
                leafRenderer.material.shader = transparentShader;
                timeAfterFall += Time.deltaTime;
                rb.isKinematic = false;
                float alpha = Mathf.Lerp(1.0f, 0.0f, timeAfterFall / timeBeforeDisappear);
                branchRenderer.material.color = new Vector4(branchRenderer.material.color.r, branchRenderer.material.color.g, branchRenderer.material.color.b, alpha);
                leafRenderer.material.color = new Vector4(leafRenderer.material.color.r, leafRenderer.material.color.g, leafRenderer.material.color.b, alpha);
                if (timeAfterFall > timeBeforeDisappear)
                {
                    gameObject.SetActive(false);
                }
            }
		}
	}
	
    public void Reset()
    {
        transform.position = initPos;
        transform.eulerAngles = initRot;
        leafRenderer.material = leafInitMaterial;
        branchRenderer.material = branchInitMaterial;
        leafRenderer.material.shader = leafInitShader;
        branchRenderer.material.shader = brachInitShader;
        leafRenderer.material.color = new Vector4(1f, 1f, 1f, 1f);
        branchRenderer.material.color = new Vector4(1f, 1f, 1f, 1f);
        colliderTouched = false;
        timeSinceColliderTouched = 0.0f;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void OnTriggerEnter(Collider collider) {
		if (collider.CompareTag("Player")) {
            SoundManager.instance.PlaySingle(glitch);
			colliderTouched = true;
            timeSinceColliderTouched = 0.0f;
            branchRenderer.material.shader = distorsionShader;
            leafRenderer.material.shader = distorsionShader;
        }
    }

}
