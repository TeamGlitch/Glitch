using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public enum keyPressValue
{
    U,
    D,
    L,
    R,
    A,
    B
}

public class KeyPress
{
    public keyPressValue value;
    public float time;

    public KeyPress(keyPressValue iniValue, float iniTime)
    {
        value = iniValue;
        time = iniTime;
    }
}

public class Debugger : MonoBehaviour {

    private List<KeyPress> presses = new List<KeyPress>();
	
	// Update is called once per frame
	void Update () {

        float valueX = InputManager.ActiveDevice.LeftStickX.Value;
        float valueY = InputManager.ActiveDevice.LeftStickY.Value;

        KeyPress keypress = null;

        if (InputManager.ActiveDevice.Action3.WasPressed)
            keypress = new KeyPress(keyPressValue.A, Time.time);
        else if (InputManager.ActiveDevice.Action2.WasPressed)
            keypress = new KeyPress(keyPressValue.B, Time.time);
        else if (valueY < 0.2 && valueY > -0.2)
        {
            if (valueX >= 0.9)
                keypress = new KeyPress(keyPressValue.R, Time.time);
            else if (valueX <= -0.9)
                keypress = new KeyPress(keyPressValue.L, Time.time);
        }
        else if (valueX < 0.2 && valueX > -0.2)
        {
            if (valueY >= 0.9)
                keypress = new KeyPress(keyPressValue.U, Time.time);
            else if (valueY <= -0.9)
                keypress = new KeyPress(keyPressValue.D, Time.time);
        }

        if (keypress != null && (presses.Count == 0 || keypress.value != presses[presses.Count - 1].value))
        {
            presses.Add(keypress);
        }


        string pressText = "";
        for (int i = presses.Count - 1; i >= 0; i--)
        {
            if (Time.time > presses[i].time + 10.0f)
            {
                presses.RemoveAt(i);
            }
            else
            {
                pressText = presses[i].value + pressText;
            }
        }

        print(pressText);

        if (pressText.Contains("UDLRLRBA"))
        {
            print("kaching");
        }
	}
}
