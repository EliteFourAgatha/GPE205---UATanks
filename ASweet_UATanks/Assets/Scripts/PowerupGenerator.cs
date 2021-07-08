using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //DateTime for RNG

//This class is placed on PowerupSpawner prefab. PowerupSpawners are created in
// MapGenerator script upon game start. Class tracks current powerups and duration timers
public class PowerupGenerator : MonoBehaviour
{
    public GameObject[] pickupPrefabArray;
    public float spawnDelay;
    public float nextSpawnTime;
    private Transform tfRef;
    private GameObject spawnedPickup;
    private int randomIndex;
    void Start()
    {
        tfRef = gameObject.GetComponent<Transform>();
        //Set next spawn time to be after one delay cycle
        nextSpawnTime = Time.time + spawnDelay;
    }
    private void Update() 
    {
        //If there is nothing spawned in this location...
        if(spawnedPickup == null)
        {
            //..And it's time to spawn a new pickup
            if(Time.time > nextSpawnTime)
            {
                randomIndex = UnityEngine.Random.Range(0, pickupPrefabArray.Length);
                Debug.Log(randomIndex);
                //Spawn pickup and set nextSpawnTime
                spawnedPickup = Instantiate(pickupPrefabArray[randomIndex], tfRef.position + Vector3.up, Quaternion.identity);
                nextSpawnTime = Time.time + spawnDelay;
            }
        }
        //Otherwise, object still exists, postpone spawn
        else
        {
            nextSpawnTime = Time.time + spawnDelay;
        }
    }
}
