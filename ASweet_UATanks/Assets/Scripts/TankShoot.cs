using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RequireComponent tells Unity that a TankData component needs to be added
//  to any game object this script is attached to
[RequireComponent (typeof(TankData))]
public class TankShoot : MonoBehaviour
{
    //Store transform in a variable.
    // This avoids calling GameObject.GetComponent() multiple times, which is slow
    public Transform firePointTF;
    //Audio source for tank blast SFX
    public AudioSource audioSource;
    public bool playerFiredShell = false;
    public TankData data;
    //Float timer to keep track of Time.time when shell is fired
    private float fireTimer;

    //Reference to tank shell prefab
    public Rigidbody shellRound;
    //Bool that must be true to call FireShell()
    public bool canFire;
    void Start()
    {
        //CanFire is true when game first starts
        canFire = true;
        //If components aren't set in inspector, set with GetComponent
        if(audioSource == null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
        }
        if(data == null)
        {
            data = gameObject.GetComponent<TankData>();
        }
    }

    void Update()
    {
        //Check if (data.shootReloadTimer value) seconds have passed since last FireShell() call,
        //  which is tracked by fireTimer
        if(Time.time - fireTimer > data.shootReloadTimer)
        {
            canFire = true;
        }
    }
    public void FireShell()
    {
        Rigidbody shell;
        //Instantiate shell at firepoint, which is transform child of Tank (located at barrel)
        shell = Instantiate(shellRound, firePointTF.position, firePointTF.rotation);
        //Use AddForce to move shell in direction tank is currently facing (forward)
        //  ForceMode.Impulse applies instant force
        shell.AddForce(firePointTF.forward * data.shellSpeed, ForceMode.Impulse);
        //Keep track of Time.time when shot is fired for reload timer
        fireTimer = Time.time;
        audioSource.Play();
        //Set bool to false when shell is fired. Set to true once reload timer is up
        canFire = false;
        //Tells AI that player just called FireShell(). Can add other tags here for
        //  multiplayer (multiple players to check)
        if(gameObject.tag == "PlayerTank")
        {
            playerFiredShell = true;
            StartCoroutine(DisableFiredShellBool());
        }
    }
    IEnumerator DisableFiredShellBool()
    {
        yield return new WaitForSeconds(2f);
        playerFiredShell = false;
    }
}
