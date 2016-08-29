using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Medal : MonoBehaviour, ISelectHandler, IDeselectHandler{

    public float nextShine;
    public Button button;

    private GameObject explanation;

	// Use this for initialization
	void Awake () {
        button = GetComponent<Button>();
        explanation = transform.GetChild(3).gameObject;
        explanation.SetActive(false);

        nextShine = Time.time + Random.Range(3.0f, 5.0f);
	}
	
	// Update is called once per frame
	void Update () {

        if (Time.time > nextShine)
        {
            GetComponent<Animator>().Play("Shine");
            nextShine = Time.time + Random.Range(5.0f, 8.0f);
        }
	}

    public void OnSelect(BaseEventData eventData)
    {
        explanation.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        explanation.SetActive(false);
    }

    public void setImageAndText(Sprite sprite, string title, string text, float multiplier)
    {
        transform.GetChild(1).gameObject.GetComponent<Image>().sprite = sprite;
        explanation.transform.GetChild(0).gameObject.GetComponent<Text>().text = "<color=#FF0000FF>" + title + "</color>\n" + text;
        transform.GetChild(4).gameObject.GetComponent<Text>().text = "x" + multiplier;
    }

}
