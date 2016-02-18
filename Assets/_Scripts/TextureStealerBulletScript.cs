using UnityEngine;
using System.Collections;

public class TextureStealerBulletScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}

	/*
	
	// Update is called once per frame
	void Update () {
		this.transform.position += speed;
	}

	void OnTriggerEnter(Collider col) {

		Transform parentColisionado = col.GetComponent<Collider> ().transform.parent;

		if (parentColisionado != null && parentColisionado.name == "World") {
			this.transform.parent.GetComponent<TextureStealerScript> ().paint = col.GetComponent<Collider> ().GetComponent<TerrainScript> ().tipo;
			Debug.Log (this.transform.parent.GetComponent<TextureStealerScript> ().paint);
		}

		Destroy (gameObject);
	}
	*/
}
