using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    //Create enumeration variable to differentiate between types of input
    //  Can change this value in inspector, only one chosen at at time
    public enum InputScheme {WASD, arrowkeys};
    public InputScheme input;
    public TankMotor motor;
    public TankShoot shoot;
    public TankData data;
    public Armor armor;

    void Start()
    {
        //If scripts aren't set in inspector, set with GetComponent
        if(motor == null)
        {
            motor = gameObject.GetComponent<TankMotor>();
        }
        if(shoot == null)
        {
            shoot = gameObject.GetComponent<TankShoot>();
        }
        if(data == null)
        {
            data = gameObject.GetComponent<TankData>();
        }
        if(armor == null)
        {
            armor = gameObject.GetComponent<Armor>();
        }
    }

    void Update()
    {
        //If player presses space and canFire = true (not still on respawn timer), FireShell()
        if(Input.GetKey(KeyCode.Space) && shoot.canFire == true)
        {
            shoot.FireShell();
        }
        //If player presses Q and canUseArmor = true (not still on respawn timer), ActivateArmor()
        if(Input.GetKey(KeyCode.Q) && armor.canUseArmor == true)
        {
            armor.ActivateArmor();
        }
        switch(input)
        {
            //If input set to WASD, move/rotate with WASD
            case InputScheme.WASD:
                if (Input.GetKey(KeyCode.W))
                {
                    motor.MoveTank(data.moveSpeed);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    motor.MoveTank(-data.moveSpeed);
                }
                if (Input.GetKey(KeyCode.A))
                {
                    motor.RotateTank(-data.turnSpeed);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    motor.RotateTank(data.turnSpeed);
                }
                break;
            //If input set to arrow keys, move/rotate with arrow keys
            case InputScheme.arrowkeys:
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    motor.MoveTank(data.moveSpeed);
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    motor.MoveTank(-data.moveSpeed);
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    motor.RotateTank(-data.turnSpeed);
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    motor.RotateTank(data.turnSpeed);
                }
                break;
        }
    }
}
