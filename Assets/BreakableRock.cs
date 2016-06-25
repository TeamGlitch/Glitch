using UnityEngine;
using System.Collections;

public class BreakableRock : MonoBehaviour {

    public Rigidbody[] rigidBodies;
    public Transform[] transforms;
    public Rigidbody parent;
    public BoxCollider box;

    private Vector3[] initialTrans;
    private BoxCollider[] boxes;

    void Start()
    {
        initialTrans = new Vector3[transforms.Length];
        boxes = new BoxCollider[transforms.Length];

        for (int i = 0; i < transforms.Length; i++)
        {
            initialTrans[i] = transforms[i].position;
            boxes[i] = transform.GetChild(i).GetComponent<BoxCollider>();
        }
    }

    public void RestartPos()
    {
        for (int i = 0; i < transforms.Length; i++)
        {
            rigidBodies[i].isKinematic = true;
            parent.isKinematic = true;
            transforms[i].position = initialTrans[i];
            boxes[i].enabled = false;
        }
        box.enabled = true;
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.other.CompareTag("Player") || coll.other.CompareTag("Floor"))
        {
            box.enabled = false;
            for (int i = 0; i < rigidBodies.Length; i++)
            {
                rigidBodies[i].isKinematic = false;
                boxes[i].enabled = true;
            }
            Invoke("RestartPos", 5.0f);
        }
    }
}
