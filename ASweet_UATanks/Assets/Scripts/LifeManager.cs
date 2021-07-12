using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    public Text playerOneLivesText;
    public Text playerTwoLivesText;
    public GameObject playerOneGameOverUI;
    public GameObject playerOneTank;
    public GameObject playerTwoGameOverUI;
    public GameObject playerTwoTank;
    public GameObject gameOverUI;
    public Game_Manager gameManager;
    public ScoreManager scoreManager;
    public SpawnTanks spawnTanks;
    public int maxLives = 1;
    private int playerOneLives;
    private int playerTwoLives;
    void Start()
    {
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
        playerOneLives = maxLives;
        playerTwoLives = maxLives;
    }
    void Update()
    {
        CheckLivesStatus();
        if(playerOneLivesText.IsActive())
        {
            playerOneLivesText.text = ("Lives: " + playerOneLives.ToString());
        }
        if(playerTwoLivesText.IsActive())
        {
            playerTwoLivesText.text = ("Lives: " + playerTwoLives.ToString());
        }
    }
    public void DecreasePlayerLife(int playerNum)
    {
        if(playerNum == 1)
        {
            playerOneLives --;
            Debug.Log("player one lives:" + playerOneLives);
            if(playerOneLives > 0)
            {
                if(spawnTanks.singlePlayerEnabled)
                {
                    spawnTanks.RespawnPlayer(1);
                }
                else if(spawnTanks.multiPlayerEnabled)
                {
                    spawnTanks.RespawnPlayer(1);
                }
            }
        }
        else if(playerNum == 2)
        {
            playerTwoLives --;
            if(playerTwoLives > 0)
            {
                spawnTanks.RespawnPlayer(2);
            }
        }
    }
    public void CheckLivesStatus()
    {
        //If single player...
        if (PlayerPrefs.GetString("numPlayers") == "single")
        {
            if(playerOneLives <= 0)
            {
                playerOneLivesText.gameObject.SetActive(false);
                playerOneGameOverUI.SetActive(false);
                scoreManager.DisableScoreText(1);
                playerOneTank.SetActive(false);
                scoreManager.ChangeHighScoreToStatus(false);
                gameOverUI.SetActive(true);
                ResetLives(1);
                Time.timeScale = 0f;
            }
        }
        //If multi player...
        if (PlayerPrefs.GetString("numPlayers") == "multi")
        {
            //If player one out of lives..
            if(playerOneLives <= 0)
            {
                //If player two out of lives as well..
                if(playerTwoLives <= 0)
                {
                    //Enable game over screen
                    playerOneLivesText.gameObject.SetActive(false);
                    playerTwoLivesText.gameObject.SetActive(false);;
                    playerOneGameOverUI.SetActive(false);
                    playerTwoGameOverUI.SetActive(false);
                    scoreManager.DisableScoreText(1);
                    scoreManager.DisableScoreText(2);
                    playerOneTank.SetActive(false);
                    playerTwoTank.SetActive(false);
                    scoreManager.ChangeHighScoreToStatus(false);
                    gameOverUI.SetActive(true);
                    ResetLives(1);
                    ResetLives(2);
                    Time.timeScale = 0f;
                }
                else
                {
                    playerOneLivesText.gameObject.SetActive(false);
                    playerOneGameOverUI.SetActive(true);
                    scoreManager.DisableScoreText(1);
                    playerOneTank.SetActive(false);
                }
            }
            //If player two out of lives..
            if(playerTwoLives <= 0)
            {
                Debug.Log("player two game over");
                playerTwoLivesText.gameObject.SetActive(false);
                playerTwoGameOverUI.SetActive(true);
                scoreManager.DisableScoreText(2);
                playerTwoTank.SetActive(false);
            }
        }
    }
    public void EnableLivesText(int playerNum)
    {
        if(playerNum == 1)
        {
            playerOneLivesText.gameObject.SetActive(true);
        }
        else if(playerNum == 2)
        {
            playerOneLivesText.gameObject.SetActive(true);
            playerTwoLivesText.gameObject.SetActive(true);
        }
    }
    public void ResetLives(int playerNum)
    {
        if(playerNum == 1)
        {
            playerOneLives = maxLives;
        }
        else if(playerNum == 2)
        {
            playerTwoLives = maxLives;
        }
    }
}
