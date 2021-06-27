using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RequireComponent tells Unity that a TankData component needs to be added
//  to any game object this script is attached to
[RequireComponent (typeof(TankData))]
public class AIPatrol : MonoBehaviour
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
    public Game_Manager gameManager;
    private int avoidanceStage = 0;
    public float avoidanceTime = 2f;
    //Exit time for obstacle avoidance
    private float exitTime;
    //Flee used to run and heal, coward used to flee for (fleeTime) seconds upon playerShoot
    public enum AIState{patrol, chaseandshoot, chase, checkflee, flee, rest};
    public AIState aiState = AIState.chase;
    public float stateEnterTime;
    public float aiSenseRadius = 8f;
    //Distance player must be away for AIPatrol to stop chasing
    public float patrolChaseRadius = 12f;
    private float lastShootTime;
    public float restHealRate = 1f;
    private bool isPatrolForward = true;
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
            else if(Vector3.Distance(tfRef.position, target.position) > aiSenseRadius)
            {
                Debug.Log("chase+fire -> chase");
                ChangeState(AIState.chase);
            }
            else if(Vector3.Distance(tfRef.position, target.position) > patrolChaseRadius)
            {
                Debug.Log("chase+fire -> patrol");
                int closeWP = FindGameObjectsWithTag("Waypoint");
                SetCurrentWaypoint(closeWP);
                ChangeState(AIState.patrol);
            }
        }
        else if(aiState == AIState.chaseandshoot)
        {
            //Do behaviors
            if(avoidanceStage != 0)
            {
                DoAvoidance();
            }
            else
            {
                DoChase();
                //Limit fire rate by (data.enemyReloadTimer) seconds
                if(Time.time > lastShootTime + data.enemyReloadTimer)
                {
                    shoot.FireShell();
                    lastShootTime = Time.time;
                }
            }
            //Check for transitions
            if(health.currentHealth < health.maxHealth * 0.5f)
            {
                ChangeState(AIState.checkflee);
            }
            else if(Vector3.Distance(tfRef.position, target.position) > aiSenseRadius)
            {
                Debug.Log("chase+fire -> chase");
                ChangeState(AIState.chase);
            }
            else if(Vector3.Distance(tfRef.position, target.position) > patrolChaseRadius)
            {
                Debug.Log("chase+fire -> patrol");
                ChangeState(AIState.patrol);
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
            Debug.Log("Rest enabled");
            DoRest();
            //Check for transitions
            if(Vector3.Distance(tfRef.position, target.position) <= aiSenseRadius)
            {
                Debug.Log("Rest -> flee");
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
        //if (health.currentHealth == health.maxHealth)
        //{
        //    needsHealing = false;
        //}
        //while(needsHealing)
        //{
            //if(Time.time > nextHealTime)
            //{
                //Debug.Log("increment health!" + health.currentHealth);
                //Increment heal time by (tickPeriod) value
                //nextHealTime = Time.time + tickPeriod;
                //StartCoroutine(DoHealingTick());

                //Increase current health by (restHealRate per second)
                //health.currentHealth += (restHealRate * Time.deltaTime);

                //Make sure we never go over max health
                //health.currentHealth = Mathf.Min(health.currentHealth, health.maxHealth);
            //}
        //}
        
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

    public void SetCurrentWaypoint(int closestWaypoint)
    {

    }
    public void DoPatrol()
    {
        
    }
    public void ChangeState(AIState newState)
    {
        //Change state to given parameter value
        aiState = newState;
        stateEnterTime = Time.time;
    }
}
