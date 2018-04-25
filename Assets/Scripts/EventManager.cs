using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

    //Rowing event
    public delegate void RowAction(Vector3 direction, float force);
    public static event RowAction OnRowed;

    private Vector3 rowingDirection;
    private float rowingForce;
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(RowingDetected()) {
            OnRowed(rowingDirection, rowingForce);
        }
	}

    //The key algorithms to detect the rowing action
    private bool RowingDetected() {
        //play with the data from gyroscope and accelerometer
        //gyroxtext.getcomponent<text>().text = gvrcontrollerinput.gyro.x.tostring("f2");
        //gyroytext.getcomponent<text>().text = gvrcontrollerinput.gyro.y.tostring("f2");
        //gyroztext.getcomponent<text>().text = gvrcontrollerinput.gyro.z.tostring("f2");
        //accelxtext.getcomponent<text>().text = gvrcontrollerinput.accel.x.tostring("f2");
        //accelytext.getcomponent<text>().text = gvrcontrollerinput.accel.y.tostring("f2");
        //accelztext.getcomponent<text>().text = gvrcontrollerinput.accel.z.tostring("f2");

        rowingDirection = new Vector3(0, 0, 0);
        rowingForce = 0f;

        return true;
    }
}
