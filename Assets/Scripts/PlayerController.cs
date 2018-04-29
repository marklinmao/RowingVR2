//using System;
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

    //buffer of accel sensor data
    private IList<Vector3> accelBuffer = new List<Vector3>();
    private const int ACCEL_BUFFER_SIZE = 200;

    private GameObject lineAccelXGameObject;
    private LineRenderer lineRenderAccelX;

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
        //show notification screen if needed
        if (gameManager.IsPlayingState())
        {
            

            Vector3 accelData = GvrControllerInput.Accel;
            Vector3 gyroData = GvrControllerInput.Gyro;
            PushToAccelBuffer(accelData);


            ShowSensorData(accelData, gyroData);



            if (lineRenderAccelX == null)
            {
                lineAccelXGameObject = GameObject.Find("LineAccelX");
                if (lineAccelXGameObject != null)
                {
                    lineRenderAccelX = (LineRenderer)lineAccelXGameObject.GetComponent("LineRenderer");
                    lineRenderAccelX.positionCount = ACCEL_BUFFER_SIZE;
                }
            }

            if(lineRenderAccelX != null)
                ShowSensorDataVisualization();

        }

    }

    private void PushToAccelBuffer(Vector3 accelData)
    {
        if (accelBuffer.Count > ACCEL_BUFFER_SIZE)
            accelBuffer.RemoveAt(0);
        accelBuffer.Add(accelData);
    }

    private void ShowSensorData(Vector3 accelData, Vector3 gyroData)
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

    private void ShowSensorDataVisualization()
    {
        int index = 0;
        foreach (Vector3 accel in accelBuffer)
        {
            lineRenderAccelX.SetPosition(index, new Vector3(0, accel.x, 0));
            index++;
        }
    }

    private void FixedUpdate()
    {

        if (gameManager.IsPlayingState())
        {
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
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        gameManager.SwitchToFailureState();
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
        //EventManager.OnRowed += Move;
    }

    void OnDisable()
    {
        //EventManager.OnRowed -= Move;    
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