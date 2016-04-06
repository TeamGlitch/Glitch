using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using InControl;
using System.Xml;

public class DialogueScript : MonoBehaviour {

	////////////////////////Dialogue Box////////////////////////

	public enum dialogueBoxState
	{
		OFF,
		PREPARE_TEXT,
		WRITTING,
		WAITING
	};

	private string[] randomLetters = new string[] {
		"•","|","£","Ø","ƒ","º","¿","®","¡","»","µ","±",
		"¶","÷","ª","ø","†","‡","‘","—","™","œ","©","€",
		"¥","~","!","$","%","&","/","(",")","=","?","@",
		"#","~","€","☺","☻","♥","♦","♣","♠","▲","✓","☀",
		"★","☂","♞","☯","☭","☢","☎","❄"
	};

	//Level Dialogue XML
	public TextAsset XMLAsset;
	private XmlDocument xmlDoc;

	//State
	private dialogueBoxState state;

	//References
	public GameObject dialogueBox;				//The dialogue box reference
	public Text dialogueBoxText;				//The dialogue box text reference
	public GameObject continueButton;			//The continue button reference

	//Message variables
	private List<string> messageList = new List<string>();  //Messages to print in the dialogue box  
	private string messageToPrint;							//Message being printed actually
	private string cleanMessage;                            //Message without tags

	//Writting variables
	private int letterIndex = 0;                //Index of the currently printing letter
	private int cleanIndex = 0;                    //Index of the currently printing letter in the clean message
	private float nextLetterTime = 0f;			//Time when the next letter will be printed

	//Bug effect on writting pointer
	private bool showPointerBug = true;
	private int currentRandomLetter = 0;				//Current bugged letter in the writting effect
	private float waitBetweenBuggedLetters = 0.02f;		//Wait between bugged letter changes
	private float nextBuggedLetterTime = 0f;			//When the next bugged letter will appear

	//Bug effect on waiting state
	private int corruptedPosition = -1;			//What index letter is corrupted
	private char originalChar;					//What original character was in it
	private float timeToSolve = 0;				//When the corruption will be solved

	//Writting properties
	private float defaultWaitBetweenLetters = 0.03f;
	private float waitBetweenLetters = 0.03f;	//How much time there is between letter and letter printing
	private bool autoJump = false;				//The current text goes to the next text automatically
	private bool skipable = true;				//The user can skip this text

	// Use this for initialization
	void Start () {

		//Read the XML document
		xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XMLAsset.text);

		//Make initial preparations
		state = dialogueBoxState.OFF;
		dialogueBox.SetActive(false);
		continueButton.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

		switch (state) {

			case dialogueBoxState.PREPARE_TEXT: //Preparing the message to write

				//Sets the message to print to the first message in the list
				messageToPrint = messageList[0];

				//Cleans the tags and stores it as the clean message
				int i = 0;
				cleanMessage = "";
				while (i < messageToPrint.Length) {
					if (messageToPrint[i] == '[') {
						while (i < messageToPrint.Length && messageToPrint[i] != ']') {
							i++;
						}
						if (messageToPrint[i] == ']') {
							i++;
						}
					} else {
						cleanMessage += messageToPrint[i];
						i++;
					}
				}

				//Sets the properties to default and starts writting
				letterIndex = -1;
				cleanIndex = -1;

				autoJump = false;
				waitBetweenLetters = defaultWaitBetweenLetters;
				skipable = true;

				state = dialogueBoxState.WRITTING;

			break;

			case dialogueBoxState.WRITTING: //Currently writting in the dialogue box

				//If A is pressed, skip message
				if (InputManager.ActiveDevice.Action1.WasPressed && skipable) {
					letterIndex  = messageToPrint.Length;
					nextLetterTime = 0;
				}
				
				//If we can print the next letter
				if (Time.time > nextLetterTime) {

					//Go to the next letter
					letterIndex++;
					cleanIndex++;

					//If the end has been reached
					if (letterIndex >= messageToPrint.Length) {

						//Put the message directly
						dialogueBoxText.text = cleanMessage;

						//Go to wait, remove the message from the list and activate the continue button
						state = dialogueBoxState.WAITING;
						messageList.RemoveAt(0);
						if (!autoJump) {
							continueButton.SetActive(true);
						}

					} else {

						//If not, we determine when the next letter will appear
						nextLetterTime = Time.time + waitBetweenLetters;
						
						//If the pointer is not visible, show it
						showPointerBug = true;

						//Read all the tags in this position (if there's any)
						while (letterIndex < messageToPrint.Length && messageToPrint[letterIndex] == '[') {
							ReadTag();
						}

						//We send the message to print with color tags that make it invisible from the print
						//letter to the end. We do this rather than doing subscripts because they mess the paragraph aligment
						string pointer = "";
						if (showPointerBug) {
							pointer += randomLetters[currentRandomLetter];
						}
						dialogueBoxText.text = cleanMessage.Insert(cleanIndex, pointer + "<color=#00000000>") + "</color>";

					}

				} else if (Time.time > nextBuggedLetterTime) {

					//We change the writting pointer's currently showed sign and we determine the next sign change
					if(showPointerBug){
						currentRandomLetter = Random.Range (0, randomLetters.Length);
						dialogueBoxText.text = cleanMessage.Insert(cleanIndex, randomLetters[currentRandomLetter] + "<color=#00000000>") + "</color>";
						nextBuggedLetterTime = Time.time + waitBetweenBuggedLetters;
					}
				}
					
			break;

			case dialogueBoxState.WAITING: //Message on dialogue box. Waiting for next instruction

				//The wait ends
				if (InputManager.ActiveDevice.Action1.WasPressed || autoJump) {

					corruptedPosition = -1;
					continueButton.SetActive(false);

					//If there are more text, we return to the prepare_text state
					if (messageList.Count > 0) {
						state = dialogueBoxState.PREPARE_TEXT;
					} else {
						dialogueBox.SetActive(false);
					}

				} else {
				
					//If there is no corruption on the message
					if (corruptedPosition == -1) {

						//0.75% chance to corrupt a char
						if(Random.value > 0.9925f){

							//Selects a position, stores it's value and sets a duration
							corruptedPosition = Random.Range(0, dialogueBoxText.text.Length);
							originalChar = dialogueBoxText.text[corruptedPosition];
							timeToSolve = Time.time + Random.Range(0.05f, 0.4f);

							//Changes the position to a random symbol
							string newText = dialogueBoxText.text.Remove(corruptedPosition, 1);
							newText = newText.Insert(corruptedPosition, randomLetters[Random.Range(0, randomLetters.Length)]);
							dialogueBoxText.text = newText;
						}

					}
					//If there is corruption and its duration has ended
					else if (Time.time > timeToSolve){

						//Fixes the corrupted char
						string newText = dialogueBoxText.text.Remove(corruptedPosition, 1);
						newText = newText.Insert(corruptedPosition, originalChar.ToString());
						dialogueBoxText.text = newText;
						corruptedPosition = -1;

					}
				}

			break;

		}
	}

	private void ReadTag(){

		string tag = "";		//The tag name
		string value = "";		//The tag value
		bool tagging = true;	//Where writting the tag? Else, we're writting the value
		letterIndex++;

		//Do until the ']' or the message end
		while (letterIndex < messageToPrint.Length && messageToPrint[letterIndex] != ']') {

			//If it's the =, we're now writting the value
			if (messageToPrint[letterIndex] == '=') {
				tagging = false;
			} else {
				
				//Store the value where needed
				if (tagging) {
					tag += messageToPrint[letterIndex];
				} else {
					value += messageToPrint[letterIndex];
				}
			}
			letterIndex++;
		}

		//If it ended with ']' (not unclosed tag), search in the effects
		if (messageToPrint[letterIndex] == ']') {

			letterIndex++;

			//Text speed
			if (tag == "speed") {
				waitBetweenLetters = float.Parse(value);
				return;
			}
			if (tag == "wait") {
				nextLetterTime = Time.time + float.Parse(value);
				showPointerBug = false;
				return;
			}
			if (tag == "skip") {
				autoJump = true;
				return;
			}
			if (tag == "noskip") {
				skipable = false;
				return;
			}
		}
	}

	public void callScene(int sceneNum){

		//We read the scene text of the given id
		XmlNode scene = xmlDoc.SelectSingleNode("/Dialogue[@lang = \"English\"]/Scene[@id = \"" + sceneNum + "\"]");

		//We add the lines to the message list 
		for (int i = 0; i < scene.ChildNodes.Count; i++) {
			messageList.Add (scene.ChildNodes[i].InnerText);
		}

		//We prepare the text
		dialogueBox.SetActive(true);
		state = dialogueBoxState.PREPARE_TEXT;
	}
}
