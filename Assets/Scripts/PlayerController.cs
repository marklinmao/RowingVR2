//using System;
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public Transform destinationPosition;

    public GameObject distanceValueText;
    public GameObject speedValueText;
    public GameObject gyroXText;
    public GameObject gyroYText;
    public GameObject gyroZText;
    public GameObject accelXText;
    public GameObject accelYText;
    public GameObject accelZText;

    public GameObject rowingDirX;
    public GameObject rowingDirY;
    public GameObject rowingDirZ;
    public GameObject rowingForce;

    private GameManager gameManager;
    private Rigidbody rb;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Quaternion initialCameraRotation;
    private Vector3 horizontalDirection;

    private float speed = 0f;
    private Vector3 speedAccumulated = new Vector3(0, 0, 0);
    public float speedNumber = 0f;
    private float acceleration = 0.5f;

    private float timestampOfClickBtnDown = 0.0f;
    private float timeElapsed = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialCameraRotation = Camera.main.transform.rotation;
        gameManager = (GameManager)Camera.main.GetComponent(typeof(GameManager));
    }

    void Update()
    {
        //show alarm screen
        if (gameManager.IsPlayingState())
        {

        }

        //show speed
        speedNumber = speedAccumulated.magnitude / 20;
        speedValueText.GetComponent<Text>().text = speedNumber.ToString("F2");

        //show distance left
        distanceValueText.GetComponent<Text>().text = GetDistance().ToString("F2");

        //show instant sensor data
        gyroXText.GetComponent<Text>().text = GvrControllerInput.Gyro.x.ToString("F2");
        gyroYText.GetComponent<Text>().text = GvrControllerInput.Gyro.y.ToString("F2");
        gyroZText.GetComponent<Text>().text = GvrControllerInput.Gyro.z.ToString("F2");
        accelXText.GetComponent<Text>().text = GvrControllerInput.Accel.x.ToString("F2");
        accelYText.GetComponent<Text>().text = GvrControllerInput.Accel.y.ToString("F2");
        accelZText.GetComponent<Text>().text = GvrControllerInput.Accel.z.ToString("F2");

    }

    private void FixedUpdate()
    {

        //if (gameManager.IsPlayingState())
        //{
            // Speed up by clicking on TouchPad
            if (GvrControllerInput.ClickButton)
            {
                speed += acceleration;
                

                Vector3 targetPos = GvrControllerInput.Orientation * Vector3.forward;
                horizontalDirection.Set(targetPos.x, 0, targetPos.z);
                rb.AddRelativeForce(horizontalDirection.normalized * speed, ForceMode.Force);

                speedAccumulated = horizontalDirection.normalized * speed + speedAccumulated;
            }
            else
            {
                speed = 0;
            }
        //}
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Land"))
        {
            Debug.Log(">>>>>>>>>>>>>collision!>>>>>>>>>>>>>>>>>" + other.gameObject.tag + ", " + other.gameObject.name);
            gameManager.SwitchToFailureState();
        }
        else
            Debug.Log(">>>>>>>>>>>>>collision with land!!>>>>>>>>>>>>>>>>>" + other.gameObject.tag + ", " + other.gameObject.name);
    }

    public float GetDistance()
    {
        return Vector3.Distance(transform.position, destinationPosition.position);
    }

    private void ShowAlarmScreen(GameObject alarmScreenObject, float interval)
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > interval)
        {
            if (alarmScreenObject.activeSelf)
            {
                alarmScreenObject.SetActive(false);
            }
            else
            {
                alarmScreenObject.SetActive(true);
            }
            timeElapsed = 0f;
        }
    }

    void OnEnable()
    {
        EventManager.OnRowed += Move;
    }

    void OnDisable()
    {
        EventManager.OnRowed -= Move;    
    }

    private void Move(Vector3 direction, float force)
    {
        rowingDirX.GetComponent<Text>().text = direction.x.ToString("F2");
        rowingDirY.GetComponent<Text>().text = direction.y.ToString("F2");
        rowingDirZ.GetComponent<Text>().text = direction.z.ToString("F2");
        rowingForce.GetComponent<Text>().text = force.ToString("F2");

        //TODO... moving the boat

    }

}