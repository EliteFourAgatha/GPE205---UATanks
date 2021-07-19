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
    public Transform firePointTFTwo;
    public Transform firePointTFThree;
    //Audio source for tank blast SFX
    public AudioSource audioSource;
    public AudioClip bombDropSFX;
    public AudioClip tankBlastSFX;
    public bool playerOneFiredShell = false;
    public bool playerTwoFiredShell = false;
    public TankData data;
    //Float timer to keep track of Time.time when shell is fired
    private float fireTimer;

    //Reference to tank shell prefab
    public Rigidbody shellRound;
    //Reference to tank bomb prefab
    public Rigidbody bombRound;
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
        if(gameObject.name == "BomberAI")
        {
            audioSource.clip = bombDropSFX;
        }
        else
        {
            audioSource.clip = tankBlastSFX;
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
    //Take numberOfShells as parameter for triple shot powerup
    public void FireShell()
    {
        Rigidbody shell;

        //Instantiate shell at firepoint, which is transform child of Tank (located at barrel)
        shell = Instantiate(shellRound, firePointTF.position, firePointTF.rotation);
        //Tells AI that player just called FireShell() (for coward script).
        // Also tells shell script which tank fired it for score calculation
        if(gameObject.tag == "PlayerOneTank")
        {
            ShellScript shellScript = shell.GetComponent<ShellScript>();
            shellScript.tankThatFiredShell = ShellScript.TankThatFiredShell.playerOne;
            playerOneFiredShell = true;
            StartCoroutine(DisableFiredShellBool());
        }
        else if(gameObject.tag == "PlayerTwoTank")
        {
            ShellScript shellScript = shell.GetComponent<ShellScript>();
            shellScript.tankThatFiredShell = ShellScript.TankThatFiredShell.playerTwo;
            playerTwoFiredShell = true;
            StartCoroutine(DisableFiredShellBool());
        }
        else if(gameObject.tag == "BomberAI")
        {
            ShellScript shellScript = shell.GetComponent<ShellScript>();
            shellScript.tankThatFiredShell = ShellScript.TankThatFiredShell.bomberAI;
            StartCoroutine(DisableFiredShellBool());
        }
        //Use AddForce to move shell in direction tank is currently facing (forward)
        //  ForceMode.Impulse applies instant force
        shell.AddForce(firePointTF.forward * data.shellSpeed, ForceMode.Impulse);

        //Keep track of Time.time when shot is fired for reload timer
        fireTimer = Time.time;
        audioSource.Play();
        //Set bool to false when shell is fired. Set to true once reload timer is up
        canFire = false;
    }
    public void DropBomb(Vector3 dropPoint)
    {
        Rigidbody bomb;
        //Instantiate shell above dropPoint, which is provided by AIBomber script
        bomb = Instantiate(bombRound, dropPoint + Vector3.up * 4, Quaternion.Euler(0, 0, 0));
        //Keep track of Time.time when shot is fired for reload timer
        fireTimer = Time.time;
        audioSource.Play();
        //Set bool to false when shell is fired. Set to true once reload timer is up
        canFire = false;
    }
    IEnumerator DisableFiredShellBool()
    {
        yield return new WaitForSeconds(2f);
        playerOneFiredShell = false;
        playerTwoFiredShell = false;
    }
}
