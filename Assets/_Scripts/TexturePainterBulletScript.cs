using UnityEngine;
using System.Collections;

public class TexturePainterBulletScript : MonoBehaviour {

	//public terrainType type;

	// Use this for initialization
	void Start () {
	
	}

	/*
	public void Paint(){
		Material newMat;

		if (type == terrainType.Icy) {
			newMat = Resources.Load ("Icy", typeof(Material)) as Material;
		} else if (type == terrainType.Slimy) {
			newMat = Resources.Load ("Slimy", typeof(Material)) as Material;
		} else {
			newMat = Resources.Load("Normal", typeof(Material)) as Material;
		}

		GetComponent<Renderer>().sharedMaterial = newMat;
	}

	void OnTriggerEnter(Collider col) {

		Transform parentColisionado = col.GetComponent<Collider> ().transform.parent;

		if (parentColisionado != null && parentColisionado.name == "World") {
			col.GetComponent<Collider> ().GetComponent<TerrainScript> ().setType (type);
		}

		Destroy (gameObject);
	}
	*/
}
