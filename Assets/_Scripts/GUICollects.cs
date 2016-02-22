using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUICollects : MonoBehaviour {
    private int items;
    private Text itemNumber;

    void Start()
    {
        items = 0;
        itemNumber = GetComponent<Text>();
        itemNumber.text = items.ToString();
    }

	public void Increment()
    {
        ++items;
        itemNumber.text = items.ToString();
    }

    public void Decrement(int itemsDecremented)
    {
        items -= itemsDecremented;
        itemNumber.text = items.ToString();
    }
}
