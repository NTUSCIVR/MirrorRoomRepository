using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//manage the events that occurs in the application
public class GameController : MonoBehaviour {

    public static GameController Instance;
    bool dropped = false;
    public GameObject cameraRig;
    public GameObject collaspeFloor;

    public bool fall = false;
    float fallSpeed = 0f;

    string userID;

    private void Awake()
    {
        Instance = this;
        //find the steamvr eye and assign it to data collector
        if (DataCollector.Instance != null)
        {
            DataCollector.Instance.user = FindObjectOfType<SteamVR_Camera>().gameObject;
            userID = DataCollector.Instance.dataID;
        }
    }

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKey(KeyCode.Return))
        {
            if (!dropped)
            {
                AudioController.Instance.PlaySingle(AudioController.Instance.openHole);
                EngageFloorDrop();
                ActiveFallZones();
            }
        }
        if(Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene("StartScene");
            Destroy(DataCollector.Instance.gameObject);
        }

        if(fall)
        {
            fallSpeed += 10f * Time.deltaTime;
            cameraRig.transform.position = new Vector3(0f, cameraRig.transform.position.y - fallSpeed * Time.deltaTime, 0f);
            if(cameraRig.transform.position.y < -10)
            {
                AudioController.Instance.PlaySingle(AudioController.Instance.fall);
                fall = false;
                cameraRig.transform.position = new Vector3(0f, -10f, 0);
            }
        }
    }
    
    void EngageFloorDrop()
    {
        dropped = true;
        //itterate through all of the children to disable kinematci and enable gravity
        for (int i = collaspeFloor.transform.childCount - 1; i >= 0; --i)
        {
            Rigidbody childRB = collaspeFloor.transform.GetChild(i).GetComponent<Rigidbody>();
            childRB.isKinematic = false;
            childRB.useGravity = true;
        }
    }

    //activate all fallzones that is a child of game controller
    void ActiveFallZones()
    {
        for(int i = transform.childCount - 1; i >= 0; --i)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if(child.tag == "FallArea")
            {
                child.SetActive(true);
            }
        }
    }
}
