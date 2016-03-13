using UnityEngine;

public class DebugScript : MonoBehaviour {

	private float ms = 0.000f;
	private float fps = 0.0f;
	private GameObject[] allObjects;
    private Renderer[] allRenderers;
    private MeshFilter[] allMeshFilters;
	private float totalPoly;
	private bool debugMode;
	private bool wireframeMode;

	void Start()
	{
		allObjects = (GameObject[]) GameObject.FindObjectsOfType(typeof(GameObject));
        allRenderers = new Renderer[allObjects.Length];
        allMeshFilters = new MeshFilter[allObjects.Length];
        for (int i = 0; i < allObjects.Length; ++i)
        {
            allRenderers[i] = allObjects[i].GetComponent<Renderer>();
            allMeshFilters[i] = allObjects[i].GetComponent<MeshFilter>();
        }
		debugMode = false;
		wireframeMode = false;
	}


	void Update()
	{
		if (Input.GetKeyDown (KeyCode.F1)) 
        {
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

		if (debugMode) 
        {
			ms = Time.deltaTime;
			fps = 1.0f / ms;
			float polyCount = 0;
			for (int i = 0; i < allObjects.Length; ++i)
			{
                if (allRenderers[i] && allRenderers[i].isVisible)
				{
					if (allMeshFilters[i])
					{
						polyCount += allMeshFilters[i].mesh.triangles.Length / 3;
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
		
    // Activate wireframe
	void myPreRenderer(Camera cam)
	{
		GL.wireframe = true;
	}

    // Deactivate wireframe
	void myPostRenderer(Camera cam)
	{
		GL.wireframe = false;
	}

}
