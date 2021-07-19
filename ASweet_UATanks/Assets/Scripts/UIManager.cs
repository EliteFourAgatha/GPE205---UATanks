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
    public AudioManager audioManager;
    public ScoreManager scoreManager;
    public LifeManager lifeManager;
    public Slider sfxSlider;
    public Slider musicSlider;
    private float sfxSliderValue;
    private float musicSliderValue;
    void Awake() 
    {
        LoadPlayerPrefs();
    }
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
        if(scoreManager == null)
        {
            scoreManager = gameObject.GetComponent<ScoreManager>();
        }
        if(lifeManager == null)
        {
            lifeManager = gameObject.GetComponent<LifeManager>();
        }
    }
    //Options menu button
    public void EnableOptionsMenu()
    {
        startMenu.SetActive(false);
        optionsMenu.SetActive(true);
        LoadPlayerPrefs();
        sfxSlider.value = sfxSliderValue;
        musicSlider.value = musicSliderValue;
    }
    //Options menu => start menu Button
    public void EnableStartMenu()
    {
        optionsMenu.SetActive(false);
        scoreManager.DisableScoreText(2);
        scoreManager.DisableScoreText(1);
        gameOverUI.SetActive(false);
        audioManager.EnableMenuMusic();
        startMenu.SetActive(true);
    }
    //Options Menu, choose map (random seed) type
    public void ChooseMapOfDay()
    {
        mapGenerator.randomSpawnType = MapGenerator.RandomSpawnType.mapOfDay;
        PlayerPrefs.SetString("randomMode", "mapOfDay");
        PlayerPrefs.Save();
    }
    public void ChooseRandomMap()
    {
        mapGenerator.randomSpawnType = MapGenerator.RandomSpawnType.random;
        PlayerPrefs.SetString("randomMode", "random");
        PlayerPrefs.Save();
    }
    //Options Menu Button
    public void ChooseSinglePlayer()
    {
        spawnTanks.singlePlayerEnabled = true;
        spawnTanks.multiPlayerEnabled = false;
        PlayerPrefs.SetString("numPlayers", "single");
        PlayerPrefs.Save();
    }
    //Options Menu Button
    public void ChooseMultiPlayer()
    {
        spawnTanks.multiPlayerEnabled = true;
        spawnTanks.singlePlayerEnabled = false;
        PlayerPrefs.SetString("numPlayers", "multi");
        PlayerPrefs.Save();
    }
    //Load all options data stored in player prefs.
    // Called in start function of UIManager. Will load all previous session options data
    //  Player prefs are stored locally and persist between sessions/scene changes
    public void LoadPlayerPrefs()
    {
        string randomMode;
        if(PlayerPrefs.HasKey("sfxVolume"))
        {
            sfxSliderValue = PlayerPrefs.GetFloat("sfxVolume");
            sfxSliderValue = Mathf.Pow(10, (sfxSliderValue / 20));
            audioManager.masterAudioMixer.SetFloat("sfxVolume", Mathf.Log(sfxSliderValue) * 20);
        }
        else
        {
            sfxSliderValue = sfxSlider.maxValue;
        }
        if(PlayerPrefs.HasKey("musicVolume"))
        {
            musicSliderValue = PlayerPrefs.GetFloat("musicVolume");
            musicSliderValue = Mathf.Pow(10, (musicSliderValue / 20));
            audioManager.masterAudioMixer.SetFloat("musicVolume", Mathf.Log(musicSliderValue) * 20);
        }
        else
        {
            musicSliderValue = musicSlider.maxValue;
        }
        if(PlayerPrefs.HasKey("randomMode"))
        {
            randomMode = PlayerPrefs.GetString("randomMode");
        }
        else
        {
            randomMode = "presetSeed";
        }

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
        
        if(PlayerPrefs.HasKey("numPlayers"))
        {
            string numPlayerMode = PlayerPrefs.GetString("numPlayers");
            Debug.Log("num players loaded from player prefs: " + numPlayerMode);
            if(numPlayerMode == "single")
            {
                spawnTanks.singlePlayerEnabled = true;
            }
            else if(numPlayerMode == "multi")
            {
                spawnTanks.multiPlayerEnabled = true;
            }
        }
        //If numPlayersGameMode = null..
        else
        {
            spawnTanks.singlePlayerEnabled = true;
            PlayerPrefs.SetString("numPlayers", "single");
            PlayerPrefs.Save();
        }
    }
    public void QuitGame()
    {
        scoreManager.SaveHighScoreToPlayerPrefs();
        PlayerPrefs.Save();
        Application.Quit();
    }
}
