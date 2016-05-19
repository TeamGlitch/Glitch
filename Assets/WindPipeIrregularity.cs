using UnityEngine;
using System.Collections;

public class WindPipeIrregularity : MonoBehaviour {

	public float slowTime;
	public float fastTime;
	public float slowIntensity;
	public float fastIntensity;

	private bool slow;
	private float change = 0;

	private WindPipe windpipe;

	// Use this for initialization
	void Start () {
		windpipe = GetComponent<WindPipe>();		
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > change) {
			if (slow) {
				slow = false;
				change = Time.time + fastTime;
				windpipe.windSpeed = fastIntensity;
			} else {
				slow = true;
				change = Time.time + slowTime;
				windpipe.windSpeed = slowIntensity;
			}

		}
	}
}
