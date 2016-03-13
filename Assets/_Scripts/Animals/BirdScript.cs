using UnityEngine;
using System.Collections;

public class BirdScript : MonoBehaviour {

	public World world;
	Animator anim;
	public float timeRuning = 5.0f;
	private float slowFPSmoveX = 0.0f;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
		if (stateInfo.IsName("fly")) {
			anim.SetBool ("ColliderTouched", false);
			timeRuning -= Time.deltaTime;
			if (world.doUpdate) {
				Vector3 temp = transform.position;
				temp.x += 0.1f + slowFPSmoveX;
				temp.y += 0.1f + slowFPSmoveX;
				transform.position = temp;
				slowFPSmoveX = 0.0f;
			} else {
				slowFPSmoveX += 0.1f;
			}			
			anim.SetFloat ("TimeRuning", timeRuning); 
		}
	}

	void OnTriggerEnter(Collider other) {
		anim.SetBool ("ColliderTouched", true);
	}
}