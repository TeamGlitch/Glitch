using UnityEngine;
using System.Collections;

public class BoxCreatorUI : MonoBehaviour {

	public BoxCreatorUIMask[] masks;

	public void boxUsed(int index, float endTime){
		
		masks [index].gameObject.SetActive (true);
		masks [index].StartMovement(endTime);

		for (int i = 0; i < masks.Length; i++) {
			if (i != index && masks[i].gameObject.activeSelf)
				masks[i].endTime = endTime;
		}

	}

}
