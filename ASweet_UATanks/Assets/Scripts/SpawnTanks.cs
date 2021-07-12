using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //Using DateTime for RNG

public class SpawnTanks : MonoBehaviour
{
    public Game_Manager gameManager;
    public LifeManager lifeManager;
    public GameObject playerOneTank;
    public GameObject playerTwoTank;
    public Transform[] EnemyTanks;
    public GameObject bomberAI;
    public GameObject hunterAI;
    public GameObject cowardAI;
    public GameObject patrolAI;
    public List<GameObject> SpawnPoints;
    private int randomSpawnIndex;
    public bool singlePlayerEnabled = true;
    public bool multiPlayerEnabled = false;
    public float playerRespawnTime = 2f;
    public Camera playerOneGameCamera;
    public Camera playerTwoGameCamera;

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
        if(gameManager == null)
        {
            gameManager = gameObject.GetComponent<Game_Manager>();
        }
        if(lifeManager == null)
        {
            lifeManager = gameObject.GetComponent<LifeManager>();
        }
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
    }
    public void SpawnAtRandomSpawnpoint()
    {
        //Find all gameobjects that are tank spawn points
        SpawnPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("TankSpawnPoint"));

        if(PlayerPrefs.GetString("numPlayers") == "single")
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
            playerOneGameCamera.rect = new Rect(0, 0, 1, 1);

            playerOneTank.GetComponent<InputController>().input = InputController.InputScheme.WASD;

            playerOneTank.SetActive(true);
        }
        else if(PlayerPrefs.GetString("numPlayers") == "multi")
        {
            //Spawn player one
            //Get random spawn point from array, from 0 to length of list
            randomSpawnIndex = UnityEngine.Random.Range(0, SpawnPoints.Count);
            Debug.Log("p1 spawn" + randomSpawnIndex);

            //Set player start position equal to random spawn point
            Transform playerSpawn = SpawnPoints[randomSpawnIndex].transform;
            playerOneTank.transform.position = playerSpawn.position;

            //Spawn player two
            Debug.Log("p2 spawn:" + randomSpawnIndex);

            //Set player start position equal to random spawn point
            Transform playerTwoSpawn = SpawnPoints[(randomSpawnIndex + 1)].transform;
            playerTwoTank.transform.position = playerSpawn.position;
            //Remove current random index from list, avoid duplicate spawns
            SpawnPoints.Remove(SpawnPoints[randomSpawnIndex]);

            //Set camera values to split screen
            //(X, Y, Width, Height)
            playerOneGameCamera.rect = new Rect(0, 0, 0.5f, 1);
            playerTwoGameCamera.rect = new Rect(0.5f, 0, 0.5f, 1);

            playerOneTank.GetComponent<InputController>().input = InputController.InputScheme.WASD;
            playerTwoTank.GetComponent<InputController>().input = InputController.InputScheme.arrowkeys;
        
            playerOneTank.SetActive(true);
            playerTwoTank.SetActive(true);
        }
        
        //Iterate through all EnemyTanks in array. Get random index and set
        // enemy's position equal to index's position
        int i = 0;
        foreach(Transform enemy in EnemyTanks)
        {
            i++;
            randomSpawnIndex = UnityEngine.Random.Range(0, SpawnPoints.Count);
            Debug.Log("Enemy" + i + " spawned at " + randomSpawnIndex);
            Transform enemySpawn = SpawnPoints[randomSpawnIndex].transform;
            if(enemy.name == "BomberAI")
            {
                bomberAI.transform.position = enemySpawn.position;
                bomberAI.SetActive(true);
            }
            else if(enemy.name == "CowardAI")
            {
                cowardAI.transform.position = enemySpawn.position;
                cowardAI.SetActive(true);
            }
            else if(enemy.name == "HunterAI")
            {
                hunterAI.transform.position = enemySpawn.position;
                hunterAI.SetActive(true);
            }
            else if(enemy.name == "PatrolAI")
            {
                patrolAI.transform.position = enemySpawn.position;
                patrolAI.SetActive(true);
            }
            SpawnPoints.Remove(SpawnPoints[randomSpawnIndex]);
        }
    }

    public void RespawnPlayer(int playerNum)
    {
        StartCoroutine(WaitForRespawnTimer());
        //Find all gameobjects that are tank spawn points
        SpawnPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("TankSpawnPoint"));
        //Get random spawn point from array, from 0 to length of list
        randomSpawnIndex = UnityEngine.Random.Range(0, SpawnPoints.Count);

        if(playerNum == 1)
        {
            //Set player start position equal to random spawn point
            Transform playerSpawn = SpawnPoints[randomSpawnIndex].transform;
            playerOneTank.transform.position = playerSpawn.position;
            playerOneTank.SetActive(true);
        }
        else if(playerNum == 2)
        {
            //Set player start position equal to random spawn point
            Transform playerSpawn = SpawnPoints[(randomSpawnIndex + 1)].transform;
            playerTwoTank.transform.position = playerSpawn.position;
            playerTwoTank.SetActive(true);
        }
    }
    public void WaitAndRespawnEnemy(GameObject aiType)
    {
        StartCoroutine(WaitForRespawnTimer());
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        //Find all gameobjects that are tank spawn points
        SpawnPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("TankSpawnPoint"));

        //Get random spawn point from array, from 0 to length of list
        randomSpawnIndex = UnityEngine.Random.Range(0, SpawnPoints.Count);

        //Set player start position equal to random spawn point
        Transform enemySpawn = SpawnPoints[randomSpawnIndex].transform;
        if(aiType == bomberAI)
        {
            Debug.Log("respawning bomber ai");
            bomberAI.transform.position = enemySpawn.position;
            bomberAI.SetActive(true);
        }
        else if(aiType == cowardAI)
        {
            Debug.Log("respawning coward ai");
            cowardAI.transform.position = enemySpawn.position;
            cowardAI.SetActive(true);
        }
        else if(aiType == hunterAI)
        {
            Debug.Log("respawning hunter ai");
            hunterAI.transform.position = enemySpawn.position;
            hunterAI.SetActive(true);
        }  
        else if(aiType == patrolAI)
        {
            Debug.Log("respawning patrol ai");
            patrolAI.transform.position = enemySpawn.position;
            patrolAI.SetActive(true);
        }    
    }
    /*
    public void CheckForOverlap(Vector3 targetPos, Vector3 boxExtents, int playerNum)
    {
        Collider[] colliders = Physics.OverlapBox(targetPos, boxExtents);
        for(int i = 0; i < colliders.Length; i++)
        {
            //If tank is currently in attempted spawn location, wait and try again
            if(colliders[i].tag == "EnemyTank" || colliders[i].tag == "PlayerOneTank"
                || colliders[i].tag == "PlayerTwoTank")
            {
                RespawnPlayer(playerNum);
            }
        }
    }
    */

    IEnumerator WaitForRespawnTimer()
    {
        yield return new WaitForSecondsRealtime(playerRespawnTime); 
    }
}
