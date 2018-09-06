using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallEvent : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //if player enters a fall area, enable gravity on player
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "MainCamera")
        {
            GameController.Instance.fall = true;
            Debug.Log("camera entered fall zone");
        }
    }
}
