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

    public GameObject HUD;

	// Update is called once per frame
	void Update () {

        #if UNITY_EDITOR

        int value = -1;

        if (Input.GetKeyDown(KeyCode.Keypad1)) value = 1;
        else if (Input.GetKeyDown(KeyCode.Keypad2)) value = 2;
        else if (Input.GetKeyDown(KeyCode.Keypad3)) value = 3;
        else if (Input.GetKeyDown(KeyCode.Keypad4)) value = 4;
        else if (Input.GetKeyDown(KeyCode.Keypad5)) value = 5;
        else if (Input.GetKeyDown(KeyCode.Keypad6)) value = 6;
        else if (Input.GetKeyDown(KeyCode.Keypad7)) value = 7;
        else if (Input.GetKeyDown(KeyCode.Keypad8)) value = 8;
        else if (Input.GetKeyDown(KeyCode.Keypad9)) value = 9;

        if(value != -1)
        {
            GameObject point = GameObject.Find("StartNextPart" + value);
            GameObject go = GameObject.Find("Player");

            if (point != null && go != null)
            {
                Vector3 position = point.transform.position;
                go.transform.position = new Vector3(position.x, position.y, go.transform.position.z);
            }
        }

        #endif


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
            checkCheats();
        }
	}

    private void checkCheats()
    {
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

        if (compareCheat(pressText, "UUDDLRLRBA"))
        {
            GameObject go = GameObject.Find("Player");
            if (go != null)
            {
                Player pl = go.GetComponent<Player>();
                if (pl != null)
                {
                    if(pl.godmode)
                    {
                        pl.godmode = false;
                        print("GODMODE DEACTIVATED");
                    }
                    else
                    {
                        pl.godmode = true;
                        print("GODMODE ACTIVATED");
                    }

                }
            }
        }
        else if (compareCheat(pressText, "URDLURDLB"))
        {
            Canvas hud = HUD.GetComponent<Canvas>();
            if (hud.isActiveAndEnabled)
                hud.enabled = false;
            else
                hud.enabled = true;
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
