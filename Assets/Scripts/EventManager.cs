using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

    //Rowing event
    public delegate void RowAction(Vector3 direction, float force);
    public static event RowAction OnRowed;

    private Vector3 rowingDirection;
    private float rowingForce;
    
    void Start () {
		
	}

	void Update () {
		if(RowingDetected()) {
            OnRowed(rowingDirection, rowingForce);
        }
	}

    //The key algorithms to detect the rowing action
    private bool RowingDetected() {
        //play with the data from gyroscope and accelerometer
        


        //Add calculation algorithms here!
        rowingDirection = new Vector3(0, 0, 0);
        rowingForce = 0f;

        return true;
    }
}
