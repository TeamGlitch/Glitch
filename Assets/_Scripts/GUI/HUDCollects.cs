using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDCollects : MonoBehaviour {
    public Player player;
    public Text itemNumber;

    void Start()
    {
        itemNumber.text = player.items.ToString();
    }

	// Function that represent the number of items collected
	public void GUIItemRepresent()
    {
        itemNumber.text = player.items.ToString();
    }
}