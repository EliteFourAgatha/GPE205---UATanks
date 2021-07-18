using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTexture : MonoBehaviour
{
    public float scrollSpeed = 0.1f;
    Renderer renderer;
    public float aoeDuration = 3f;
    private float startTime;
    void Start()
    {
        renderer = gameObject.GetComponent<Renderer>();
        startTime = Time.time;
    }
    void Update()
    {
        PanTexture();
        CheckAOEDuration();
    }
    public void PanTexture()
    {
        float moveSpeed = Time.time * scrollSpeed;
        renderer.material.SetTextureOffset("_MainTex", new Vector2(0, moveSpeed));
    }
    public void CheckAOEDuration()
    {
        if(startTime + aoeDuration > Time.time)
        {

        }
    }
}
