using UnityEngine;
using System.Collections;

public class SoldierExplosion : MonoBehaviour {

	private Rigidbody[] _rigidbodies;

	// Use this for initialization
	void Awake ()
	{
		_rigidbodies = new Rigidbody[transform.childCount];
		for (int i = 0; i < _rigidbodies.Length; ++i)
		{
			_rigidbodies[i] = transform.GetChild(i).GetComponent<Rigidbody>();
		}
	}
	
	public void Explosion ()
	{
		for (int i = 0; i < _rigidbodies.Length; ++i) {
			float randomForce = Random.Range (500.0f, 1000.0f);
			float randomAngle = Random.Range(0f,180f);
			
			int random = Random.Range (1, 3);
			if (random == 1)
				randomForce = -randomForce;

			_rigidbodies[i].AddForce(randomForce * Mathf.Cos(randomAngle), randomForce * Mathf.Sin(randomAngle), 0.0f);

			float anglularVelocityX = 10.0f;
			float anglularVelocityY = 10.0f;
			random = Random.Range (1, 3);
			if (random == 1)
				anglularVelocityX = -anglularVelocityX;
			random = Random.Range (1, 3);
			if (random == 1)
				anglularVelocityY = -anglularVelocityY;


			_rigidbodies[i].angularVelocity = new Vector3(anglularVelocityX,anglularVelocityY,0f);
		}
	}

}
