using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using InControl;
using System.Xml;
using InControl;

public class DialogueScript : MonoBehaviour {

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
    
    //Resolution
    private float currentResolution;

	//State
	public dialogueBoxState state;

	//External References
	public PlayerController player;						//The player reference
    public AdvanceBarEnemies advanceBarEnemies = null;  //The enemies advance bar

	//Internal References
	public GameObject dialogueBox;						//The dialogue box reference
	private Image face;									//The face reference
	private Text dialogueBoxText;						//The dialogue box text reference
	private RectTransform dialogueBoxTextRectTransform;	//The RecTransform of the dialogueBoxText
	private Image background;							//The dialogue box background
	private GameObject continueButtonController;		//The continue button for controllers reference
    private GameObject continueButtonKeyboard;          //The continue button for keyboards reference
	private AudioSource audio;							//The audio source reference

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

	//Face animation
	private DialogueCharacterDB dialogueCharacterDB;				//Class that stores character information
	public List<Sprite> faceAnimationSprites = new List<Sprite>();	//The face animation sprites
	private int animationIndex = 0;									//The current sprite index. If it's -1, there's no face
	private float nextAnimationStep = 0f;							//When to go to the next sprite
	private float faceAnimationSpeed = 0.35f;						//Time between sprites

    //Tutorial arrows
    public GameObject[] tutoArrows;

	// Use this for initialization
	void Start () {

		//Read the XML document
		xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XMLAsset.text);

		//Set references
		Transform content = dialogueBox.transform.FindChild("Content");

		dialogueBoxText = content.FindChild("Text").gameObject.GetComponent<Text>();
		dialogueBoxTextRectTransform = dialogueBoxText.gameObject.GetComponent<RectTransform>();
		face = content.FindChild("Head").gameObject.GetComponent<Image>();

		audio = gameObject.GetComponent<AudioSource>();
		background = dialogueBox.transform.FindChild("background").gameObject.GetComponent<Image>();
		continueButtonController = dialogueBox.transform.FindChild("ContinueButtonController").gameObject;
        continueButtonKeyboard = dialogueBox.transform.FindChild("ContinueButtonKeyboard").gameObject;

		//Initialize variables
		dialogueCharacterDB = new DialogueCharacterDB();

		//Make initial preparations
		state = dialogueBoxState.OFF;
		dialogueBox.SetActive(false);
		continueButtonController.SetActive(false);
        continueButtonKeyboard.SetActive(false);

        correctToResolution();
	}
	
	// Update is called once per frame
	void Update () {

        if (state != dialogueBoxState.OFF)
        {
            if(Camera.current != null && Camera.current.aspect != currentResolution)
                correctToResolution();
            if (audio.volume != SoundManager.instance.getSoundVolume())
                audio.volume = SoundManager.instance.getSoundVolume();
        }

		switch (state) {

			case dialogueBoxState.PREPARE_TEXT: //Preparing the message to write

				//Sets the message to print to the first message in the list
				messageToPrint = messageList[0];

				//Cleans the tags and stores it as the clean message
				letterIndex = 0;
				cleanMessage = "";
				while (letterIndex < messageToPrint.Length) {
                    if (messageToPrint[letterIndex] == '[')
                    {
                        preRead();
					} else {
                        cleanMessage += messageToPrint[letterIndex];
					}
                    letterIndex++;
				}

				//Sets the properties to default and starts writting
				letterIndex = -1;
				cleanIndex = -1;
				if(animationIndex != -1) animationIndex = 0;

				autoJump = false;
				waitBetweenLetters = defaultWaitBetweenLetters;
				skipable = true;

                dialogueBoxText.text = "";

				state = dialogueBoxState.WRITTING;

			break;

			case dialogueBoxState.WRITTING: //Currently writting in the dialogue box

				//If A is pressed, skip message
				if (InputManager.ActiveDevice.Action1.WasPressed && skipable) {
				
					letterIndex = messageToPrint.Length;
					nextLetterTime = 0;

				} 
				//If there's a face and it's time to put the next animation sprite
				else if (animationIndex != -1 && Time.time > nextAnimationStep) {
					
					//Get it's index
					animationIndex++;
					if (animationIndex == faceAnimationSprites.Count) {
						animationIndex = 0;
					}

					//Change the sprite and set the next change time
					face.sprite = faceAnimationSprites[animationIndex];
					nextAnimationStep = Time.time + faceAnimationSpeed;
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

						//Go to wait
						state = dialogueBoxState.WAITING;

						//If there's a face, stop the animation in the first frame
						if(animationIndex != -1) animationIndex = 0;
						face.sprite = faceAnimationSprites[0];

						//Remove the message from the list and activate the continue button (if it doesn't autojump)
						messageList.RemoveAt(0);
						if (!autoJump) {
                            if (InputManager.ActiveDevice.Name == "Keyboard/Mouse")
                                continueButtonKeyboard.SetActive(true);
                            else
							    continueButtonController.SetActive(true);
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
						
						audio.Play();

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
					continueButtonController.SetActive(false);
                    continueButtonKeyboard.SetActive(false);

					//If there are more text, we return to the prepare_text state
					if (messageList.Count > 0) {
						state = dialogueBoxState.PREPARE_TEXT;
					} else {
                        if (advanceBarEnemies != null)
                            advanceBarEnemies.Pause(false);
						dialogueBox.SetActive(false);
						if (player.allowMovement == false) {
							player.allowMovement = true;
							player.playerActivedJump = true;
						}
                        state = dialogueBoxState.OFF;
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

			//Change the face
			if (tag == "face"){
				
				//Recovers the character data from the DB and sets its values
				CharacterEntry character = dialogueCharacterDB.loadCharacter(value);

				if (character.faceAnimation.Count > 0) {
					
					//If it's a character, restore the interface if it was in "none" mode
					if (animationIndex == -1) {
						face.gameObject.SetActive(true);
						dialogueBoxTextRectTransform.offsetMin = new Vector2(200f, dialogueBoxTextRectTransform.offsetMin.y);
					}

					faceAnimationSprites = character.faceAnimation;
					animationIndex = 0;
					face.sprite = faceAnimationSprites[animationIndex];


				} //If it's "none" and it previously wasn't, set the interface to "none" mode
				else if (animationIndex != -1) { 
						animationIndex = -1;
						face.gameObject.SetActive(false);
						dialogueBoxTextRectTransform.offsetMin = new Vector2(0f, dialogueBoxTextRectTransform.offsetMin.y);
				}

				audio.pitch = character.pitch;
			}
			//Text speed
			if (tag == "speed") {
				waitBetweenLetters = float.Parse(value);
				return;
			}
			//Wait x time
			if (tag == "wait") {
				nextLetterTime = Time.time + float.Parse(value);
				showPointerBug = false;
				return;
			}
			//Skip this text upon ending
			if (tag == "skip") {
				autoJump = true;
				return;
			}
			//Don't allow to skip this text
			if (tag == "noskip") {
				skipable = false;
				return;
			}
			//Change background transparency
			if (tag == "bga"){
				Color bgcolor = background.color;
				bgcolor.a = float.Parse(value);
				background.color = bgcolor;
				return;
			}
			//Change text align
			if (tag == "align"){
				if (value == "center") {
					dialogueBoxText.alignment = TextAnchor.MiddleCenter;
				} else if (value == "left") {
					dialogueBoxText.alignment = TextAnchor.MiddleLeft;
				} else if (value == "right") {
					dialogueBoxText.alignment = TextAnchor.MiddleRight;
				}
			}
			//Allow or not the player to move
			if (tag == "playermove"){
				if (value == "true"){
					player.allowMovement = true;
				} else if (value == "false") {
					player.allowMovement = false;
				}
			}
			//Message ends instantaneously
			if (tag == "end"){
				nextLetterTime = 0;
				letterIndex = messageToPrint.Length - 1;
			}
            //Show tutorial arrows
            if (tag == "tutorialArrow")
            {
                int arrowValue = int.Parse(value);
                if (arrowValue == -1)
                {
                    for (int i = 0; i < tutoArrows.Length; i++)
                    {
                        tutoArrows[i].SetActive(false);
                    }
                }
                else if (arrowValue >= 0 && arrowValue < tutoArrows.Length)
                {
                    tutoArrows[arrowValue].SetActive(true);
                    tutoArrows[arrowValue].GetComponent<Image>().enabled = true;
                }
            }
		}
	}

    private void preRead(){

        //TODO: Inicio común con lo superior. Crear función común

        string tag = "";		//The tag name
        string value = "";		//The tag value
        bool tagging = true;	//Where writting the tag? Else, we're writting the value
        letterIndex++;

        //Do until the ']' or the message end
        while (letterIndex < messageToPrint.Length && messageToPrint[letterIndex] != ']')
        {

            //If it's the =, we're now writting the value
            if (messageToPrint[letterIndex] == '=')
            {
                tagging = false;
            }
            else
            {
                //Store the value where needed
                if (tagging)
                {
                    tag += messageToPrint[letterIndex];
                }
                else
                {
                    value += messageToPrint[letterIndex];
                }
            }
            letterIndex++;
        }

        //If it ended with ']' (not unclosed tag), search in the effects
        if (messageToPrint[letterIndex] == ']')
        {
            if ((tag == "controller" && InputManager.ActiveDevice.Name != "Keyboard/Mouse")
            || (tag == "keyboard" && InputManager.ActiveDevice.Name == "Keyboard/Mouse"))
            {
                messageToPrint = messageToPrint.Insert(letterIndex + 1, value);
            }
        }

    }

	public void callScene(int sceneNum){

		//We read the scene text of the given id
    	XmlNode scene = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/Scene[@id = \"" + sceneNum + "\"]");

		//We add the lines to the message list 
		for (int i = 0; i < scene.ChildNodes.Count; i++) {
			messageList.Add (scene.ChildNodes[i].InnerText);
		}

		//We prepare the text
		dialogueBox.SetActive(true);
        player.allowMovement = false;
		state = dialogueBoxState.PREPARE_TEXT;

        if(advanceBarEnemies != null)
            advanceBarEnemies.Pause(true);
	}

    private void correctToResolution()
    {
        if (Camera.current != null)
        {
            float multiplier = (37f - 33f) / ((16f / 9f) - (5f / 4f));
            float extra = 33 - ((5f / 4f) * multiplier);

            currentResolution = Camera.current.aspect;

            dialogueBoxText.fontSize = (int)((currentResolution * multiplier) + extra);
        }
    }
}
