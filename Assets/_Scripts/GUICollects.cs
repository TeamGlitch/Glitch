using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUICollects : MonoBehaviour {
    public PlayerController player;
    private Text itemNumber;

    void Start()
    {
        itemNumber = GetComponent<Text>();
        itemNumber.text = player.items.ToString();
    }

	public void GUIItemRepresent()
    {
        itemNumber.text = player.items.ToString();
    }
}