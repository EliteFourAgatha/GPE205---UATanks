using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RequireComponent tells Unity that a TankData component needs to be added
//  to any game object this script is attached to
[RequireComponent (typeof(TankData))]
public class Armor : MonoBehaviour
{
    public TankData data;
    public TankHealth tankHealth;
    public GameObject tankArmor;
    public bool canUseArmor;
    //Float value to keep track of reload timer (time until you can use again)
    private float armorReloadTimer;
    //Float value to keep track of armor duration
    private float armorDuration;
    void Start()
    {
        canUseArmor = true;
    }
    void Update()
    {
        //Check if (data.armorDurationTime) has expired. If so, turn off armor in hierarchy
        //  and set tankHealth.canTakeDamage back to true
        if(Time.time - armorDuration > data.armorDurationTime)
        {
            tankHealth.canTakeDamage = true;
            tankArmor.SetActive(false);
        }
        //Check if (data.armorResetTimer value) seconds have passed since last ActivateArmor() call,
        //  which is tracked by armorTimer
        if(Time.time - armorReloadTimer > data.armorResetTimer)
        {
            canUseArmor = true;
        }
    }
    //Activate armor, setting it active in hierarchy. Set TankHealth.canTakeDamage to false
    public void ActivateArmor()
    {
        tankHealth.canTakeDamage = false;
        tankArmor.SetActive(true);
        armorReloadTimer = Time.time;
        armorDuration = Time.time;
        canUseArmor = false;
    }
}
