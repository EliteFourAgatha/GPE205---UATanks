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
    public LifeManager lifeManager;
    public Game_Manager gameManager;
    public SpawnTanks spawnTanks;
    public AudioSource audioSource;
    public AudioClip fireShellSFX;
    public AudioClip explosionSFX;
    public GameObject explosionVFX;
    public TankData tankData;
    public bool canTakeDamage;
    public bool canDestroy;
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

        spawnTanks = GameObject.FindGameObjectWithTag("GameController").GetComponent<SpawnTanks>();

        maxHealth = tankData.maxHealth;
        //Set current health = max health on start
        currentHealth = maxHealth;
        //Cantakedamage is true on start
        canTakeDamage = true;
        canDestroy = true;
    }
    void Update()
    {
        if(currentHealth <= 0) // && canDestroy == true... necessary??
        {
            Debug.Log("Destroyed tank, less than 0 hp");
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
        if(gameObject != null)
        {
            AudioSource.PlayClipAtPoint(explosionSFX, trfRef.position, 0.5f);
        }
        audioSource.clip = fireShellSFX;
        Instantiate(explosionVFX, trfRef.position, Quaternion.identity);
        if(gameObject.tag == "PlayerOneTank")
        {
            lifeManager.DecreasePlayerLife(1);
            //canDestroy = false;
            gameObject.SetActive(false);
        }
        else if(gameObject.tag == "PlayerTwoTank")
        {
            lifeManager.DecreasePlayerLife(2);
            //canDestroy = false;
            gameObject.SetActive(false);
        }
        else
        {
            spawnTanks.WaitAndRespawnEnemy(gameObject);
            ResetHealth();
            //canDestroy = false;
            gameObject.SetActive(false);
        }
    }
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
    //Call onenable when gameobject setactive(true). Use to reset health to maximum
    public void OnEnable() 
    {
        ResetHealth();
        canDestroy = true;
        Debug.Log("can destroy = true");
    }
}
