using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RequireComponent tells Unity that a TankData component needs to be added
//  to any game object this script is attached to
[RequireComponent (typeof(TankData))]
public class AIBomber : MonoBehaviour
{
    //Store transform in a variable.
    // This avoids calling GameObject.GetComponent() multiple times, which is slow
    private Transform tfRef;
    //Target transform to chase / flee from
    public Transform target;
    public TankData data;
    public TankMotor motor;
    public TankHealth health;
    public TankShoot shoot;
    public Armor armor;
    public Game_Manager gameManager;
    private int avoidanceStage = 0;
    public float avoidanceTime = 2f;
    //Exit time for obstacle avoidance
    private float exitTime;
    //Flee used to run and heal, coward used to flee for (fleeTime) seconds upon playerShoot
    public enum AIState{bomb, chase, checkflee, flee, rest};
    public AIState aiState = AIState.chase;
    public float stateEnterTime;
    public float aiSenseRadius = 8f;
    public float bomberRadius = 15f;
    public float tooCloseRadius = 3f;
    private float lastShootTime;
    public float bombDistance = 2f;
    public float restHealRate = 1f;
    void Start()
    {
        tfRef = gameObject.GetComponent<Transform>();
        if(data == null)
        {
            data = gameObject.GetComponent<TankData>();
        }
        if(motor == null)
        {
            motor = gameObject.GetComponent<TankMotor>();
        }
        if(health == null)
        {
            health = gameObject.GetComponent<TankHealth>();
        }
        if(shoot == null)
        {
            shoot = gameObject.GetComponent<TankShoot>();
        }
        if(armor == null)
        {
            armor = gameObject.GetComponent<Armor>();
        }
    }
    void Update()
    {
        if(aiState == AIState.chase)
        {
            //Do behaviors
            if (avoidanceStage != 0)
            {
                DoAvoidance();
            }
            else
            {
                DoChase();
            }
            //Check for transitions
            if(health.currentHealth < health.maxHealth * 0.5f)
            {
                ChangeState(AIState.checkflee);
            }
            //If within bomb radius but outside sense radius, enable bomber mode
            else if(Vector3.Distance(tfRef.position, target.position) >= aiSenseRadius)
            {
                if(Vector3.Distance(tfRef.position, target.position) <= bomberRadius)
                {
                    ChangeState(AIState.bomb);
                }
            }
            else if(Vector3.Distance(tfRef.position, target.position) <= aiSenseRadius)
            {
                ChangeState(AIState.flee);
            }
        }
        else if(aiState == AIState.bomb)
        {
            //Do behaviors
            if(avoidanceStage != 0)
            {
                DoAvoidance();
            }
            else
            {
                //Instantiate bomb above player's position + 1 second in future
                // (1 second + forward vector of player tank)
                Vector3 bombTarget = target.position + (target.forward * bombDistance);
                motor.RotateTowardsWP(bombTarget, data.turnSpeed);
                //Limit fire rate by (data.enemyBombReloadTimer) seconds
                if(Time.time > lastShootTime + data.enemyBombReloadTimer)
                {
                    shoot.DropBomb(bombTarget);
                    lastShootTime = Time.time;
                }
            }
            //Check for transitions
            if(health.currentHealth < health.maxHealth * 0.5f)
            {
                ChangeState(AIState.checkflee);
            }
            else if(Vector3.Distance(tfRef.position, target.position) <= tooCloseRadius)
            {
                ChangeState(AIState.flee);
            }
            else if(Vector3.Distance(tfRef.position, target.position) >= bomberRadius)
            {
                Debug.Log("out of range!");
                Debug.Log(Vector3.Distance(tfRef.position, transform.position));
                ChangeState(AIState.chase);
            }
        }
        else if(aiState == AIState.flee)
        {
            //Do behaviors
            if(avoidanceStage != 0)
            {
                DoAvoidance();
            }
            else
            {
                DoFlee();
            }
            //Check for transitions
            if(Time.time >= stateEnterTime + 30)
            {
                ChangeState(AIState.checkflee);
            }
            //If fleeing and outside range, and health >= maxHealth * 0.5, chase again
            if(Vector3.Distance(target.position, tfRef.position) >= aiSenseRadius)
            {
                if(health.currentHealth >= (health.maxHealth * 0.5f))
                {
                    if(Vector3.Distance(target.position, tfRef.position) <= bomberRadius)
                    {
                        Debug.Log("flee -> bomb");
                        ChangeState(AIState.bomb);
                    }
                    else
                    {
                        Debug.Log("flee -> chase");
                        ChangeState(AIState.chase);
                    }
                }
            }
        }
        else if(aiState == AIState.checkflee)
        {
            //Do behaviors
            CheckForFlee();
            //Check for transitions
            if(Vector3.Distance(tfRef.position, target.position) <= aiSenseRadius)
            {
                ChangeState(AIState.flee);
            }
            else
            {
                ChangeState(AIState.rest);
            }
        }
        else if(aiState == AIState.rest)
        {
            //Do behaviors
            DoRest();
            //Check for transitions
            if(Vector3.Distance(tfRef.position, target.position) <= aiSenseRadius)
            {
                ChangeState(AIState.flee);
            }
            else if(health.currentHealth >= health.maxHealth)
            {
                ChangeState(AIState.chase);
            }
        }
    }

    public void DoChase()
    {
        motor.RotateTowardsWP(target.position, data.turnSpeed);
        //Check if we can move (data.MoveSpeed) units away
        // That is distance traveled in 1 second
        //  Looking for collisions "1 second into future"
        if(CanMove(data.moveSpeed))
        {
            motor.MoveTank(data.moveSpeed);
        }
        else
        {
            // Enter obstacle avoidance stage 1
            avoidanceStage = 1;
        }
    }
    public void DoAvoidance()
    {
        if(avoidanceStage == 1)
        {
            //Rotate left
            motor.RotateTank(-1 * data.turnSpeed);

            //If can move, go to stage 2
            if(CanMove(data.moveSpeed))
            {
                avoidanceStage = 2;
            }
            //Set number of seconds we stay in stage 2
            exitTime = avoidanceTime;

            //Otherwise, repeat stage 1.            
        }
        else if(avoidanceStage == 2)
        {
            if(CanMove(data.moveSpeed))
            {
                //Subtract frame time from timer and move
                exitTime -= Time.deltaTime;
                motor.MoveTank(data.moveSpeed);
                //If have moved for long enough, enable chase mode
                if(exitTime <= 0)
                {
                    avoidanceStage = 0;
                }
            }
            //Can't move forward, back to stage 1
            else
            {
                avoidanceStage = 1;
            }

        }
    }
    public bool CanMove(float speed)
    {
        RaycastHit hit;
        //Cast ray forward in distance we set
        // If raycast hits something...
        if(Physics.Raycast(tfRef.position, tfRef.forward, out hit, speed))
        {
            //If we don't hit the player...
            if(!hit.collider.CompareTag("PlayerTank"))
            {
                //Then cannot move
                return false;
            }
        }
        //Otherwise, can move, return true
        return true;
    }
    public void CheckForFlee()
    {
        //If player is still in range, flee.
       if(Vector3.Distance(tfRef.position, target.position) <= data.fleeDistance)
       {
           ChangeState(AIState.flee);
       }
       //If not, rest.
       else
       {
           ChangeState(AIState.rest);
       }
    }
    public void DoRest()
    {
        if(armor.canUseArmor)
        {
            armor.ActivateArmor();
        }
        //Increase current health by (restHealRate per second)
        health.currentHealth += (restHealRate * Time.deltaTime);

        //Make sure we never go over max health
        health.currentHealth = Mathf.Min(health.currentHealth, health.maxHealth); 
    }
    public void DoFlee()
    {
        // The vector from AI (this object) to target is (target.position - thisObject.position)
        Vector3 vectorToTarget = target.position - tfRef.position;
        // Flip vector (away from target) by multiplying by negative 1.
        Vector3 vectorAwayFromTarget = (vectorToTarget * -1);
        // Normalize to give vector magnitude of 1
        vectorAwayFromTarget.Normalize();
        // Multiply by (data.fleeDistance) to make normalized vector that length
        vectorAwayFromTarget *= data.fleeDistance;
        // Create position to flee to by adding VectorAway distance to current position
        Vector3 fleePosition = vectorAwayFromTarget + tfRef.position;
        motor.RotateTowardsWP(fleePosition, data.turnSpeed);
        motor.MoveTank(data.moveSpeed);
    }

    public void ChangeState(AIState newState)
    {
        //Change state to given parameter value
        aiState = newState;
        stateEnterTime = Time.time;
    }
}
