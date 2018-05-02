﻿using System; using System.Collections.Generic; using UnityEngine; using UnityEngine.UI;  public class SensorManager : MonoBehaviour {       //sensor data buffer and control     private List<Vector3> accelBuffer = new List<Vector3>();     private const int ACCEL_BUFFER_SIZE = 200;     private List<float> accelXSingleWaveBuffer = new List<float>();     private List<List<float>> accelXWaveBuffer = new List<List<float>>();     private List<float> accelXPeakBuffer = new List<float>();     private const int ACCEL_WAVE_BUFFER_SIZE = 10;     private const float THRESHOLD_ACCEL_X = -10f;     private bool waveXSwitchOn = false;     private int exceedCounter = 0;     private float lastExceedEndTime = 0f;       //visualization components     public GameObject textAccelX;     public GameObject textAccelY;     public GameObject textAccelZ;     public GameObject textPeakValues;     public GameObject textPeakValueLatest;      public GameObject lineAccelX;     private LineRenderer lineRendererAccelX;     public GameObject lineAccelY;     private LineRenderer lineRendererAccelY;     public GameObject lineAccelZ;     private LineRenderer lineRendererAccelZ;     public GameObject lineAccelWaveX;     private LineRenderer lineRendererAccelXWave;      //The scale is for making sure that the line doesn't exceed the visualization panel     private float timeScale = 100f;     private float dataScale = 180f;       //Rowing event     public delegate void RowAction(Vector3 direction, float force);     public static event RowAction OnRowed;     private Vector3 rowingDirection;     private float rowingForce;       //player     public GameObject player;     private GameManager gameManager;       void Start ()
    {         gameManager = (GameManager)Camera.main.GetComponent(typeof(GameManager));          lineRendererAccelX = (LineRenderer)lineAccelX.GetComponent("LineRenderer");         lineRendererAccelY = (LineRenderer)lineAccelY.GetComponent("LineRenderer");         lineRendererAccelZ = (LineRenderer)lineAccelZ.GetComponent("LineRenderer");         lineRendererAccelXWave = (LineRenderer)lineAccelWaveX.GetComponent("LineRenderer");     }       void Update ()
    {         if (gameManager.IsPlayingState())
        {
            float timestamp = Time.realtimeSinceStartup;              Vector3 accel = GvrControllerInput.Accel;             textAccelX.GetComponent<Text>().text = accel.x.ToString("F2");             textAccelY.GetComponent<Text>().text = accel.y.ToString("F2");             textAccelZ.GetComponent<Text>().text = accel.z.ToString("F2");              //push to buffer             PushToBuffer(accel);              //visualize the whole accel buffer             VisualizeAccelBuffer();              //check if it's rising or dropping!             if (IsAccelXThresholdExceeded(accel.x))             {                 if (!waveXSwitchOn) //that means it start to exceed the threshold                 {                     exceedCounter++;                                      //switch on the flag                     waveXSwitchOn = true;                      //clear the single wave buffer to get prepared                     accelXSingleWaveBuffer.Clear();                 }                  if (waveXSwitchOn)                 {                     //push to single wave buffer                     accelXSingleWaveBuffer.Add(accel.x);                 }             }             else             {                 if (waveXSwitchOn) //that means is get back to normal                 {                     //switch off the flag                     waveXSwitchOn = false;                      if(Time.realtimeSinceStartup - lastExceedEndTime > 0.5f) //this is for avoid noise, sometimes the accelerometer bounce back and it also exceed the threshold                     {                         //copy single wave buffer to wave buffer                         PushToAccelXWaveBuffer(accelXSingleWaveBuffer);                          lastExceedEndTime = Time.realtimeSinceStartup;                          //calculate the peak                         CalculateAccelXPeakAndBroadcastResult(accelXSingleWaveBuffer);                     }                  }             }              //visualize wave buffer             VisualizeAccelWaveBuffer();
        }     }      private bool IsAccelXThresholdExceeded(float accelX)     {         return accelX <= THRESHOLD_ACCEL_X;     }      private void PushToBuffer(Vector3 accel)     {         if (accelBuffer.Count >= ACCEL_BUFFER_SIZE)             accelBuffer.RemoveAt(0);         accelBuffer.Add(accel);     }      private void PushToAccelXWaveBuffer(List<float> accelXWave)     {         if (accelXWaveBuffer.Count >= ACCEL_WAVE_BUFFER_SIZE)             accelXWaveBuffer.RemoveAt(0);          List<float> newList = new List<float>(accelXWave.ToArray());         accelXWaveBuffer.Add(newList);     }      private void PushToAccelXPeakBuffer(float peak)     {         if (accelXPeakBuffer.Count >= ACCEL_WAVE_BUFFER_SIZE)             accelXPeakBuffer.RemoveAt(0);         accelXPeakBuffer.Add(peak);     }      private void CalculateAccelXPeakAndBroadcastResult(List<float> accelXWave)     {         //calculate the max value         float accelXMax = GetMax(accelXWave);          //push the value to peak buffer (for visualization only)         PushToAccelXPeakBuffer(accelXMax);          //generate the sensor result and broadcast the event!         //TODO: !!!!!!!!!!!!!!!!!!!!to be changed to be based on Gyro!!!!!!!!!!!!!!!!!!!!!         this.rowingDirection = Vector3.forward;         this.rowingForce = accelXMax;         OnRowed(this.rowingDirection, this.rowingForce);     }      private float GetMax(List<float> wave)     {         float maxValue = 0f;         foreach (float waveValue in wave)         {             float waveValueAbs = Math.Abs(waveValue);             if (waveValueAbs > maxValue)                 maxValue = waveValueAbs;         }         return maxValue;     }      private void VisualizeAccelBuffer()     {         lineRendererAccelX.positionCount = accelBuffer.Count;         lineRendererAccelY.positionCount = accelBuffer.Count;         lineRendererAccelZ.positionCount = accelBuffer.Count;         for (int i = 0; i < accelBuffer.Count; i ++)         {             lineRendererAccelX.SetPosition(i, new Vector3(i / timeScale, accelBuffer[i].x / dataScale, 0));             lineRendererAccelY.SetPosition(i, new Vector3(i / timeScale, accelBuffer[i].y / dataScale, 0));             lineRendererAccelZ.SetPosition(i, new Vector3(i / timeScale, accelBuffer[i].z / dataScale, 0));         }     }      private void VisualizeAccelWaveBuffer()     {         int spaceLength = 8;         int totalCount = 0;         for(int i = 0; i < accelXWaveBuffer.Count; i ++)         {             totalCount += accelXWaveBuffer[i].Count + (spaceLength * 2);         }         lineRendererAccelXWave.positionCount = totalCount;          int index = 0;         foreach (List<float> wave in accelXWaveBuffer)         {             //print some space before the wave             for (int i = 0; i < spaceLength; i++)             {                 lineRendererAccelXWave.SetPosition(index, new Vector3(index / timeScale, 0, 0));                 index++;             }              foreach (float value in wave)             {                 lineRendererAccelXWave.SetPosition(index, new Vector3(index / timeScale, Math.Abs(value) / (dataScale / 2.2f), 0));                 index++;             }              //print some space after the wave             for (int i = 0; i < spaceLength; i++)             {                 lineRendererAccelXWave.SetPosition(index, new Vector3(index / timeScale, 0, 0));                 index++;             }         }          for(int i = 0; i < accelXPeakBuffer.Count; i ++)         {             string peakValueStr = accelXPeakBuffer[i].ToString("F2");             if (i == accelXPeakBuffer.Count - 1)                 textPeakValueLatest.GetComponent<Text>().text = peakValueStr;         }      } } 