﻿//using System;
//using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public Transform destinationPosition;

    public GameObject distanceValueText;
    public GameObject speedValueText;
    public GameObject timerValueText;
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
        if (gameManager.IsPlayingState())
        {
            Vector3 accelData = GvrControllerInput.Accel;
            Vector3 gyroData = GvrControllerInput.Gyro;
            ShowStatusData(accelData, gyroData);
        }
    }


    private void ShowStatusData(Vector3 accelData, Vector3 gyroData)
    {
        //show distance left
        distanceValueText.GetComponent<Text>().text = GetDistance().ToString("F2");

        //show speed
        speedNumber = speedAccumulated.magnitude / 100;
        speedValueText.GetComponent<Text>().text = speedNumber.ToString("F2");

        //show timer
        //timerValueText.GetComponent<Text>().text = xxxxx.ToString("F2");

        //show instant sensor data
        accelXText.GetComponent<Text>().text = accelData.x.ToString("F2");
        accelYText.GetComponent<Text>().text = accelData.y.ToString("F2");
        accelZText.GetComponent<Text>().text = accelData.z.ToString("F2");
        gyroXText.GetComponent<Text>().text = gyroData.x.ToString("F2");
        gyroYText.GetComponent<Text>().text = gyroData.y.ToString("F2");
        gyroZText.GetComponent<Text>().text = gyroData.z.ToString("F2");
    }

    private void FixedUpdate()
    {
        if (gameManager.IsPlayingState())
        {
            ///////////////////////////////////////from spacewalk////////////////////////////////////
            //if (GvrControllerInput.ClickButton)
            //{
            //    speed += acceleration;
            //    
            //    Vector3 targetPos = GvrControllerInput.Orientation * Vector3.forward;
            //    horizontalDirection.Set(targetPos.x, 0, targetPos.z);
            //    rb.AddRelativeForce(horizontalDirection.normalized * speed, ForceMode.Force);
            //
            //    speedAccumulated = horizontalDirection.normalized * speed + speedAccumulated;
            //}
            //else
            //{
            //    speed = 0;
            //}
            ////////////////////////////////////////////////////////////////////////////////////////

            if(speedNumber > 2 )
            {
                Vector3 backForce = Vector3.back.normalized * 0.3f;
                rb.AddRelativeForce(backForce, ForceMode.Force);

                speedAccumulated -= backForce;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        //gameManager.SwitchToFailureState();
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
        SensorManager.OnRowed += Move;
    }

    void OnDisable()
    {
        SensorManager.OnRowed -= Move;    
    }

    private void Move(Vector3 direction, float force)
    {
        rowingDirX.GetComponent<Text>().text = direction.x.ToString("F2");
        rowingDirY.GetComponent<Text>().text = direction.y.ToString("F2");
        rowingDirZ.GetComponent<Text>().text = direction.z.ToString("F2");
        
        //move the canoe
        horizontalDirection.Set(direction.x, 0, direction.z);
        float finalForceValue = ForceCalculation(force);
        rowingForce.GetComponent<Text>().text = finalForceValue.ToString("F2");

        Vector3 finalForce = horizontalDirection.normalized * finalForceValue;
        rb.AddRelativeForce(finalForce, ForceMode.Force);

        speedAccumulated += finalForce;
    }

    private float sensorMax = 78;
    private float sensorMin = 10f;
    private float mappedMax = 5f;
    private float mappedMin = 0.5f;
    private float ForceCalculation(float sensorForce)
    {
        return sensorForce * 2;
        //return ((sensorForce - sensorMin) / (sensorMax - sensorMin)) * (mappedMax - mappedMin) + mappedMin;
    }

}