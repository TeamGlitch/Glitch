using UnityEngine;
using System.Collections;

public enum foregroundDissapearState
{
	APPEARING,
	DISSAPPEARING,
	WAITING
};

public class ForegroundDissapear : MonoBehaviour {

	private Material material;
	private foregroundDissapearState state;

	// Use this for initialization
	void Start () {
		material = GetComponent<Renderer>().material;
		material.SetFloat("_Mode", 2);
		material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		material.SetInt("_ZWrite", 0);
		material.DisableKeyword("_ALPHATEST_ON");
		material.EnableKeyword("_ALPHABLEND_ON");
		material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		material.renderQueue = 3000;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (state == foregroundDissapearState.APPEARING) {

			Color color = material.color;
			color.a += 1.0f * Time.deltaTime;

			if (color.a > 1.0f) {
				color.a = 1.0f;
				state = foregroundDissapearState.WAITING;
			}

			material.SetColor("_Color", color);
			
		} else if (state == foregroundDissapearState.DISSAPPEARING) {

			Color color = material.color;
			color.a -= 1.0f * Time.deltaTime;

			if (color.a < 0.0f) {
				color.a = 0.0f;
				state = foregroundDissapearState.WAITING;
			}

			material.SetColor("_Color", color);

		}

	}

	void OnTriggerEnter(Collider coll){
		if (coll.gameObject.CompareTag("Player")) {

			state = foregroundDissapearState.DISSAPPEARING;

		}
	}

	void OnTriggerExit(Collider coll){
		if (coll.gameObject.CompareTag("Player") && coll.gameObject.transform.parent.GetComponent<PlayerController>().state != PlayerController.player_state.TELEPORTING) {

			state = foregroundDissapearState.APPEARING;

		}
	}

}
