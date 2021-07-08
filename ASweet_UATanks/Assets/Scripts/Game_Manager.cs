using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    //Static variable (same across all class instances)
    public static Game_Manager GMInstance;
    public GameObject playerOneRef;
    public GameObject playerTwoRef;
    public GameObject gameOverUI;
    public TankData playerTankData;
    public TankData cowardTankData;
    public TankData hunterTankData;
    public TankData patrolTankData;
    public TankData bomberTankData;
    public TankShoot playerShootRef;
    public MapGenerator mapGenerator;
    public SpawnTanks spawnTanks;
    public UIManager uiManager;
    public AudioManager audioManager;
    public GameObject[] powerupSpawnerArray;
    public GameObject[] activeAIArray;
    public bool playerOneFiredShellRef = false;
    public bool playerTwoFiredShellRef = false;

    //Awake is called when object is first created, before start calls
    void Awake()
    {
        //Store instance of game manager, which is this script itself
        if(GMInstance == null)
        {
            GMInstance = this;
        }
        if(mapGenerator == null)
        {
            mapGenerator = gameObject.GetComponent<MapGenerator>();
        }
        if(spawnTanks == null)
        {
            spawnTanks = gameObject.GetComponent<SpawnTanks>();
        }
        if(uiManager == null)
        {
            uiManager = gameObject.GetComponent<UIManager>();
        }
        //If game manager already exists, destroy object this script is attached to.
        //  Makes sure nobody can create another instance of GameManager by accident
        else
        {
            Debug.LogError("Error: Only one instance of GameManager can exist");
            Destroy(gameObject);
        }
    }
    void Update()
    {
        playerOneFiredShellRef = playerShootRef.playerOneFiredShell;
        playerTwoFiredShellRef = playerShootRef.playerTwoFiredShell;
        //If player is destroyed, game over screen. Or, alternatively, can 
        // decrement lives/score here.
        //Can also keep track of player lives, and if 0, enable game over

    }
    //Depending on options selected...
    //Generate map, spawn tanks, disable UI
    public void PlayGame()
    {
        //Generate grid before game can start
        mapGenerator.GenerateGrid();
        //If grid is generated (spawnpoint objects exist, can search)
        if(mapGenerator.gridGenerated)
        {
            spawnTanks.SpawnAtRandomSpawnpoint();
            powerupSpawnerArray = GameObject.FindGameObjectsWithTag("PowerupSpawner");
            audioManager.EnableGameMusic();
            activeAIArray = GameObject.FindGameObjectsWithTag("EnemyTank");
        }
    }
    //Enable game over UI screen
    // Set time scale to 0 so game freezes / nothing going on behind UI
    void EnableGameOver()
    {
        Time.timeScale = 0f;
        gameOverUI.SetActive(true);
    }
}
