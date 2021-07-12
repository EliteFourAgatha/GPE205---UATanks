using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHover : MonoBehaviour
{
    public float degPerSec = 15f;
    public float amplitude = 1f;
    public float frequency = 1f;

    //Temporary variables for position of object
    Vector3 offsetPosition = new Vector3();
    Vector3 tempPosition = new Vector3();
    void Update()
    {
        MakeObjectHover();
    }
    public void MakeObjectHover()
    {
        //Rotate object around y axis, in (degPerSec) degrees per second.
        // Space.World because we're rotating in relation to the scene itself
        gameObject.transform.Rotate(new Vector3(0f, Time.deltaTime * degPerSec, 0f), Space.World);

        //Set offset position
        tempPosition = offsetPosition;
        
        //Change temp position using sin wave
        // Multiply by (freuqency) value for
        //EXPLAIN FREQUENCY & AMPLITUDE WHAT THEY AFFECT
        tempPosition.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        //Set current position equal to created tempPosition, along sin wave
        gameObject.transform.position = tempPosition;
    }
}
