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
        if(world.doUpdate)
        {
            float offset = world.lag * scrollSpeed;
            rend.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
        }
    }
}