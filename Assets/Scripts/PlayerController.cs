//using System;
//using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    private Vector3 departurePosition;
    public Transform destinationPosition;

    public GameObject timeCostValueText;

    public GameObject goScreen;
    public GameObject rushScreen;

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

    private Vector3 horizontalDirection;

    private float speedValue = 0f;
    private float acceleration = 0.5f;

    private float timestampOfClickBtnDown = 0.0f;
    private float timeElapsed = 0f;

    //the track of the player
    private List<Vector3> posTrackList = new List<Vector3>();
    private const int POS_TRACKLIST_SIZE = 50;
    private const float POS_TRACK_PERIOD = 1;


    private void AddToTrakList(Vector3 position)     {         if (posTrackList.Count >= POS_TRACKLIST_SIZE)
            posTrackList.RemoveAt(0);          posTrackList.Add(position);     }


    private void Awake()
    {
        this.departurePosition = this.transform.position;
    }


    void Start()
    {
        this.rb = GetComponent<Rigidbody>();
        this.gameManager = (GameManager)Camera.main.GetComponent(typeof(GameManager));
    }

    void Update()
    {
        if(gameManager.IsInitState())
        {
            ShowInitStatus();
        }

        if (gameManager.IsPlayingState())
        {
            Vector3 accelData = GvrControllerInput.Accel;
            Vector3 gyroData = GvrControllerInput.Gyro;
            ShowPlayingStatus(accelData, gyroData);
        }
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

            //track position
            AddToTrakList(this.transform.position);

            //calculate and show speed
            CalculateSpeed();


            //show notification screen
            if (Vector3.Distance(this.transform.position, this.departurePosition) < 100)
                showNotificationScreen(goScreen, 1f);
            else
                goScreen.SetActive(false);

            if (Vector3.Distance(this.transform.position, this.destinationPosition.position) < 200)
                showNotificationScreen(rushScreen, 1f);
            else
                rushScreen.SetActive(false);
        }
        
    }

    private void showNotificationScreen(GameObject alarmScreenObject, float interval)
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

    private void CalculateSpeed()
    {
        float distance = Vector3.Distance(this.posTrackList[posTrackList.Count - 1], this.posTrackList[0]);
        this.speedValue = distance / (Time.deltaTime * POS_TRACKLIST_SIZE);
    }

    private void ShowInitStatus()
    {
        timeCostValueText.GetComponent<Text>().text = GameManager.bestCompletePlayingTime.ToString("F2");
    }

    private void ShowPlayingStatus(Vector3 accelData, Vector3 gyroData)
    {
        //show distance left
        distanceValueText.GetComponent<Text>().text = GetDistance().ToString("F2");

        //show speed
        //speedNumber = speedAccumulated.magnitude / 100;
        speedValueText.GetComponent<Text>().text = speedValue.ToString("F2");

        //show timer
        timerValueText.GetComponent<Text>().text = (Time.realtimeSinceStartup - GameManager.startPlayingTime).ToString("F2");

        //show instant sensor data
        accelXText.GetComponent<Text>().text = accelData.x.ToString("F2");
        accelYText.GetComponent<Text>().text = accelData.y.ToString("F2");
        accelZText.GetComponent<Text>().text = accelData.z.ToString("F2");
        gyroXText.GetComponent<Text>().text = gyroData.x.ToString("F2");
        gyroYText.GetComponent<Text>().text = gyroData.y.ToString("F2");
        gyroZText.GetComponent<Text>().text = gyroData.z.ToString("F2");
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