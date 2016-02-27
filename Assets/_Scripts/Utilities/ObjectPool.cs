using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour {

	private int maxSize;				//Max size of the array. If it's -1, its an auto-extensible array
	private List<GameObject> buffer;	//Elements list
	private GameObject prefab;

	public ObjectPool(GameObject objectPrefab, int poolSize = -1 )
	{
		buffer = new List<GameObject>();
		maxSize = poolSize;
		prefab = objectPrefab;
	}

	//Returns the first inactive object or creates a new one
	//if there are none.
	//Returns null if there aren't inactive objects and
	//can't add more because the buffer is at max size
	public GameObject getObject (){

		GameObject returnedObject = null;
		for (int i = 0; i < buffer.Count; i++) {
			if (buffer[i].activeSelf == false) {
				returnedObject = buffer[i];
				returnedObject.SetActive(true);
				break;
			}
		}

		if (returnedObject == null && (maxSize == -1 || buffer.Count + 1 < maxSize)) {
			returnedObject = (GameObject) Instantiate (prefab);
			buffer.Add (returnedObject);
		}

		return returnedObject;
	}

	//Clears the pool
	public void ClearPool(){
		for (int i = buffer.Count - 1; i > 0; i--) {
			GameObject obj = buffer[i];
			buffer.RemoveAt(i);
			Destroy(obj);
		}
	}

	//////////////////Getters//////////////////

	//Actual size of the buffer
	public int getActualSize(){
		return buffer.Count;
	
	}

	//Buffer active members
	public int getActiveMembers(){
		
		int number = 0;
		for (int i = 0; i < buffer.Count; i++) {
			if (buffer[i].activeSelf == true) {
				number++;
			}
		}
		return number;

	}

	//Buffer inactive members
	public int getInactiveMembers(){

		int number = 0;
		for (int i = 0; i < buffer.Count; i++) {
			if (buffer[i].activeSelf == false) {
				number++;
			}
		}
		return number;

	}

	//Buffer limitation
	public int getMaxSize(){
		return maxSize;
	}

}
