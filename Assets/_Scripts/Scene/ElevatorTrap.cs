using UnityEngine;
using System.Collections;

public class ElevatorTrap : MonoBehaviour {

    public GameObject parentCubes;
    public World world;
    public bool impact;

    private GameObject[] cubes;
    private int aux = 0;
    private int childrenCount;

	void Start () {
	    childrenCount = parentCubes.transform.childCount;
        cubes = new GameObject[childrenCount];
        for (int i = 0; i < childrenCount; ++i)
        {
            cubes[i] = parentCubes.transform.GetChild(i).gameObject;
        }
        impact = true;
	}
	
	void Update () {
        if (impact && (childrenCount > aux))
        {
            cubes[aux].SetActive(true);
            impact = false;
            ++aux;
        }
        
	}
}
