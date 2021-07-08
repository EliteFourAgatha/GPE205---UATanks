using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //Using DateTime for RNG

public class SpawnTanks : MonoBehaviour
{
    public GameObject playerOneTank;
    public GameObject playerTwoTank;
    public Transform[] EnemyTanks;
    public List<GameObject> SpawnPoints;
    private int randomSpawnIndex;
    public bool singlePlayerEnabled = true;
    public bool multiPlayerEnabled = false;
    public Camera playerOneCamera;
    public Camera playerTwoCamera;
    private void Start()
    {
        if(playerOneTank == null)
        {
            playerOneTank = GameObject.FindGameObjectWithTag("PlayerOneTank");
        }
        if(playerTwoTank == null)
        {
            playerTwoTank = GameObject.FindGameObjectWithTag("PlayerTwoTank");
        }
    }
    public void SpawnAtRandomSpawnpoint()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        //Find all gameobjects that are tank spawn points
        SpawnPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("TankSpawnPoint"));

        if(singlePlayerEnabled)
        {
            //Spawn player one
            //Get random spawn point from array, from 0 to length of list
            randomSpawnIndex = UnityEngine.Random.Range(0, SpawnPoints.Count);

            //Set player start position equal to random spawn point
            Transform playerSpawn = SpawnPoints[randomSpawnIndex].transform;
            playerOneTank.transform.position = playerSpawn.position;
            //Remove current random index from list, avoid duplicate spawns
            SpawnPoints.Remove(SpawnPoints[randomSpawnIndex]);

            //Return camera rect to default values to view entire screen
            playerOneCamera.rect = new Rect(0, 0, 1, 1);

            playerOneTank.GetComponent<InputController>().input = InputController.InputScheme.WASD;
        }
        else if(multiPlayerEnabled)
        {
            //Spawn player one
            //Get random spawn point from array, from 0 to length of list
            randomSpawnIndex = UnityEngine.Random.Range(0, SpawnPoints.Count);

            //Set player start position equal to random spawn point
            Transform playerSpawn = SpawnPoints[randomSpawnIndex].transform;
            playerOneTank.transform.position = playerSpawn.position;
            //Remove current random index from list, avoid duplicate spawns
            SpawnPoints.Remove(SpawnPoints[randomSpawnIndex]);

            //Spawn player two
            //Get random spawn point from array, from 0 to length of list
            randomSpawnIndex = UnityEngine.Random.Range(0, SpawnPoints.Count);

            //Set player start position equal to random spawn point
            Transform playerTwoSpawn = SpawnPoints[randomSpawnIndex].transform;
            playerTwoTank.transform.position = playerSpawn.position;
            //Remove current random index from list, avoid duplicate spawns
            SpawnPoints.Remove(SpawnPoints[randomSpawnIndex]);

            //Set camera values to split screen
            //(X, Y, Width, Height)
            playerOneCamera.rect = new Rect(0, 0, 0.5f, 1);
            playerTwoCamera.rect = new Rect(0.5f, 0, 0.5f, 1);

            playerOneTank.GetComponent<InputController>().input = InputController.InputScheme.WASD;
            playerTwoTank.GetComponent<InputController>().input = InputController.InputScheme.arrowkeys;
        }
        
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
