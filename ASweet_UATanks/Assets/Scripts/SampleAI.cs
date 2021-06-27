using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RequireComponent tells Unity that a TankData component needs to be added
//  to any game object this script is attached to
[RequireComponent (typeof(TankData))]
public class SampleAI : MonoBehaviour
{
    //Transform reference to avoid GetComponent calls
    public Transform tfRef;
    public Transform[] waypoints;
    public enum WPLoopType {Stop, Loop, PingPong};
    public WPLoopType wpLoopType;
    public TankData data;
    public TankMotor motor;
    private int currentWP = 0;
    private bool isPatrolForward = true;
    public float closeEnoughToWP = 1f;
    private void Awake()
    {
        tfRef = gameObject.GetComponent<Transform>();
    }
    void Start()
    {
        if(data == null)
        {
            data = gameObject.GetComponent<TankData>();
        }
        if(motor == null)
        {
            data = gameObject.GetComponent<TankData>();
        }
    }
    void Update()
    {
        //If tank is unable to rotate, (RotateTowards = false), rotate tank
        if (motor.RotateTowardsWP(waypoints[currentWP].position, data.turnSpeed) == false)
        {
            motor.MoveTank(data.moveSpeed);
        }
        //If we are close enough to waypoint, advance to next waypoint in array.
        //Using Distance formula:
        // Distance between points(p, q) = √(q1 - p1)^2 + (q2 - p2)^2
        //  Use Sqr Magnitude to get square root of equation
        if(Vector3.SqrMagnitude(waypoints[currentWP].position - tfRef.position) < (closeEnoughToWP * closeEnoughToWP))
        {
            if(wpLoopType == WPLoopType.Stop)
            {
                RunStopLoop();
            }
            else if(wpLoopType == WPLoopType.Loop)
            {
                RunLoop();
            }
            else if(wpLoopType == WPLoopType.PingPong)
            {
                RunPatrolLoop();
            }
        }
    }
    //Stop after one patrol loop
    public void RunStopLoop()
    {
        if(currentWP < (waypoints.Length - 1))
        {
            currentWP++;
        }
    }
    //Loop through waypoints in order in a cycle
    public void RunLoop()
    {
        if(currentWP < (waypoints.Length - 1))
        {
            currentWP++;
        }
        else
        {
            currentWP = 0;
        }
    }
    public void RunPatrolLoop()
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
