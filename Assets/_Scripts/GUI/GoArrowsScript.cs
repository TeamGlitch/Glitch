using UnityEngine;
using System.Collections;

public class GoArrowsScript : MonoBehaviour {

    private Canvas canvas;

    private GameObject arrowUP;
    private GameObject arrowRIGHT;
    private GameObject arrowDOWN;

    private float timeEnd;

	void Start () {

        canvas = GetComponent<Canvas>();

        arrowUP = transform.GetChild(0).gameObject;
        arrowRIGHT = transform.GetChild(1).gameObject;
        arrowDOWN = transform.GetChild(2).gameObject;

	}
	
	void Update () {

        if (canvas.enabled){
            if (Time.time >= timeEnd)
            {
                canvas.enabled = false;
            }
        }

	}

    public void activate(Vector3 target1, Vector3 target2){

        arrowUP.SetActive(false);
        arrowRIGHT.SetActive(false);
        arrowDOWN.SetActive(false);

        activateArrow(target1);
        activateArrow(target2);

        timeEnd = Time.time + 3f;
        canvas.enabled = true;
    }

    private void activateArrow(Vector3 direction){
        if (direction == Vector3.up)
            arrowUP.SetActive(true);
        else if (direction == Vector3.right)
            arrowRIGHT.SetActive(true);
        else if (direction == Vector3.down)
            arrowDOWN.SetActive(true);
    }
}
