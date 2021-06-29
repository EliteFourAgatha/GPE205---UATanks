using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellScript : MonoBehaviour
{
    private float startTime;
    //How long to wait to destroy shell if it hasn't collided already
    //  Guarantees it won't linger in game world
    private float destroyTimer = 3f;
    public ParticleSystem roundExplodeVFX;
    void Start()
    {
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
        if(collision.gameObject.tag == "EnemyTank")
        {
            TankHealth tankHealth = collision.gameObject.GetComponent<TankHealth>();
            tankHealth.DamageTank();
            roundExplosion = Instantiate(roundExplodeVFX, collision.transform.position, Quaternion.Euler(0, 0, 0));
            Destroy(gameObject);
        }
        //If object collided with is Player tank, do (data.EnemyShellDamage) damage
        else if (collision.gameObject.tag == "PlayerTank")
        {
            TankHealth tankHealth = collision.gameObject.GetComponent<TankHealth>();
            tankHealth.DamageTank();
            roundExplosion = Instantiate(roundExplodeVFX, collision.transform.position, Quaternion.Euler(0, 0, 0));
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
