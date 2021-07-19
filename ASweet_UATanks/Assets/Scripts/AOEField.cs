using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEField : MonoBehaviour
{
    public float scrollSpeed = 0.1f;
    Renderer lavaRenderer;
    public float aoeDuration = 3f;
    public float tickRate = 1f;
    private float startTime;

    void Start()
    {
        lavaRenderer = gameObject.GetComponent<Renderer>();
        startTime = Time.time;
    }
    void Update()
    {
        PanTexture();
        CheckAOEDuration();
    }
    //Slowly pan lava texture by (moveSpeed) value
    public void PanTexture()
    {
        float moveSpeed = Time.time * scrollSpeed;
        lavaRenderer.material.SetTextureOffset("_MainTex", new Vector2(0, moveSpeed));
    }
    //If either player tank enters aoe zone, do damage at (tickrate) interval
    public void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "PlayerOneTank")
        {
            TankHealth health = other.gameObject.GetComponent<TankHealth>();
            health.currentHealth -= tickRate * Time.deltaTime;
            Debug.Log(health.currentHealth);
        }
        else if(other.gameObject.tag == "PlayerTwoTank")
        {
            TankHealth health = other.gameObject.GetComponent<TankHealth>();
            health.currentHealth -= tickRate * Time.deltaTime;
        }
    }
    //Check if (aoeDuration) has expired. If so, destroy AOE field
    public void CheckAOEDuration()
    {
        if(Time.time - startTime > aoeDuration)
        {
            Debug.Log("aoe field destroyed");
            Destroy(gameObject);
        }
    }
}
