using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    public TankData data;
    public TankMotor motor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        motor.MoveTank(data.moveSpeed);
        motor.RotateTank(data.turnSpeed);
    }
}
