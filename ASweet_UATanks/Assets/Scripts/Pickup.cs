using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is attached to Pickup Prefab objects, in order to call OnTriggerEnter
// upon player or enemy tank colliding with it
public class Pickup : MonoBehaviour
{
    public Powerup powerup;
    public AudioClip goodFeedback;
    public AudioClip badFeedback;
    public AudioSource audioSource;
    public Transform tfRef;
    public bool isGoodPowerup;
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if(tfRef == null)
        {
            tfRef = gameObject.GetComponent<Transform>();
        }
    }
    //On trigger enter called when other gameobject collides with 
    // gameobject this script is attached to 
    public void OnTriggerEnter(Collider other)
    {
        //Variable to store other's PowerupController... if it has one
        PowerupController powController = other.GetComponent<PowerupController>();

        //If other gameobject has powerup controller attached...
        if(powController != null)
        {
            //Add powerup
            powController.Add(powerup);

            if(isGoodPowerup)
            {
                AudioSource.PlayClipAtPoint(goodFeedback, tfRef.position, 1f);
            }
            else
            {
                AudioSource.PlayClipAtPoint(badFeedback, tfRef.position, 1f);
            }
        }
        //Destroy pickup object
        Destroy(gameObject);
    }
}
