using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RequireComponent tells Unity that a TankData component needs to be added
//  to any game object this script is attached to
[RequireComponent (typeof(TankData))]
public class TankHealth : MonoBehaviour
{
    //Floats for max and current health to determine if tank should be destroyed on collision
    private float maxHealth;
    private float currentHealth;
    private float damageValue;
    public TankData tankData;
    public bool canTakeDamage;
    void Start()
    {
        //If tank data isn't set in inspector, set it with GetComponent
        if(tankData == null)
        {
            tankData = gameObject.GetComponent<TankData>();
        }
        //Check tag. If player, set maxHealth to (playerMaxHealth)
        //  Also set damageValue equal to (enemyShellDamage)

        //If tag is enemy, set maxHealth to (enemyMaxHealth)
        //  Also set damageValue equal to (playerShellDamage)
        if(gameObject.tag == "PlayerTank")
        {
            maxHealth = tankData.playerMaxHealth;
            damageValue = tankData.enemyShellDamage;
        }
        else
        {
            maxHealth = tankData.enemyMaxHealth;
            damageValue = tankData.playerShellDamage;
        }
        //Set current health = max health on start
        currentHealth = maxHealth;
        //Cantakedamage is true on start
        canTakeDamage = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            DestroyTank();
        }
    }
    public void DamageTank()
    {
        if(canTakeDamage == true)
        {
            currentHealth -= damageValue;
        }
    }
    public void DestroyTank()
    {
        //SFX / VFX
        Destroy(gameObject);
    }
}
