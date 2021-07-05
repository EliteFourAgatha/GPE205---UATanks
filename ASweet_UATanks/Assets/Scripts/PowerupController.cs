using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RequireComponent tells Unity that a TankData component needs to be added
//  to any game object this script is attached to
[RequireComponent (typeof(TankData))]

//This class is used as a component, and is attached to player and enemy tanks. This class allows
// ability to add powerup to list (activate it, use effects), as well as periodically check for
public class PowerupController : MonoBehaviour
{
    //Dynamic array (list) to hold current powerups
    public List<Powerup> powerupList;
    public TankData data;
    void Start()
    {
        //Initialize list
        powerupList = new List<Powerup>();
        if(data == null)
        {
            data = gameObject.GetComponent<TankData>();
        }
    }
    void Update()
    {
        CheckForExpiredPowerups();
    }
    public void Add(Powerup powerup)
    {
        powerup.OnActivate(data);
        //Only add permanent powerups to list
        if(!powerup.isPermanent)
        {
            powerupList.Add(powerup);
        }
    }
    public void CheckForExpiredPowerups()
    {
        //Dynamic array (list) to store expired powerups to be removed
        List<Powerup> expiredPowerups = new List<Powerup>();
        //Iterate through list of powerups. Update timer.
        // If timer <= 0, add to expiredPowerup list, which will be removed after loop
        foreach(Powerup power in powerupList)
        {
            //Subtract from duration timer
            power.duration -= Time.deltaTime;
            //Assemble list of expired powerups
            if(power.duration <= 0)
            {
                expiredPowerups.Add(power);
            }
        }
        //Use created list of expired powerups to remove expired ones
        foreach(Powerup power in expiredPowerups)
        {
            Debug.Log(powerupList);
            
            power.OnDeactivate(data);
            powerupList.Remove(power);
        }
        //Because expiredPowerups is local list, it is destroyed when function ends.
        //  Call clear to learn how to empty list
        expiredPowerups.Clear();
    }
}
