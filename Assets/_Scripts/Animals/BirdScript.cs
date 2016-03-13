using UnityEngine;
using System.Collections;

public class BirdScript : MonoBehaviour {

	Animator anim;
	public float timeRuning = 5.0f;

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
			Vector3 temp = transform.position;
			temp.x += 0.1f;
			temp.y += 0.1f;
			transform.position = temp;
			anim.SetFloat ("TimeRuning", timeRuning); 
		}
	}

	void OnTriggerEnter(Collider other) {
		anim.SetBool ("ColliderTouched", true);
	}
}