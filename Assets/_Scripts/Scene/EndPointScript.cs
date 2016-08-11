using UnityEngine;
using UnityEngine.UI;
using System.Xml;

//Everything is temporal!
public class EndPointScript : MonoBehaviour {

    public GameObject titlesGameObject;
    public Text title;
    public Text subtitle;

    public TextAsset XMLAsset;

	private float endGame = -1;

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
		}
	}

	void Update(){

		if (endGame != -1 && Time.time >= endGame) {
            Loader.LoadScene("Congratulations", true, false, true, true);
		}

	}

}
