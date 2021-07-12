using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankData : MonoBehaviour
{
    //Movespeed for MoveTank in meters/second
    public float moveSpeed = 10f;
    //Movespeed for AIPatrol tank in meters/second
    public float aiPatrolMoveSpeed = 1.5f;
    //Euler angle value for RotateTank in degrees/second
    public float turnSpeed = 180f;
    //Float value for AIController RunFleeMode() distance
    public float fleeDistance = 5f;
    //Float value for distance in front of tank to aim at for AI Hunt mode
    public float huntDistance = 1f;
    //Float value for AIController Coward distance
    public float cowardFleeTimer = 3f;
    //Float value for TankShoot.FireShell() shell speed
    public float shellSpeed = 50f;
    //Float value for time to reload Armor.ActivateArmor(), in seconds
    public float armorResetTimer = 10f;
    //Float value for length of time armor is active, in seconds
    public float armorDurationTime = 3f;
    //Float value for time to reload TankShoot.FireShell(), in seconds
    public float shootReloadTimer = 3f;
    //Float value for time for hunter AI to reload TankShoot.FireShell(), in seconds
    public float enemyHuntReloadTimer = 1f;
    //Float value for time for bomber AI to reload TankShoot.DropBomb(), in seconds
    public float enemyBombReloadTimer = 5f;
    //TankShoot.FireShell() timeout period, in seconds
    public float timeoutTimer = 5f;
    //TankShoot.FireShell() damage value
    public float shellDamage = 5f;
    //Max health value
    public float maxHealth = 5f;
}
