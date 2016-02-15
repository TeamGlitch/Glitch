using UnityEngine;
using System.Collections;

public class FlickeringTexture : MonoBehaviour {

	public Material bugMaterial;
	public float baseStableTime = 1.3f;		//Time the original texture is on between flickerings
	public float baseFlickeringTime = 0.2f;	//How much time a flickering takes
	public float deviation = 0.3f;      //Percentage of time deviation
	public int minRepetitions = 1;		//Minimum and maximum number of consecutive activations
	public int maxRepetitions = 4;		//and desactivations of a texture during a flickering

	private static Material texture;
	private float operationBegin = 0;
	private bool flickering = false;
	private int repetitions;
	private int repetitions_done;
	private float duration = 0;

	// Update is called once per frame
	void Update () {
		if (flickering == false) {

			//If it's not flickering, wait until there's no more stability time left
			if (Time.time >= operationBegin + duration) {

				//Start flickering
				flickering = true;

				//Choose a number of repetitions to do and calcule their speed
				//(a repetitions its is bug-unbug cycle, thus * 2)
				repetitions = UnityEngine.Random.Range(minRepetitions, maxRepetitions) * 2;
				duration = (baseFlickeringTime*Random.Range(1-deviation, 1+deviation))/repetitions;

				//Set the repetitions done to 0, start a new flickering phase and
				//restart the stable counter
				repetitions_done = 0;
				operationBegin = 0;

			}
		} else {

			//Decrease the flickering phase time left
			if(Time.time >= operationBegin + duration){
				//If the flickering pahse is over, increase the repetitions done by 1
				repetitions_done++;

				//If the module of the phase by 2 is 0 (even) the texture shown is original one - unbugged -,
				//if not (even) the texture shown is the bugged one - bugged -
				if(repetitions_done % 2 == 0){
					this.GetComponent<MeshRenderer>().material = texture;
				} else {
					texture = this.GetComponent<MeshRenderer>().material;
					this.GetComponent<MeshRenderer>().material = bugMaterial;
				}

				//If there has been enough repetitions, end the flickering and go to a stable time
				//else, go to the next flickering phase
				if(repetitions_done == repetitions){
					flickering = false;
					duration = baseStableTime*Random.Range(1-deviation, 1+deviation);
				}
				operationBegin = Time.time;
			}
		}
	}
}
