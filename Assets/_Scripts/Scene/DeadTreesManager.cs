using UnityEngine;
using System.Collections;

public class DeadTreesManager : MonoBehaviour
{

    public FallingTreeLeaf[] fallingTreeVector;
    public Player player;

    // Use this for initialization
    void Start()
    {
        player.PlayerReviveEvent += ResetBranches;
    }

    // Update is called once per frame
    public void ResetBranches()
    {
        for (int i = 0; i < fallingTreeVector.Length; ++i)
        {
            if (!fallingTreeVector[i].gameObject.activeSelf)
            {
                fallingTreeVector[i].gameObject.SetActive(true);
            }
            fallingTreeVector[i].Reset();
        }
    }
}