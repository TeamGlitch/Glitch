using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDCollects : MonoBehaviour {
    public PlayerController player;
    private Text itemNumber;

    void Start()
    {
        itemNumber = GetComponent<Text>();
        itemNumber.text = player.items.ToString();
    }

	// Function that represent the number of items collected
	public void GUIItemRepresent()
    {
        itemNumber.text = player.items.ToString();
    }
}