using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //Using DateTime for RNG

public class SpawnTanks : MonoBehaviour
{
    public GameObject playerTank;
    public Transform[] EnemyTanks;
    public List<GameObject> SpawnPoints;
    private int randomSpawnIndex;
    private void Start()
    {
        if(playerTank == null)
        {
            playerTank = GameObject.FindGameObjectWithTag("PlayerTank");
        }
    }
    public void SpawnAtRandomSpawnpoint()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        //Find all gameobjects that are tank spawn points
        SpawnPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("TankSpawnPoint"));

        //Get random spawn point from array, from 0 to length of list
        randomSpawnIndex = UnityEngine.Random.Range(0, SpawnPoints.Count);

        //Set player start position equal to random spawn point
        Transform playerSpawn = SpawnPoints[randomSpawnIndex].transform;
        playerTank.transform.position = playerSpawn.position;
        //Remove current random index from list, avoid duplicate spawns
        SpawnPoints.Remove(SpawnPoints[randomSpawnIndex]);
        
        //Iterate through all EnemyTanks in array. Get random index and set
        // enemy's position equal to index's position
        //  Call instantiate here because objects don't yet exist in scene, as opposed to playerTank
        foreach(Transform enemy in EnemyTanks)
        {
            randomSpawnIndex = UnityEngine.Random.Range(0, SpawnPoints.Count);
            Debug.Log(randomSpawnIndex);
            Transform enemySpawn = SpawnPoints[randomSpawnIndex].transform;
            enemy.position = enemySpawn.position;
            Instantiate(enemy, enemy.position, Quaternion.identity);
            SpawnPoints.Remove(SpawnPoints[randomSpawnIndex]);
        }
        //After all objects spawned, destroy spawnpoint gameobjects to declutter scene
        foreach(GameObject spawnPoint in SpawnPoints)
        {
            Debug.Log("Destroy all spawnpoints!");
            Destroy(spawnPoint);
        }
    }
}
