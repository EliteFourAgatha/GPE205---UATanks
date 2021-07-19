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
    public GameObject aiPatrolWaypoint;
    public List<GameObject> SpawnPoints;
    public List<Transform> GeneratedWaypoints;
    private int randomSpawnIndexOne;
    private int randomSpawnIndexTwo;
    public bool singlePlayerEnabled = true;
    public bool multiPlayerEnabled = false;
    public float playerRespawnTime = 2f;
    public bool patrolAISpawned;
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
            randomSpawnIndexOne = UnityEngine.Random.Range(0, SpawnPoints.Count);

            //Set player start position equal to random spawn point
            Transform playerSpawn = SpawnPoints[randomSpawnIndexOne].transform;
            playerOneTank.transform.position = playerSpawn.position + new Vector3(0, 0.5f, 0);
            //Remove current random index from list, avoid duplicate spawns
            SpawnPoints.Remove(SpawnPoints[randomSpawnIndexOne]);

            //Return camera rect to default values to view entire screen
            playerOneGameCamera.rect = new Rect(0, 0, 1, 1);

            playerOneTank.GetComponent<InputController>().input = InputController.InputScheme.WASD;

            playerOneTank.SetActive(true);
        }
        else if(PlayerPrefs.GetString("numPlayers") == "multi")
        {
            //Spawn player one
            //Get random spawn point from array, from 0 to length of list
            randomSpawnIndexOne = UnityEngine.Random.Range(0, SpawnPoints.Count);
            Debug.Log("p1 spawn" + randomSpawnIndexOne);

            //Set player start position equal to random spawn point
            Transform playerSpawn = SpawnPoints[randomSpawnIndexOne].transform;
            playerOneTank.transform.position = playerSpawn.position + new Vector3(0, 0.5f, 0);
            playerOneTank.SetActive(true);
            SpawnPoints.Remove(SpawnPoints[randomSpawnIndexOne]);

            //Spawn player two
            randomSpawnIndexTwo = UnityEngine.Random.Range(0, SpawnPoints.Count);
            if(randomSpawnIndexOne == randomSpawnIndexTwo)
            {
                if(randomSpawnIndexTwo > 0)
                {
                    randomSpawnIndexTwo --;
                }
                else
                {
                    randomSpawnIndexTwo ++;
                }
            }
            //Set player start position equal to random spawn point
            Transform playerTwoSpawn = SpawnPoints[randomSpawnIndexTwo].transform;
            Debug.Log("p2 spawn:" + randomSpawnIndexTwo);
            playerTwoTank.transform.position = playerSpawn.position + new Vector3(0, 0.5f, 0);
            playerTwoTank.SetActive(true);
            //Remove current random index from list, avoid duplicate spawns
            SpawnPoints.Remove(SpawnPoints[randomSpawnIndexTwo]);

            //Set camera values to split screen
            //(X, Y, Width, Height)
            playerOneGameCamera.rect = new Rect(0, 0, 0.5f, 1);
            playerTwoGameCamera.rect = new Rect(0.5f, 0, 0.5f, 1);

            playerOneTank.GetComponent<InputController>().input = InputController.InputScheme.WASD;
            playerTwoTank.GetComponent<InputController>().input = InputController.InputScheme.arrowkeys;
        }
        
        //Iterate through all EnemyTanks in array. Get random index and set
        // enemy's position equal to index's position
        int i = 0;
        foreach(Transform enemy in EnemyTanks)
        {
            i++;
            randomSpawnIndexOne = UnityEngine.Random.Range(0, SpawnPoints.Count);
            Debug.Log("Enemy" + i + " spawned at " + randomSpawnIndexOne);
            Transform enemySpawn = SpawnPoints[randomSpawnIndexOne].transform;
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
                patrolAISpawned = false;
                patrolAI.transform.position = enemySpawn.position;
                ResetPatrolWaypoints();
                SpawnWaypointsForPatroller(patrolAI.transform.position);
                patrolAISpawned = true;
                patrolAI.SetActive(true);
            }
            SpawnPoints.Remove(SpawnPoints[randomSpawnIndexOne]);
        }
    }

    public void RespawnPlayer(int playerNum)
    {
        //StartCoroutine(WaitForRespawnTimer());
        //Find all gameobjects that are tank spawn points
        SpawnPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("TankSpawnPoint"));
        //Get random spawn point from array, from 0 to length of list
        randomSpawnIndexOne = UnityEngine.Random.Range(0, SpawnPoints.Count);

        if(playerNum == 1)
        {
            //Set player start position equal to random spawn point
            Transform playerSpawn = SpawnPoints[randomSpawnIndexOne].transform;
            Debug.Log("p1 respawn point" + randomSpawnIndexOne);
            playerOneTank.transform.position = playerSpawn.position + new Vector3(0, 0.5f, 0);
            playerOneTank.SetActive(true);
        }
        else if(playerNum == 2)
        {
            //Set player start position equal to random spawn point
            Transform playerSpawn = SpawnPoints[randomSpawnIndexOne].transform;
            playerTwoTank.transform.position = playerSpawn.position + new Vector3(0, 0.5f, 0);
            playerTwoTank.SetActive(true);
        }
    }
    public void WaitAndRespawnEnemy(GameObject aiType)
    {
        //StartCoroutine(WaitForRespawnTimer());
        //Find all gameobjects that are tank spawn points
        SpawnPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("TankSpawnPoint"));

        //Get random spawn point from array, from 0 to length of list
        randomSpawnIndexOne = UnityEngine.Random.Range(0, SpawnPoints.Count);

        //Set enemy start position equal to random spawn point
        Transform enemySpawn = SpawnPoints[randomSpawnIndexOne].transform;
        if(aiType.tag == "BomberAI")
        {
            bomberAI.transform.position = enemySpawn.position + new Vector3(0, 0.5f, 0);
            bomberAI.SetActive(true);
            Debug.Log("respawned bomber ai");
        }
        else if(aiType == cowardAI)
        {
            Debug.Log("respawning coward ai");
            cowardAI.transform.position = enemySpawn.position + new Vector3(0, 0.5f, 0);
            cowardAI.SetActive(true);
        }
        else if(aiType == hunterAI)
        {
            Debug.Log("respawning hunter ai");
            hunterAI.transform.position = enemySpawn.position + new Vector3(0, 0.5f, 0);
            hunterAI.SetActive(true);
        }  
        else if(aiType == patrolAI)
        {
            patrolAISpawned = false;
            Debug.Log("respawning patrol ai");
            patrolAI.transform.position = enemySpawn.position + new Vector3(0, 0.5f, 0);
            ResetPatrolWaypoints();
            SpawnWaypointsForPatroller(patrolAI.transform.position);
            patrolAISpawned = true;
            patrolAI.SetActive(true);
        }    
    }
    IEnumerator WaitForRespawnTimer()
    {
        yield return new WaitForSecondsRealtime(playerRespawnTime); 
    }
    //Spawn waypoints in the each of the corners of the room the Patrol AI is spawned in
    public void SpawnWaypointsForPatroller(Vector3 patrolSpawnpoint)
    {
        //20, 20. -20, 20. -20, -20. 20, -20. values for corners of each room. 20 in each direction
        GameObject waypointOne = Instantiate(aiPatrolWaypoint, patrolSpawnpoint + new Vector3(20, 0, 20), Quaternion.identity);
        GameObject waypointTwo = Instantiate(aiPatrolWaypoint, patrolSpawnpoint + new Vector3(20, 0, -20), Quaternion.identity);
        GameObject waypointThree = Instantiate(aiPatrolWaypoint, patrolSpawnpoint + new Vector3(-20, 0, -20), Quaternion.identity);
        GameObject waypointFour = Instantiate(aiPatrolWaypoint, patrolSpawnpoint + new Vector3(-20, 0, 20), Quaternion.identity);      
        
        GeneratedWaypoints.Add(waypointOne.transform);
        GeneratedWaypoints.Add(waypointTwo.transform);
        GeneratedWaypoints.Add(waypointThree.transform);
        GeneratedWaypoints.Add(waypointFour.transform);
    }
    //Find all waypoints and destroy them before spawning new ones
    public void ResetPatrolWaypoints()
    {
        GameObject[] waypointArray = GameObject.FindGameObjectsWithTag("PatrolWaypoint");
        foreach (GameObject waypoint in waypointArray)
        {
            Destroy(waypoint);
        }
    }
}
