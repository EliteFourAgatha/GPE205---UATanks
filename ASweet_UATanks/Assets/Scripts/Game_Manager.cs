using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; //Using Concat to combine arrays

public class Game_Manager : MonoBehaviour
{
    //Static variable (same across all class instances)
    public static Game_Manager GMInstance;
    public GameObject playerOneRef;
    public GameObject playerTwoRef;
    public GameObject gameOverUI;
    public GameObject startMenu;
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
    public LifeManager lifeManager;
    public ScoreManager scoreManager;
    public GameObject[] powerupSpawnerArray;
    private GameObject[] aiArray;
    private GameObject[] bomberAIArray;
    public GameObject[] allActiveAIArray;
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
        //If game manager already exists, destroy object this script is attached to.
        //  Makes sure nobody can create another instance of GameManager by accident
        else
        {
            Debug.LogError("Error: Only one instance of GameManager can exist");
            Destroy(gameObject);
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
        if(lifeManager == null)
        {
            lifeManager = gameObject.GetComponent<LifeManager>();
        }
        if(scoreManager == null)
        {
            scoreManager = gameObject.GetComponent<ScoreManager>();
        }
    }
    void Update()
    {
        playerOneFiredShellRef = playerShootRef.playerOneFiredShell;
        playerTwoFiredShellRef = playerShootRef.playerTwoFiredShell;
    }
    //Depending on options selected...
    //Generate map, spawn tanks, disable UI
    public void PlayGame()
    {
        startMenu.SetActive(false);
        gameOverUI.SetActive(false);
        Time.timeScale = 1f;
        mapGenerator.DestroyGrid();
        //Generate grid before game can start
        mapGenerator.GenerateGrid();
        //If grid is generated (spawnpoint objects exist, can search)
        if(mapGenerator.gridGenerated)
        {
            //Reset game state (single player)
            if(PlayerPrefs.GetString("numPlayers") == "single")
            {
                lifeManager.ResetLives(1);
                lifeManager.EnableLivesText(1);
                scoreManager.ResetCurrentScores();
                scoreManager.EnableScoreText(1);
                spawnTanks.SpawnAtRandomSpawnpoint();
            }
            //Reset game state (multi player)
            else if(PlayerPrefs.GetString("numPlayers") == "multi")
            {
                lifeManager.ResetLives(1);
                lifeManager.ResetLives(2);
                lifeManager.EnableLivesText(2);
                scoreManager.ResetCurrentScores();
                scoreManager.EnableScoreText(2);
                spawnTanks.SpawnAtRandomSpawnpoint();
            }
            powerupSpawnerArray = GameObject.FindGameObjectsWithTag("PowerupSpawner");
            audioManager.EnableGameMusic();
            scoreManager.ChangeHighScoreToStatus(true);
            aiArray = GameObject.FindGameObjectsWithTag("EnemyTank");
            bomberAIArray = GameObject.FindGameObjectsWithTag("BomberAI");
            allActiveAIArray = aiArray.Concat(bomberAIArray).ToArray();
        }
    }
    public void OnApplicationQuit() 
    {
        scoreManager.SaveHighScoreToPlayerPrefs();
    }
}
