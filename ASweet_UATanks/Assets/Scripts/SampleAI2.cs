using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RequireComponent tells Unity that a TankData component needs to be added
//  to any game object this script is attached to
[RequireComponent (typeof(TankData))]
public class SampleAI2 : MonoBehaviour
{
    //Store transform in a variable.
    // This avoids calling GameObject.GetComponent() multiple times, which is slow
    private Transform tfRef;
    //Target transform to chase / flee from
    public Transform target;
    public enum EnemyAIMode {chase, flee};
    public EnemyAIMode enemyAIMode;
    public TankData tankData;
    public TankMotor tankMotor;
    void Start()
    {
        tfRef = gameObject.GetComponent<Transform>();
        if(tankData == null)
        {
            tankData = gameObject.GetComponent<TankData>();
        }
        if(tankMotor == null)
        {
            tankMotor = gameObject.GetComponent<TankMotor>();
        }
    }

    void Update()
    {
        if(enemyAIMode == EnemyAIMode.chase)
        {
            RunChaseMode();
        }
        else if(enemyAIMode == EnemyAIMode.flee)
        {
            RunFleeMode();
        }
    }
    //Constantly rotate and move towards target (player) position
    public void RunChaseMode()
    {
        tankMotor.RotateTowardsWP(target.position, tankData.turnSpeed);
        tankMotor.MoveTank(tankData.moveSpeed);
    }
    public void RunFleeMode()
    {
        // The vector from AI (this object) to target is (target.position - thisObject.position)
        Vector3 vectorToTarget = target.position - tfRef.position;
        // Flip vector (away from target) by multiplying by negative 1.
        Vector3 vectorAwayFromTarget = (vectorToTarget * -1);
        // Normalize to give vector magnitude of 1
        vectorAwayFromTarget.Normalize();
        // Multiply by (data.fleeDistance) to make normalized vector that length
        vectorAwayFromTarget *= tankData.fleeDistance;
        // Create position to flee to by adding VectorAway distance to current position
        Vector3 fleePosition = vectorAwayFromTarget + tfRef.position;
        tankMotor.RotateTowardsWP(fleePosition, tankData.turnSpeed);
        tankMotor.MoveTank(tankData.moveSpeed);

    }
}
