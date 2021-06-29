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
    public Armor armor;
    public Game_Manager gameManager;
    private int avoidanceStage = 0;
    public float avoidanceTime = 2f;
    //Exit time for obstacle avoidance
    private float exitTime;
    //Flee used to run and heal, coward used to flee for (fleeTime) seconds upon playerShoot
    public enum AIState{patrol, chaseandshoot, chase, checkflee, flee, rest};
    public AIState aiState = AIState.patrol;
    public float stateEnterTime;
    public float aiSenseRadius = 10f;
    //Distance player must be away for AIPatrol to stop chasing
    public float patrolChaseRadius = 5f;
    public float tooCloseRadius = 3f;
    private float lastShootTime;
    public float restHealRate = 1f;
    public Transform[] waypoints;
    private int currentWP = 0;
    private bool isPatrolForward = true;
    public float closeEnoughToWP = 1f;
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
            else if(Vector3.SqrMagnitude(tfRef.position - target.position) >= (patrolChaseRadius * patrolChaseRadius))
            {
                Debug.Log("chase+fire -> patrol");
                SetCurrentWaypoint();
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
            else if(Vector3.SqrMagnitude(tfRef.position - target.position) >= (patrolChaseRadius * patrolChaseRadius))
            {
                Debug.Log("chase+fire -> patrol");
                SetCurrentWaypoint();
                ChangeState(AIState.patrol);
            }
            else if(Vector3.SqrMagnitude(tfRef.position - target.position) <= (tooCloseRadius * tooCloseRadius))
            {
                ChangeState(AIState.flee);
            }
        }
        else if(aiState == AIState.patrol)
        {
            //Do behaviors
            if(avoidanceStage != 0)
            {
                DoAvoidance();
            }
            else
            {
                DoPatrol();
            }
            //Check for transitions
            if(health.currentHealth < health.maxHealth * 0.5f)
            {
                ChangeState(AIState.checkflee);
            }
            //if player enters spotlight collider or short raycast forward, change state
            //filler
            //filler code until spotlight set up
            RaycastHit hit;
            if(Physics.Raycast(tfRef.position, tfRef.forward, out hit, 10f))
            {
                if(hit.collider.CompareTag("PlayerTank"))
                {
                    Debug.Log("Patrol -> chaseandshoot");
                    ChangeState(AIState.chaseandshoot);
                }
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
            if(Vector3.SqrMagnitude(tfRef.position - target.position) >= (patrolChaseRadius * patrolChaseRadius))
            {
                if(health.currentHealth >= (health.maxHealth * 0.5f))
                {
                    ChangeState(AIState.patrol);
                }
            }
        }
        else if(aiState == AIState.checkflee)
        {
            //Do behaviors
            CheckForFlee();
            //Check for transitions
            if(Vector3.SqrMagnitude(tfRef.position - target.position) <= (aiSenseRadius * aiSenseRadius))
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
            if(Vector3.SqrMagnitude(tfRef.position - target.position) <= (aiSenseRadius * aiSenseRadius))
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
       if(Vector3.SqrMagnitude(tfRef.position - target.position) <= (data.fleeDistance * data.fleeDistance))
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

    public void SetCurrentWaypoint()
    {
        Transform closestWP = null;
        foreach(Transform wp in waypoints)
        {
            //Start at high value and work backwards, infinity because size of game world unknown
            float distance = Mathf.Infinity;
            Vector3 diff = wp.transform.position - tfRef.position;
            float curDistance = diff.sqrMagnitude;
            //Check every waypoint in array. If distance is closer than one previously tested,
            // Set distance equal to that value until closest waypoint is found.
            if(curDistance < distance)
            {
                closestWP = wp;
                currentWP = System.Array.IndexOf(waypoints, wp);
                distance = curDistance;
            }
        }
        //closestWP = currentWP;
        Debug.Log("CurrentWP: " + currentWP);
    }
    public void DoPatrol()
    {
        //If tank is unable to rotate, (RotateTowards = false), rotate tank
        if (motor.RotateTowardsWP(waypoints[currentWP].position, data.turnSpeed) == false)
        {
            motor.MoveTank(data.aiPatrolMoveSpeed);
        }
        //If we are close enough to waypoint, advance to next waypoint in array.
        //Using Distance formula:
        // Distance between points(p, q) = √(q1 - p1)^2 + (q2 - p2)^2
        //  Use Sqr Magnitude to get square root of equation
        if(Vector3.SqrMagnitude(waypoints[currentWP].position - tfRef.position) < (closeEnoughToWP * closeEnoughToWP))
        {
            if(isPatrolForward == true)
            {
                if(currentWP < (waypoints.Length - 1))
                {
                    currentWP++;
                }
                //Patrol backward and decrement current waypoint
                else
                {
                    isPatrolForward = false;
                    currentWP--;
                }
            }  
            else
            {
                if(currentWP > 0)
                {
                    currentWP--;
                }
                //Patrol forward and increment current waypoint
                else
                {
                    isPatrolForward = true;
                    currentWP++;
                }
            }
        }
    }
    public void ChangeState(AIState newState)
    {
        //Change state to given parameter value
        aiState = newState;
        stateEnterTime = Time.time;
    }
}
