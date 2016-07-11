using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ToFinalScene : MonoBehaviour
{

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            //TODO: NEEEDS TO USE LOADER
            SceneManager.LoadScene("Boss Stage");
        }
    }
}
