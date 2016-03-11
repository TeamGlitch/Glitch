using UnityEngine;
using System.Collections;

public class DebugScript : MonoBehaviour {

	private float ms = 0.000f;
	private float fps = 0.0f;

	private GameObject[] allObjects;
	private float totalPoly;

	private bool debugMode;
	private bool wireframeMode;

	void Start()
	{
		allObjects = (GameObject[]) GameObject.FindObjectsOfType(typeof(GameObject));

		debugMode = false;
		wireframeMode = false;
	}

// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.F1)) {
			debugMode = !debugMode;
		} 
		if (Input.GetKeyDown (KeyCode.F2) && !wireframeMode)
		{
			Camera.onPreRender += myPreRenderer;
			Camera.onPostRender += myPostRenderer;
			wireframeMode = true;
		} 
		else if (Input.GetKeyDown (KeyCode.F2))
		{
			Camera.onPreRender -= myPreRenderer;
			Camera.onPostRender -= myPostRenderer;
			wireframeMode = false;
		}

		if (debugMode) {
			ms = Time.deltaTime;
			fps = 1.0f / ms;
			float polyCount = 0;
			int vertCount = 0;
			for (int i = 0; i < allObjects.Length; ++i)
			{
				GameObject obj = allObjects [i];
				Renderer rend = obj.GetComponent<Renderer> ();
				if (rend && rend.isVisible)
				{
					MeshFilter mf = obj.GetComponent<MeshFilter>();
					if (mf)
					{
						polyCount += mf.mesh.triangles.Length / 3;
					}
				}
			}
			totalPoly = polyCount;
		}

	}


	void OnGUI() {
		if (debugMode) {
			GUI.Label (new Rect (Screen.width - 100, 10, 100, 20), "fps: " + (int)fps);
			string msString = ms.ToString ();
			if (msString.Length > 5)
				GUI.Label (new Rect (Screen.width - 100, 30, 100, 20), "ms: " + msString.Remove (5));
			else
				GUI.Label (new Rect (Screen.width - 100, 30, 100, 20), "ms: " + msString);
			GUI.Label (new Rect (Screen.width - 100, 50, 100, 20), "tri/s: " + totalPoly.ToString ());
		}
	}
		
	void myPreRenderer(Camera cam)
	{
		GL.wireframe = true;
	}


	void myPostRenderer(Camera cam)
	{
		GL.wireframe = false;
	}

}
