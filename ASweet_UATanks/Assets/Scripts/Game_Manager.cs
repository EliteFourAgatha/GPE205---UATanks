using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    //Static variable (same across all class instances)
    public static Game_Manager GMInstance;
    //Awake is called when object is first created, before start calls
    public void Awake()
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
}
