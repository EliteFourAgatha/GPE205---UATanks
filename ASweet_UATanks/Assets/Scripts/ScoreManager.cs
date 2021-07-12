using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text gameOverHighScoreText;
    public Text inGameHighScoreText;
    public Text playerOneScoreText;
    public Text playerTwoScoreText;
    public int highScore;
    private int playerOneScore;
    private int playerTwoScore;
    void Start()
    {
        if(PlayerPrefs.HasKey("HighScore"))
        {
            highScore = PlayerPrefs.GetInt("HighScore");
        }
        else
        {
            highScore = 0;
        }
        playerOneScore = 0;
        playerTwoScore = 0;
    }
    void Update()
    {
        CheckForNewHighScore();
        if(gameOverHighScoreText.IsActive())
        {
            gameOverHighScoreText.text = highScore.ToString();
        }
        if(inGameHighScoreText.IsActive())
        {
            inGameHighScoreText.text = "HIGH SCORE: " + highScore.ToString();
        }
        if(playerOneScoreText.IsActive())
        {
            playerOneScoreText.text = "Score: " + playerOneScore;
        }
        if(playerTwoScoreText.IsActive())
        {
            playerTwoScoreText.text = "Score: " + playerTwoScore;
        }
    }
    public void CheckForNewHighScore()
    {
        if(playerOneScore > highScore)
        {
            highScore = playerOneScore;
        }
        if(playerTwoScore > highScore)
        {
            highScore = playerTwoScore;
        }
    }
    public void ResetCurrentScores()
    {
        playerOneScore = 0;
        playerTwoScore = 0;
    }
    public void AddPoint(int playerNum)
    {
        if(playerNum == 1)
        {
            playerOneScore ++;
        }
        else if(playerNum == 2)
        {
            playerTwoScore ++;
        }
    }
    public void EnableScoreText(int playerNum)
    {
        if(playerNum == 1)
        {
            playerOneScoreText.gameObject.SetActive(true);
        }
        else if(playerNum == 2)
        {
            playerOneScoreText.gameObject.SetActive(true);
            playerTwoScoreText.gameObject.SetActive(true);
        }
    }
    //Disable score text UI objects (for when one player game over and the other still playing),
    // also for gameover screen
    public void DisableScoreText(int playerNum)
    {
        if(playerNum == 1)
        {
            playerOneScoreText.gameObject.SetActive(false);
        }
        else if(playerNum == 2)
        {
            playerTwoScoreText.gameObject.SetActive(false);
        }
    }
    //Enable or disable in-game high score text
    public void ChangeHighScoreToStatus(bool choice)
    {
        inGameHighScoreText.gameObject.SetActive(choice);
    }
    public void SaveHighScoreToPlayerPrefs()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }
}
