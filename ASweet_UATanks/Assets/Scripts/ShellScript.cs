using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellScript : MonoBehaviour
{
    private float startTime;
    //How long to wait to destroy shell if it hasn't collided already
    //  Guarantees it won't linger in game world
    [Range (1, 3)]
    private float destroyTimer = 3f;
    public ScoreManager scoreManager;
    public ParticleSystem roundExplodeVFX;
    public GameObject aoeField;
    public AudioSource audioSource;
    public AudioClip tankHitSFX;
    public enum TankThatFiredShell{bomberAI, enemy, playerOne, playerTwo};
    public TankThatFiredShell tankThatFiredShell;
    void Start()
    {
        if(audioSource == null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
        }
        tankHitSFX = (AudioClip)Resources.Load("tankHitSFX");
        scoreManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScoreManager>();
        startTime = Time.time;
    }

    void Update()
    {
        //If more than (destroyTimer) second have passed since shell was fired, destroy it
        if(Time.time - startTime > destroyTimer)
        {
            Destroy(gameObject);
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        ParticleSystem roundExplosion;
        //If object collided with is Enemy tank, do (data.playerShellDamage) damage
        if(collision.gameObject.tag == "EnemyTank" || collision.gameObject.tag == "BomberAI")
        {
            TankHealth tankHealth = collision.gameObject.GetComponent<TankHealth>();
            tankHealth.DamageTank();
            roundExplosion = Instantiate(roundExplodeVFX, collision.transform.position, Quaternion.Euler(0, 0, 0));
            if(gameObject != null)
            {
                AudioSource.PlayClipAtPoint(tankHitSFX, collision.gameObject.transform.position, 0.5f);
            }
            if(tankThatFiredShell == TankThatFiredShell.playerOne)
            {
                scoreManager.AddPoint(1);
            }
            else if(tankThatFiredShell == TankThatFiredShell.playerTwo)
            {
                scoreManager.AddPoint(2);
            }
            Destroy(gameObject);
        }
        //If object collided with is Player tank, do (data.EnemyShellDamage) damage
        else if (collision.gameObject.tag == "PlayerOneTank" || collision.gameObject.tag == "PlayerTwoTank")
        {
            TankHealth tankHealth = collision.gameObject.GetComponent<TankHealth>();
            tankHealth.DamageTank();
            roundExplosion = Instantiate(roundExplodeVFX, collision.transform.position, Quaternion.Euler(0, 0, 0));
            if(gameObject != null)
            {
                AudioSource.PlayClipAtPoint(tankHitSFX, collision.gameObject.transform.position, 0.5f);
            }
            if(tankThatFiredShell == TankThatFiredShell.playerOne)
            {
                scoreManager.AddPoint(1);
            }
            else if(tankThatFiredShell == TankThatFiredShell.playerTwo)
            {
                scoreManager.AddPoint(2);
            }
                Destroy(gameObject);
        }
        //If bomb collides with wall or ground..
        else
        {
            //If tank that fired was bomber AI...
            if(tankThatFiredShell == TankThatFiredShell.bomberAI)
            {
                //Spawn aoe field at collision point
                Instantiate(aoeField, gameObject.transform.position + new Vector3(0, -0.65f, 0), Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
