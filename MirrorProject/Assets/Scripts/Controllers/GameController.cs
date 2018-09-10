using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//manage the events that occurs in the application
public class GameController : MonoBehaviour {
    
    public static GameController Instance;

    public List<GameObject> playerModels;
    int index;
    int prevIndex;
    float userHeight = 1.8f;

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
        index = 0;
        playerModels[index].SetActive(true);
        ScaleModel(playerModels[index]);
    }
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("StartScene");
            Destroy(DataCollector.Instance.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            CalibrateHeight();
        }

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            IncreaseIndex();
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            DecreaseIndex();
        }
    }

    void ActivateModel()
    {
        //set previous model to inactive
        playerModels[prevIndex].SetActive(false);
        //set current model to active
        playerModels[index].SetActive(true);
        ScaleModel(playerModels[index]);
    }

    void CalibrateHeight()
    {
        //calibrate the scale of all objects currently registered using the current camera rig height as a reference
        //all the models need a box collider to represent its height
        //need find the curr camera in use to get height
        foreach(SteamVR_Camera go in FindObjectsOfType<SteamVR_Camera>())
        {
            if(go.gameObject.activeInHierarchy)
            {
                userHeight = go.transform.position.y;
                break;
            }
        }
        ScaleModel(playerModels[index]);
    }

    void ScaleModel(GameObject model)
    {
        //the model itself
        GameObject go = GetModelObject(model);

        float modelHeight = model.GetComponent<BoxCollider>().bounds.extents.y * 2;
        //get the scale needed
        float ratio = userHeight / modelHeight;
        go.transform.localScale = new Vector3(ratio, ratio, ratio);
    }

    GameObject GetModelObject(GameObject model)
    {
        //get the model object child
        foreach(Transform child in model.transform)
        {
            if (child.tag == "Player")
                return child.gameObject;
        }
        Debug.LogWarning("Model itself does not have a player tag, please check again");
        return null;
    }

    public void IncreaseIndex()
    {
        prevIndex = index;
        ++index;
        if(index >= playerModels.Count)
        {
            index = 0;
        }
        ActivateModel();
    }

    public void DecreaseIndex()
    {
        prevIndex = index;
        --index;
        if(index < 0)
        {
            index = playerModels.Count - 1;
        }
        ActivateModel();
    }
}
