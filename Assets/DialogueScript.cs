using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueScript : MonoBehaviour {

	//Dialogue Box Writing
	public Text dialogueBoxText;				//The dialogue box text reference
	public float waitBetweenLetters = 0.05f;	//How much time there is between letter and letter printing

	private bool printing = false;				//Currently printing in the dialogue box
	private string messageToPrint;				//Message to print in the dialogue box
	private int printLetter = 0;				//Index of the currently printing letter
	private float nextLetterTime = 0f;			//Time when the next letter will be printed

	// Use this for initialization
	void Start () {
		messageToPrint = "Pack my box with five dozen liquor jugs. The five boxing wizards jump quickly. How vexingly quick daft zebras jump!";
		printing = true;
	}
	
	// Update is called once per frame
	void Update () {

		//We check if we're printing something and we can print the next letter
		if (printing && Time.time > nextLetterTime) {

			//Go to the next letter
			printLetter++;

			if(printLetter >= messageToPrint.Length){

				//If the end has been reached, put the message directly and restore variables
				dialogueBoxText.text = messageToPrint;
				printing = false;
				printLetter = 0;

			} else {

				//If not, we send the message to print with color tags that make it invisible from the print
				//letter to the end. We do this rather than doing subscripts because they mess the paragraph aligment
				string newMessage = messageToPrint.Insert(printLetter, "<color=#00000000>");
				newMessage = newMessage.Insert(newMessage.Length, "</color>");
				dialogueBoxText.text = newMessage;

				//Finally we determine when the next letter will appear
				nextLetterTime = Time.time + waitBetweenLetters;

			}




		}
	}


}
