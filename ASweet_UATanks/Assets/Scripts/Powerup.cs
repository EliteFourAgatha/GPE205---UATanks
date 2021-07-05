using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//See custom class details in inspector
[System.Serializable]

//Regular class that holds information to be passed to PowerupController
//  *Not a component attached to a game object*
public class Powerup
{
    public float speedModifier;
    public float damageModifier;
    public bool isHealthHeart;
    public float fireRateModifier;
    public bool isArmorModifier;
    //General powerup duration
    public float duration;
    public bool isPermanent;
    public void OnActivate(TankData target)
    {
        if(isHealthHeart)
        {
            TankHealth health = target.gameObject.GetComponent<TankHealth>();
            health.currentHealth = health.maxHealth;
        }
        if(isArmorModifier)
        {
            target.armorResetTimer = 5f;
        }
        target.moveSpeed += speedModifier;
        target.shellDamage += damageModifier;
        target.shootReloadTimer -= fireRateModifier;
    }
    public void OnDeactivate(TankData target)
    {
        target.moveSpeed -= speedModifier;
        target.shellDamage -= damageModifier;
        target.shootReloadTimer += fireRateModifier;
    }
}
