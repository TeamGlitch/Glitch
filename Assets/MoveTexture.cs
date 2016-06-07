using UnityEngine;
using System.Collections;

public class MoveTexture : MonoBehaviour {
    public float scrollSpeed = 0.1f;
    public Renderer rend;
    public World world;

    void Start() {
        rend = GetComponent<Renderer>();
    }

    void Update() {
        Debug.Log(world.doUpdate);
        if(world.doUpdate)
        {
            Debug.Log(world.lag);
            float offset = world.lag * scrollSpeed;
            rend.material.SetTextureOffset("_MainTex", new Vector2(rend.material.GetTextureOffset("_MainTex").x + offset, 0));
        }
    }
}