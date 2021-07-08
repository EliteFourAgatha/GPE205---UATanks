using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject optionsMenu;
    public GameObject gameOverUI;
    public Text scoreText;
    public Text highScoreText;
    public MapGenerator mapGenerator;
    public Game_Manager gameManager;
    public SpawnTanks spawnTanks;
    public Slider sfxSlider;
    public Slider musicSlider;
    private float sfxSliderValue;
    private float musicSliderValue;
    void Start()
    {
        if(mapGenerator == null)
        {
            mapGenerator = gameObject.GetComponent<MapGenerator>();
        }
        if(gameManager == null)
        {
            gameManager = gameObject.GetComponent<Game_Manager>();
        }
        if(spawnTanks == null)
        {
            spawnTanks = gameObject.GetComponent<SpawnTanks>();
        }
        LoadPlayerPrefsForOptionsMenu();
    }
    //Play game button
    public void PlayGame()
    {
        startMenu.SetActive(false);
        gameManager.PlayGame();
    }
    //Options menu button
    public void EnableOptionsMenu()
    {
        startMenu.SetActive(false);
        optionsMenu.SetActive(true);
        sfxSlider.value = sfxSliderValue;
        musicSlider.value = musicSliderValue;
    }
    //Options menu => start menu Button
    public void EnableStartMenu()
    {
        optionsMenu.SetActive(false);
        gameOverUI.SetActive(false);
        startMenu.SetActive(true);
    }
    //Options Menu, choose map (random seed) type
    public void ChooseMapOfDay()
    {
        mapGenerator.randomSpawnType = MapGenerator.RandomSpawnType.mapOfDay;
        PlayerPrefs.SetString("randomMode", "mapOfDay");
    }
    public void ChooseRandomMap()
    {
        mapGenerator.randomSpawnType = MapGenerator.RandomSpawnType.random;
        PlayerPrefs.SetString("randomMode", "random");
    }
    //Options Menu Button
    public void ChooseSinglePlayer()
    {
        spawnTanks.singlePlayerEnabled = true;
        spawnTanks.multiPlayerEnabled = false;
        PlayerPrefs.SetString("numPlayersGameMode", "single");
    }
    //Options Menu Button
    public void ChooseMultiPlayer()
    {
        spawnTanks.multiPlayerEnabled = true;
        spawnTanks.singlePlayerEnabled = false;
        PlayerPrefs.SetString("numPlayersGameMode", "multi");
    }
    //Load all options data stored in player prefs.
    // Called in start function of UIManager. Will load all previous session options data
    //  Player prefs are stored locally and persist between sessions/scene changes
    public void LoadPlayerPrefsForOptionsMenu()
    {
        sfxSliderValue = PlayerPrefs.GetFloat("sfxVolume");
        musicSliderValue = PlayerPrefs.GetFloat("musicVolume");

        string randomMode = PlayerPrefs.GetString("randomMode");
        if(randomMode == "mapOfDay")
        {
            mapGenerator.randomSpawnType = MapGenerator.RandomSpawnType.mapOfDay;
        }
        else if(randomMode == "random")
        {
            mapGenerator.randomSpawnType = MapGenerator.RandomSpawnType.random;
        }
        else
        {
            mapGenerator.randomSpawnType = MapGenerator.RandomSpawnType.presetSeed;
        }
        
        string numPlayerMode = PlayerPrefs.GetString("numPlayersGameMode");
        if(numPlayerMode == "single")
        {
            spawnTanks.singlePlayerEnabled = true;
        }
        else if(numPlayerMode == "multi")
        {
            spawnTanks.multiPlayerEnabled = true;
        }
        //If numPlayersGameMode = null...
        else
        {
            spawnTanks.singlePlayerEnabled = true;
            PlayerPrefs.SetString("numPlayersGameMode", "single");
        }
    }
    public void QuitGame()
    {
        //save high score etc. with player prefs?
        Application.Quit();
    }
}
