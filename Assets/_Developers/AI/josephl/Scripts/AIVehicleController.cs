using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AIVehicleController : MonoBehaviour
{
    [SerializeField] private GameObject wheelMeshes, wheelColliders;
    private WheelCollider[] wheels = new WheelCollider[4];
    private GameObject[] wheelMesh = new GameObject[4];
    private Rigidbody rigidBody;
    [SerializeField] private GameObject centerOfMass;
    internal enum DriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive
    }
    internal enum BrakeType
    {
        FrontWheelBrake,
        RearWheelBrake,
        FourWheelBrake
    }

    [Header("Car Properties")]
    [SerializeField] private DriveType driveType = DriveType.FourWheelDrive;
    [SerializeField] private BrakeType brakeType = BrakeType.FourWheelBrake;
    [SerializeField] private float topSpeed;
    [SerializeField] private float carPower;
    [SerializeField] private float brakePower;
    [SerializeField] private int boostPower;
    [SerializeField] private float turningRadius;
    [Viewable] [SerializeField] private float downForceValue;
    [Viewable] [SerializeField] private float currentSpeed;

    private float topSpeedDrag, idleDrag = 50f;


    private float horizontalInput;
    private float verticalInput;
    private bool handbrakeInput;
    private bool boostInput;


    private void Awake()
    {
        GetGameObjects();

        rigidBody.centerOfMass = centerOfMass.transform.localPosition;
    }

    private void FixedUpdate()
    {
        AnimateWheels();

        HandleAcceleration(verticalInput);

        HandleSteering(horizontalInput);

        HandleBraking(handbrakeInput);

        AdjustDrag();

        AddDownForce();
    }

    private void HandleAcceleration(float vInput)
    {
        if (driveType == DriveType.FourWheelDrive)
        {
            foreach (WheelCollider wheel in wheels)
            {
                wheel.motorTorque = vInput * (carPower / 4);
                //Debug.Log(wheel.motorTorque);
            }
        }
        else if (driveType == DriveType.RearWheelDrive)
        {
            for (int i = 2; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = vInput * (carPower / 2);
            }
        }
        else if (driveType == DriveType.FrontWheelDrive)
        {
            for (int i = 0; i < wheels.Length - 2; i++)
            {
                wheels[i].motorTorque = vInput * (carPower / 2);
            }
        }

        currentSpeed = rigidBody.velocity.magnitude * 3.6f;



        //Boost
        if (boostInput)
        {
            rigidBody.AddForce(transform.forward * boostPower);
        }
    }

    private void HandleBraking(bool brakeInput)
    {
        if (brakeInput)
        {
            if (brakeType == BrakeType.RearWheelBrake)
            {
                wheels[2].brakeTorque = brakePower;
                wheels[3].brakeTorque = brakePower;
            }
            else if (brakeType == BrakeType.FrontWheelBrake)
            {
                wheels[0].brakeTorque = brakePower;
                wheels[1].brakeTorque = brakePower;
            }
            else if (brakeType == BrakeType.FourWheelBrake)
            {
                foreach (WheelCollider wheel in wheels)
                {
                    wheel.brakeTorque = brakePower;
                }
            }
        }
        else
        {
            foreach (WheelCollider wheel in wheels)
            {
                wheel.brakeTorque = 0;
            }
        }
    }

    private void HandleSteering(float hInput)
    {
        //Ackerman steering formula
        if (hInput > 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.5f / (turningRadius + (1.5f / 2))) * hInput;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.5f / (turningRadius - (1.5f / 2))) * hInput;
        }
        else if (hInput < 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.5f / (turningRadius - (1.5f / 2))) * hInput;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.5f / (turningRadius + (1.5f / 2))) * hInput;
        }
        else
        {
            wheels[0].steerAngle = 0;
            wheels[1].steerAngle = 0;
        }
    }

    private void AnimateWheels()
    {
        Vector3 wheePosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].GetWorldPose(out wheePosition, out wheelRotation);
            wheelMesh[i].transform.position = wheePosition;
            wheelMesh[i].transform.rotation = wheelRotation;
        }
    }

    public void HandleInput(float v, float h, bool b)
    {
        //Replace with new input system
        verticalInput = v;
        horizontalInput = h;
        handbrakeInput = b;
        //boostInput = Input.GetKey(KeyCode.LeftShift);
    }

    private void AddDownForce()
    {
        downForceValue = currentSpeed / 2;

        rigidBody.AddForce(-transform.up * downForceValue * rigidBody.velocity.magnitude);
    }

    private void GetGameObjects()
    {
        for (int i = 0; i < wheelMeshes.transform.childCount; i++)
        {
            wheelMesh[i] = wheelMeshes.transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < wheelColliders.transform.childCount; i++)
        {
            wheels[i] = wheelColliders.transform.GetChild(i).GetComponent<WheelCollider>();
        }

        rigidBody = GetComponent<Rigidbody>();


    }


    void AdjustDrag()
    {
        if (currentSpeed >= topSpeed)
        {
            rigidBody.drag = topSpeedDrag;
        }
        else if (carPower == 0)
        {
            rigidBody.drag = idleDrag;
        }
    }

    public float GetSpeed() => currentSpeed;

    private void OnGUI()
    {
        //Change gui color
        GUI.color = Color.black;
        //Change gui size
        GUI.skin.label.fontSize = 15;
        //Show speed on screen
        GUI.Label(new Rect(10, 10, 150, 100), "Speed: " + currentSpeed.ToString("0") + " KPH");
    }
}
