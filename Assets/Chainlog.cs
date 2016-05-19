using UnityEngine;
using System.Collections;

public class Chainlog : MonoBehaviour {

	public GameObject chainFragment;
	private Transform[] chains;

	private float endX;
	public float speed;

	// Use this for initialization
	void Start () {
		Vector3 size = transform.GetChild(1).GetChild(0).gameObject.GetComponent<Renderer>().bounds.size;
		endX = transform.GetChild(0).localPosition.x;
		Destroy (transform.GetChild(0).gameObject);

		int number = Mathf.FloorToInt(endX / size.x);

		chains = new Transform[number];
		chains[0] = transform.GetChild(1);

		for (int i = 1; i < number; i++) {
			GameObject go = GameObject.Instantiate(chainFragment);
			go.transform.parent = transform;
			go.transform.localPosition = new Vector3 (i * size.x, 0, 0);
			chains[i] = go.transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < chains.Length; i++) {
			chains[i].Translate (new Vector3(speed * Time.deltaTime,0,0));
			if (chains [i].localPosition.x > endX) {
				chains [i].Translate (new Vector3 (-endX, 0, 0));
			}
		}
	}
}
