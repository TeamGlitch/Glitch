using UnityEngine;
using System.Collections;

public class FallingBlock : MonoBehaviour {

    public World world;

    private Vector3 originalPosition;
    private float distanceTravelled = 0;
    public float fallingSpeed = 0.2f;
    public float maximumMovement = 50.0f;

	// Use this for initialization
	void Start () {
        originalPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (world.doUpdate)
        {
            if (distanceTravelled + (fallingSpeed * world.lag) > maximumMovement)
            {
                transform.position = originalPosition;
                distanceTravelled = 0;
            }
            else
            {
                transform.Translate(new Vector3(0, -fallingSpeed * world.lag, 0));
                distanceTravelled += fallingSpeed * world.lag;
            }
        }
	}
}
