using UnityEngine;
using System.Collections;

public class FallingBridgeParts : MonoBehaviour {

    public Rigidbody[] rigidbodyBridgeParts;

    public void OnTriggerEnter(Collider collider)
    {
        for(int i=0; i < rigidbodyBridgeParts.Length; ++i)
        {
            rigidbodyBridgeParts[i].isKinematic = false;
        }
    }

}
