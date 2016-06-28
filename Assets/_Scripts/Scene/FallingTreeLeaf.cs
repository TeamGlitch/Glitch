using UnityEngine;
using System.Collections;

public class FallingTreeLeaf : MonoBehaviour {

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

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
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
                float alpha = Mathf.Lerp(1.0f, 0.0f, timeAfterFall / timeBeforeDisappear);
                branchRenderer.material.color = new Vector4(branchRenderer.material.color.r, branchRenderer.material.color.g, branchRenderer.material.color.b, alpha);
                leafRenderer.material.color = new Vector4(leafRenderer.material.color.r, leafRenderer.material.color.g, leafRenderer.material.color.b, alpha);
                rb.isKinematic = false;
                if (timeAfterFall > timeBeforeDisappear)
                {
                    gameObject.SetActive(false);
                }
            }
		}
	}
	
	void OnTriggerEnter(Collider collider) {
		if (collider.CompareTag("Player")) {
			colliderTouched = true;
            branchRenderer.material.shader = distorsionShader;
            leafRenderer.material.shader = distorsionShader;
		}
	}

}
