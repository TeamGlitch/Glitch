using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
    public bool doUpdate = true;
	public float slowDown = 1;

	public void toggleSlowFPS (){
		if (slowDown == 1) {
			slowDown = 0.5f;
		} else {
			slowDown = 1.0f;
		}
	}
}
