using UnityEngine;
using System.Collections;

public class ArrowScript : MonoBehaviour {

    public Vector3 direction;

    void OnCollisionEnter(Collision coll)
    {
        gameObject.SetActive(false);
    }
	
	void Update () 
    {
        transform.Translate(direction * Time.deltaTime*6);
	}
}
