using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using InControl;

[RequireComponent (typeof (AudioSource))]

public class IntroScript : MonoBehaviour {

	public MovieTexture movie;
	private AudioSource audio;
	private float timeToEnd;

	void Start(){
		audio = GetComponent<AudioSource>();

		GetComponent<Renderer>().material.mainTexture = movie as MovieTexture;
		audio.clip = movie.audioClip;
		movie.Play();
		audio.Play();

		timeToEnd = Time.time + movie.duration;
	}

	// Update is called once per frame
	void Update () {

		if (Time.time > timeToEnd || InputManager.ActiveDevice.AnyButton.WasPressed) {
			SceneManager.LoadScene ("menu");
		} else if (Camera.current == Camera.main) {
			float height = Camera.current.orthographicSize * 2;
			float width = Camera.current.aspect * height;
			transform.localScale = new Vector3 (width, height, 0.1f);
		}


	}
}
