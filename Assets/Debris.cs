using UnityEngine;
using System.Collections;

public class Debris : MonoBehaviour {
    private Rigidbody[] rocksRigids;
    private BreakableRock[] debrisScript;
    private int rand;

    void Awake()
    {
        rocksRigids = new Rigidbody[transform.childCount];
        debrisScript = new BreakableRock[transform.childCount];
        for (int i = 0; i < transform.childCount; ++i)
        {
            debrisScript[i] = transform.GetChild(i).GetComponent<BreakableRock>();
            rocksRigids[i] = transform.GetChild(i).GetComponent<Rigidbody>();
        }
    }

    public void Restart()
    {
        rocksRigids[0].isKinematic = true;
        rocksRigids[1].isKinematic = true;
        rocksRigids[2].isKinematic = true;
        rocksRigids[3].isKinematic = true;
        debrisScript[0].Restart();
        debrisScript[1].Restart();
        debrisScript[2].Restart();
        debrisScript[3].Restart();
    }

    public void Fall()
    {
        rocksRigids[0].isKinematic = false;
        rocksRigids[1].isKinematic = false;
        rocksRigids[2].isKinematic = false;
        rocksRigids[3].isKinematic = false;
    }

    public void Reubicate()
    {
        rand = Random.Range(0, 2);
        float z;
        if (rand == 0)
        {
            z = -0.125f;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
        }
        else
        {
            z = 0.2f;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
        }
        debrisScript[0].Reubicate(z);
        debrisScript[1].Reubicate(z);
        debrisScript[2].Reubicate(z);
        debrisScript[3].Reubicate(z);
    }
}
