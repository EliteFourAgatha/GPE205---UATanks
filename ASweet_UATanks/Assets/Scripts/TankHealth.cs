using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RequireComponent tells Unity that a TankData component needs to be added
//  to any game object this script is attached to
[RequireComponent (typeof(TankData))]
public class TankHealth : MonoBehaviour
{
    public Transform trfRef;
    //Floats for max and current health to determine if tank should be destroyed on collision
    public float maxHealth;
    public float currentHealth;
    private float damageValue;
    public Game_Manager gameManager;
    public AudioSource audioSource;
    public AudioClip explosionSFX;
    public GameObject explosionVFX;
    public TankData tankData;
    public bool canTakeDamage;
    void Start()
    {
        //If tank data isn't set in inspector, set it with GetComponent
        if(tankData == null)
        {
            tankData = gameObject.GetComponent<TankData>();
        }
        if(trfRef == null)
        {
            trfRef = gameObject.GetComponent<Transform>();
        }
        audioSource = gameObject.GetComponent<AudioSource>();

        maxHealth = tankData.maxHealth;
        //Set current health = max health on start
        currentHealth = maxHealth;
        //Cantakedamage is true on start
        canTakeDamage = true;
    }
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
            currentHealth -= tankData.shellDamage;
        }
    }
    public void DestroyTank()
    {
        audioSource.clip = explosionSFX;
        AudioSource.PlayClipAtPoint(explosionSFX, trfRef.position);
        Instantiate(explosionVFX, trfRef.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
