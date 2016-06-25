﻿using UnityEngine;
using System.Collections;

public class BreakableRock : MonoBehaviour {

    public Rigidbody[] rigidBodies;
    public Transform[] transforms;
    public BoxCollider box;

    private Vector3 initPosition;
    private Quaternion initRot;
    private Vector3[] initialTrans;
    private Quaternion[] initialRot;
    private BoxCollider[] boxes;

    void Start()
    {
        initPosition = transform.position;
        initRot = transform.rotation;
        initialTrans = new Vector3[transforms.Length];
        initialRot = new Quaternion[transforms.Length];
        boxes = new BoxCollider[transforms.Length];

        for (int i = 0; i < transforms.Length; i++)
        {
            initialTrans[i] = transforms[i].localPosition;
            initialRot[i] = transforms[i].localRotation;
            boxes[i] = transform.GetChild(i).GetComponent<BoxCollider>();
        }
    }

    public void Restart()
    {
        for (int i = 0; i < transforms.Length; i++)
        {
            rigidBodies[i].isKinematic = true;
            transforms[i].localPosition = initialTrans[i];
            transforms[i].localRotation = initialRot[i];
            boxes[i].enabled = false;
        }
        transform.position = initPosition;
        transform.rotation = initRot;
        box.enabled = true;
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("Player") || coll.collider.CompareTag("Floor"))
        {
            box.enabled = false;
            for (int i = 0; i < rigidBodies.Length; i++)
            {
                rigidBodies[i].isKinematic = false;
                boxes[i].enabled = true;
            }
        }
    }
}
