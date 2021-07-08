using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lives : MonoBehaviour
{
    public Text playerOneLivesText;
    public Text playerTwoLivesText;
    public GameObject playerOneGameOverUI;
    public GameObject playerOneTank;
    public GameObject playerTwoGameOverUI;
    public GameObject playerTwoTank;
    public Game_Manager gameManager;
    public int maxLives = 3;
    int livesRemaining;
    void Start()
    {
        livesRemaining = maxLives;
    }
    private void Update()
    {
        if(livesRemaining == 0)
        {
            if(gameObject.name == "PlayerOneTank")
            {
                playerOneGameOverUI.SetActive(true);
                playerOneTank.SetActive(false);
            }
            else if(gameObject.name == "PlayerTwoTank")
            {
                playerTwoGameOverUI.SetActive(true);
                playerTwoTank.SetActive(false);
            }
        }
    }
    public void LoseLife()
    {
        livesRemaining --;
    }
}
