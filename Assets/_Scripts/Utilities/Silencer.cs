using UnityEngine;
using System.Collections;

public class Silencer : MonoBehaviour {

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            SoundManager.instance.StartFadeOut(10f);
            GameObject.Destroy(this.gameObject);
        }
    }
}
