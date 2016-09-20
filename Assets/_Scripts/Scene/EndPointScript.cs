using UnityEngine;
using UnityEngine.UI;
using System.Xml;

//Everything is temporal!
public class EndPointScript : MonoBehaviour {

    public GameObject titlesGameObject;
    public GameObject player;
    public Text title;
    public Text subtitle;
    public TextAsset XMLAsset;

    private Rigidbody playerRig;
    private BoxCollider playerCol;
	private float endGame = -1;

    void Start()
    {
        playerRig = player.GetComponent<Rigidbody>();
        playerCol = player.GetComponent<BoxCollider>();
        enabled = false;
    }

    // Script is disabled until OnTriggerEnter detects the player
	void OnTriggerEnter(Collider coll){

		if(coll.gameObject.CompareTag("Player")){

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(XMLAsset.text);
            titlesGameObject.SetActive(true);
            XmlNode texts = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/EndMessage/Title");
            title.text = texts.InnerText;
			title.color = Color.red;
            texts = xmlDoc.SelectSingleNode("/Dialogue/Set[@lang = \"" + Configuration.getLanguage() + "\"]/EndMessage/Subtitle");
            subtitle.text = texts.InnerText;
			coll.transform.gameObject.GetComponent<PlayerController>().allowMovement = false;
			endGame = Time.time + 3.0f;
            enabled = true;
            playerCol.isTrigger = true;
            playerRig.isKinematic = true; 
		}
	}

	void Update(){

		if (endGame != -1 && Time.time >= endGame) {
            Loader.LoadScene("Score", false, false, true, true);
		}

	}

}
