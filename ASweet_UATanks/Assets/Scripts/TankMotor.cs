using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RequireComponent tells Unity that a TankData component needs to be added
//  to any game object this script is attached to
[RequireComponent (typeof(TankData))]
public class TankMotor : MonoBehaviour
{
    //Store transform in a variable.
    // This avoids calling GameObject.GetComponent() multiple times, which is slow
    private Transform tfRef;
    //Character Controller reference
    private CharacterController characterController;
    public TankData tankData;
    //Float to check if tank is close enough to waypoint
    public float closeEnoughToWaypoint = 1f;

    public void Awake()
    {
        tfRef = gameObject.GetComponent<Transform>();
    }
    public void Start()
    {
        //If tank data isn't set in inspector, set it with GetComponent
        if(tankData == null)
        {
            tankData = gameObject.GetComponent<TankData>();
        }
        characterController = gameObject.GetComponent<CharacterController>();
    }
    private void Update() 
    {
        //if(RotateTowards)
    }
    //Passing float speed, negative value is backwards
    public void MoveTank(float speed)
    {
        //Vector3 to hold speed value.
        //Start with vector pointing same direction as game object
        Vector3 speedVector = tfRef.forward;

        //Multiply by speed so the vector isn't one unit in length
        speedVector *= speed;

        //SimpleMove will multiply speed float by Time.deltaTime for meters/second instead of
        //  frames per second automatically
        characterController.SimpleMove(speedVector);
    }
    //Passing float speed for speed of rotation.
    public void RotateTank(float speed)
    {
        //Vector3 to hold rotation value in euler angles
        // Start with vector3.up which is the same as (0,1,0). (X, Y, Z).
        //  We want to rotate about the Y-axis, which will turn the tank's head
        //   Positive value rotates right, negative value rotates left
        Vector3 rotateVector = Vector3.up;

        //Multiply rotate vector by the product of float speed * time.deltaTime.
        // Multiplying by time.deltaTime will change it from "per frame" to "per second".
        //  time.deltaTime is the time between frames or time it took to draw the last frame.
        rotateVector *= (speed * Time.deltaTime);

        //Call rotate, and use Space.self to rotate in relation to the object itself,
        //  instead of to the scene, which would be Space.world
        tfRef.Rotate(rotateVector, Space.Self);
    }
    //Rotates towards target waypoint.
    //if can rotate, true. if not, (already facing target), false)
    public bool RotateTowardsWP(Vector3 targetWP, float speed)
    {
        Vector3 vectorToNextWP;
        //The vector to next waypoint is the difference (subtraction) between
        // the two points, starting at target
        vectorToNextWP = targetWP - tfRef.position;
        //Find quaternion that looks down that vector
        // Quaternions are math objects that tell how to rotate an object
        Quaternion targetRotation = Quaternion.LookRotation(vectorToNextWP);
        //If not currently
        if (targetRotation != tfRef.rotation)
        {
            //Use RotateTowards "from" current transform rotation "to" target transform rotation.
            //  data.turnSpeed = degree speed turned per sec
            //    Time.deltaTime used to change to "degrees per sec" instead of "degrees per frame"
            tfRef.rotation = Quaternion.RotateTowards(tfRef.rotation, targetRotation, tankData.turnSpeed * Time.deltaTime);
            //Rotated, so return true
            return true;
        }
        //Already facing that direction, return false
        else
        {
            return false;
        }
    }
}
