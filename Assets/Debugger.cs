﻿using UnityEngine;
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
    B,
    N
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

        keyPressValue keypress = keyPressValue.N;

        if (InputManager.ActiveDevice.Action3.WasPressed)
            keypress = keyPressValue.A;
        else if (InputManager.ActiveDevice.Action2.WasPressed)
            keypress = keyPressValue.B;
        else if (valueY < 0.2 && valueY > -0.2)
        {
            if (valueX >= 0.9)
                keypress = keyPressValue.R;
            else if (valueX <= -0.9)
                keypress = keyPressValue.L;
        }
        else if (valueX < 0.2 && valueX > -0.2)
        {
            if (valueY >= 0.9)
                keypress = keyPressValue.U;
            else if (valueY <= -0.9)
                keypress = keyPressValue.D;
        }

        if (presses.Count == 0 || keypress != presses[presses.Count - 1].value)
        {
            presses.Add(new KeyPress(keypress, Time.time));

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

            checkCheats(pressText);
        }
	}

    private void checkCheats(string pressText)
    {
        if (compareCheat(pressText, "UUDDLRLRBA"))
        {
            print("kaching");
        }
    }

    private bool compareCheat(string input, string cheat)
    {
        string converter = cheat;

        int i = 1;
        while(i < converter.Length)
        {
            converter = converter.Insert(i, "N");
            i += 2;
        }

        if (input.Contains(converter))
        {
            presses.Clear();
            return true;
        }
        else
            return false;

    }
}
