using UnityEngine;
using System.Collections;

public class BoxCreatorUI : MonoBehaviour {

	public BoxCreatorUIMask[] masks;

	public void boxUsed(int i, float endTime){
		masks [i].gameObject.SetActive (true);
		masks [i].StartMovement(endTime);
	}

}
