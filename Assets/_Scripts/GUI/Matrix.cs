using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Matrix : MonoBehaviour {

    private List<List<Text>> columns;
    private List<int> active;

    public float frequency = 0.2f;
    public float variation = 0.3f;
    private float nextOne = 0;

    public float speed = 0.05f;
    private float nextStep = 0;

    private Color32[] matrixColors = {
                                   new Color32(0,255,48,255),
                                   new Color32(0,202,38,255),
                                   new Color32(0,202,38,255),
                                   new Color32(0,210,40,216),
                                   new Color32(0,222,42,158),
                                   new Color32(0,235,44,97),
                                   new Color32(0,247,46,38)
    };

	// Use this for initialization
	void Start () {

        columns = new List<List<Text>>();
        active = new List<int>();

        GameObject number = transform.GetChild(0).gameObject;
        RectTransform numberRT = number.GetComponent<RectTransform>();

        Vector2 numPos = new Vector2(0, 0);
        while (numPos.x < Screen.width)
        {
            numPos.y = -Screen.height;
            List<Text> column = new List<Text>();

            while (numPos.y < 0)
            {
                
                GameObject returned = Object.Instantiate(number);
                returned.transform.SetParent(transform);
                returned.GetComponent<RectTransform>().position = new Vector3(numPos.x, numPos.y + Screen.height);

                Text text = returned.GetComponent<Text>();
                column.Add(text);
                text.gameObject.SetActive(false);


                numPos.y += numberRT.rect.height;
            }

            columns.Add(column);
            active.Add(-1);
            numPos.x += numberRT.rect.width;
        }

        Destroy(number);

	}
	
	// Update is called once per frame
	void Update () {

        if (Time.time > nextOne)
        {
            List<int> available = new List<int>();

            for (int i = 0; i < active.Count; i++)
            {
                if (active[i] == -1)
                    available.Add(i);
            }

            if (available.Count > 0)
            {
                int selected = available[Random.Range(0, available.Count)];
                active[selected] = 0;
                nextOne = Time.time + (frequency * Random.Range(1 - variation, 1 + variation));
            }

        }

        if (Time.time > nextStep)
        {
            nextStep = Time.time + speed;

            for (int i = 0; i < active.Count; i++)
            {
                if (active[i] != -1)
                {

                    int head = active[i];

                    if (head == columns[i].Count + matrixColors.Length)
                    {
                        active[i] = -1;
                        columns[i][columns[i].Count - 1].gameObject.SetActive(false);

                    }
                    else
                    {
                        for (int z = 0; z < columns[i].Count; z++)
                        {
                            if(z <= head && z >= head - matrixColors.Length + 1)
                            {
                                if (!columns[i][z].gameObject.activeSelf)
                                {
                                    columns[i][z].gameObject.SetActive(true);
                                }

                                if(Random.value < 0.5f)
                                {
                                    columns[i][z].text = "0";
                                }
                                else {
                                    columns[i][z].text = "1";
                                }
                                columns[i][z].color = matrixColors[head - z];

                            }
                            else if (columns[i][z].gameObject.activeSelf)
                            {
                                columns[i][z].gameObject.SetActive(false);
                            }
                        }

                        active[i]++;
                    }
                }
            }
        }
	}
}
