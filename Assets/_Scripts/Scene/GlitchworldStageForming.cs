using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlitchworldStageForming : MonoBehaviour {

    private List<MeshRenderer> renderers;
    private List<MeshRenderer> rendering = new List<MeshRenderer>();

    public List<MeshRenderer> exceptions;
    public Material preMaterial;
    public Material duringMaterial;
    public Material postMaterial;

    public float startDistance;
    public float endDistance;

	// Use this for initialization
	void Start () {

        renderers = new List<MeshRenderer>(transform.GetComponentsInChildren<MeshRenderer>());

        renderers.Sort(delegate(MeshRenderer a, MeshRenderer b)
        {
            return (a.bounds.min.x).CompareTo(b.bounds.min.x);
        });

        for (int i = 0; i < renderers.Count; i++)
        {
            if (exceptions.Contains(renderers[i]))
            {
                renderers[i].material = postMaterial;
                renderers.RemoveAt(i);
                i--;
            }
            else
                renderers[i].material = preMaterial;
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (Camera.current != null && Camera.current == Camera.main)
        {
            Vector3 camPosition;
            for (int i = 0; i < renderers.Count; i++)
            {
                camPosition = Camera.main.WorldToViewportPoint(renderers[i].bounds.min);
                if (camPosition.x < startDistance)
                {
                    renderers[i].material = duringMaterial;
                    rendering.Add(renderers[i]);
                    renderers.RemoveAt(i);
                    i--;
                }
                else
                    break;
            }

            for (int i = 0; i < rendering.Count; i++)
            {
                camPosition = Camera.main.WorldToViewportPoint(rendering[i].bounds.min);

                if (camPosition.x < endDistance)
                {
                    rendering[i].material = postMaterial;
                    rendering.RemoveAt(i);
                    i--;
                }
                else
                {
                    float percent = ((camPosition.x - endDistance) / (endDistance - startDistance)) * -1;

                    float value = 4.6484f * Mathf.Pow(percent, 5);
                    value -= 5.3789f * Mathf.Pow(percent, 4);
                    value += 1.6635f * Mathf.Pow(percent, 3);
                    value += 0.0695f * Mathf.Pow(percent, 2);
                    value += 0.0006f * percent;
                    value += 0.0005f;

                    rendering[i].material.SetFloat("_TimeLeft", value);

                }
            }

        }
	}

}
