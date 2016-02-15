using UnityEngine;
using System.Collections;

public class FlickeringTexture : MonoBehaviour {

	public Material bugMaterial;
	public float baseStableTime = 1.3f;		//Time the original texture is on between flickerings
	public float baseFlickeringTime = 0.4f;	//How much time a flickering takes
	public float deviation = 0.3f;      //Percentage of time deviation
	public int minRepetitions = 1;		//Minimum and maximum number of consecutive activations
	public int maxRepetitions = 4;		//and desactivations of a texture during a flickering

	private static Material texture;
	private bool flickering = false;
	private int repetitions;
	private int repetitionsDone;
	private float repetitionTime;
	private float phaseEnd = 0;

	// Update is called once per frame
	void Update () {
		if (flickering == false) {

			//If it's not flickering, wait until the end of the stability time
			if (Time.time >= phaseEnd) {

				//Choose a number of repetitions to do and calcule their time
				//(a repetitions its is bug-unbug cycle, thus * 2)
				repetitions = UnityEngine.Random.Range(minRepetitions, maxRepetitions) * 2;
				repetitionTime = (baseFlickeringTime*Random.Range(1-deviation, 1+deviation))/repetitions;
				phaseEnd = Time.time + repetitionTime;

				//Set the repetitions done to 0 and start flickering
				repetitionsDone = 0;
				flickering = true;

			}
		} else {

			//Wait until the flickering phase end
			if(Time.time >= phaseEnd){

				//If the flickering phase is over, increase the repetitions done by 1
				repetitionsDone++;

				//If the module of the phase by 2 is 0 (even) the texture shown is original one - unbugged -,
				//if not (even) the texture shown is the bugged one - bugged -
				if(repetitionsDone % 2 == 0){
					this.GetComponent<MeshRenderer>().material = texture;
				} else {
					texture = this.GetComponent<MeshRenderer>().material;
					this.GetComponent<MeshRenderer>().material = bugMaterial;
				}

				//If there has been enough repetitions, end the flickering and go to a stable time
				//else, go to the next flickering phase
				if(repetitionsDone == repetitions){
					flickering = false;
					phaseEnd = Time.time + (baseStableTime*Random.Range(1-deviation, 1+deviation));
				} else {
					phaseEnd = Time.time + repetitionTime;
				}
			}
		}
	}
}
