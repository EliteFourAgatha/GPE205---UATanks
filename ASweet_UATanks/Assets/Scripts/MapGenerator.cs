using System.Collections;
using System.Collections.Generic;
using System; // Using DateTime
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int rows;
    public int columns;
    private float roomWidth = 50f;
    private float roomHeight = 50f;
    public Game_Manager gameManager;
    public SpawnTanks spawnTanks;
    public GameObject tankSpawnPoint;
    public GameObject aiPatrolWaypoint;
    //List to hold waypoints generated in GenerateGrid. Copying those values to AIPatrol array.
    public List<Transform> generatedWaypoints;
    public GameObject powerupSpawner;
    public GameObject[] gridPrefabArray;
    //Grid that is manipulated at runtime by MapGenerator script.
    // Comma tells C# you are using 2 numbers to refer to single location in memory
    //  Store in [X,Y] fashion. Call for grid[3,5] instead of grid[28]
    public Room[,] grid;
    public enum RandomSpawnType{random, presetSeed, mapOfDay};
    public RandomSpawnType randomSpawnType = RandomSpawnType.random;
    private int mapSeed;
    public int presetSeed = 10;
    public bool gridGenerated = false;

    void Start()
    {
        spawnTanks = gameObject.GetComponent<SpawnTanks>();
        gameManager = gameObject.GetComponent<Game_Manager>();
        if(randomSpawnType == RandomSpawnType.mapOfDay)
        {
            //Use DateTime and call current date
            // DateTime.Now.Date returns year/month/date
            mapSeed = DateToInt(DateTime.Now.Date);
        }
        else if(randomSpawnType == RandomSpawnType.random)
        {
            //Use DateTime and call current time, including seconds/milliseconds
            mapSeed = DateToInt(DateTime.Now);
        }
        else if(randomSpawnType == RandomSpawnType.presetSeed)
        {
            //Use preset seed value from inspector
            mapSeed = presetSeed;
        }
    }
    //Returns random prefab room from list
    public GameObject ReturnRandomPrefab()
    {
        return gridPrefabArray[UnityEngine.Random.Range(0, gridPrefabArray.Length)];
    }
    //Cycle through rows and columns and create random room (ReturnRandomPrefab)
    // at each position.
    public void GenerateGrid()
    {
        gridGenerated = false;
        //Set Random's initial state to chosen map seed, so designer can choose type of random behavior
        //  Use UnityEngine.Random because we are using System library and they both have definitions
        //    for random.
        UnityEngine.Random.InitState(mapSeed);
        //Clear previous grid before starting function
        grid = new Room[columns, rows];
        //For each row
        for(int i = 0; i < rows; i++)
        {
            //For each column of that row
            for(int j = 0; j < columns; j++)
            {
                //Figure out spawn location
                //Multiply each room's dimension by the current row/column number
                //
                float xPosition = roomWidth * j;
                float yPosition = roomHeight * i;
                Vector3 newPosition = new Vector3(xPosition, 0f, yPosition);

                //Create grid object at location supplied by newPosition
                GameObject tempRoomObject = Instantiate(ReturnRandomPrefab(), newPosition, Quaternion.identity) as GameObject;

                //Set the object's parent
                tempRoomObject.transform.parent = this.transform;

                //Give meaningful name for inspector
                tempRoomObject.name = "Room_#"+ j + i;

                //Get room component from temporary object
                Room tempRoom = tempRoomObject.GetComponent<Room>();

                //Set tank spawn point at (0,0,0) value of each room
                Instantiate(tankSpawnPoint, newPosition, Quaternion.identity);

                //Set power up spawn point to (0,0,5) units ahead of tank start point
                Instantiate(powerupSpawner, newPosition + Vector3.forward * 3f, Quaternion.identity);              

                //Open doors depending on room's location (rows)
                if(i == 0)
                {
                    //If on bottom row (i=0), open north door
                    tempRoom.doorNorth.SetActive(false);
                    //Also enable tree scenery (south)
                    tempRoom.treeScenerySouth.SetActive(true);
                }
                else if(i == (rows - 1))
                {
                    //If on top row (i = rows.length - 1), open south door
                    tempRoom.doorSouth.SetActive(false);
                    //Also enable tree scenery (north)
                    tempRoom.treeSceneryNorth.SetActive(true);
                }
                else
                {
                    //Otherwise, we are in middle of grid, open both
                    tempRoom.doorSouth.SetActive(false);
                    tempRoom.doorNorth.SetActive(false);
                }

                //Open doors depending on room's location (columns)
                if(j == 0)
                {
                    //If on first column (j=0), open east door
                    tempRoom.doorEast.SetActive(false);
                    //Also enable tree scenery (west)
                    tempRoom.treeSceneryWest.SetActive(true);
                }
                else if(j == (columns - 1))
                {
                    //If on last column (j = rows.length - 1), open west door
                    tempRoom.doorWest.SetActive(false);
                    //Also enable tree scenery (east)
                    tempRoom.treeSceneryEast.SetActive(true);
                }
                else
                {
                    //Otherwise, we are in middle of grid, open both
                    tempRoom.doorEast.SetActive(false);
                    tempRoom.doorWest.SetActive(false);
                }

                //Save current room in loop to array
                grid[j, i] = tempRoom;
            }
        }
        gridGenerated = true;
    }
    public void DestroyGrid()
    {
        foreach (GameObject powerupSpawner in gameManager.powerupSpawnerArray)
        {
            Destroy(powerupSpawner);
        }
        GameObject[] TankSpawnPoints = GameObject.FindGameObjectsWithTag("TankSpawnPoint");
        foreach (GameObject spawnPoint in TankSpawnPoints)
        {
            Destroy(spawnPoint);
        }
        GameObject[] roomArray = GameObject.FindGameObjectsWithTag("Room");
        foreach (GameObject room in roomArray)
        {
            Destroy(room);
        }
        GameObject[] activePowerupArray = GameObject.FindGameObjectsWithTag("Powerup");
        foreach (GameObject powerup in activePowerupArray)
        {
            Destroy(powerup);
        }
    }
    public int DateToInt(DateTime dateUsed)
    {
        //Add up current date and return as integer
        int date = (dateUsed.Year + dateUsed.Month + dateUsed.Day + dateUsed.Hour + 
            dateUsed.Minute + dateUsed.Second + dateUsed.Millisecond);
        return date;
    }
}
