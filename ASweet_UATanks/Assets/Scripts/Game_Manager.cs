using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    //Static variable (same across all class instances)
    public static Game_Manager GMInstance;
    public GameObject playerOneRef;
    public GameObject gameOverUI;
    public TankData playerTankData;
    public TankData aiOneTankData;
    public TankData aiTwoTankData;
    public TankData aiThreeTankData;
    public TankData aiFourTankData;
    public TankShoot playerShootRef;
    public bool playerFiredShellRef = false;
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
    }
    void Update()
    {
        playerFiredShellRef = playerShootRef.playerFiredShell;
        //If player is destroyed, game over screen. Or, alternatively, can 
        // decrement lives/score here.
        //Can also keep track of player lives, and if 0, enable game over
        if(playerOneRef == null)
        {
            EnableGameOver();
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
