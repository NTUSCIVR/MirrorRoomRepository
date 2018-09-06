using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlankEvent : MonoBehaviour {

    bool fall = false;
    [SerializeField]
    GameObject plankA;
    [SerializeField]
    GameObject plankB;
    [SerializeField]
    GameObject fallArea;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Alpha1))
        {
            if(!fall)
            {
                fall = true;
                plankA.GetComponent<Rigidbody>().isKinematic = false;
                plankB.GetComponent<Rigidbody>().isKinematic = false;
                fallArea.SetActive(true);
            }
        }
	}
}
