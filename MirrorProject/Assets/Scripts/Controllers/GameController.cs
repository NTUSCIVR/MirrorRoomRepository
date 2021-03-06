﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//manage the events that occurs in the application
public class GameController : MonoBehaviour {
    [Tooltip("Place all existing objects of player models in this list to enable swapping")]
    public List<GameObject> playerModels;
    //the current index of the used model
    int modelIndex;
    //the previous index
    int modelPrevIndex;
    //the current height of the user, can be calibrated to adjust the height of hte model
    float userHeight = 1.8f;

    [Tooltip("Place all existing mirrors in this list to enable swapping")]
    public List<GameObject> mirrors;
    //the current mirror being used
    int mirrorIndex;
    //the previous mirror
    int mirrorPrevIndex;

    //not used as unable to used a different hair on the model
    [Tooltip("Place all types of hair that is used with avatarSDK")]
    public List<GameObject> hairs;

    string userID;

    private void Awake()
    {
       
    }

    // Use this for initialization
    void Start () {
        //find the steamvr eye and assign it to data collector
        if (DataCollector.Instance != null)
        {
            DataCollector.Instance.user = FindObjectOfType<SteamVR_Camera>().gameObject;
            userID = DataCollector.Instance.dataID;
        }
        modelIndex = 0;
        playerModels[modelIndex].SetActive(true);
        ScaleModel(playerModels[modelIndex]);

        mirrorIndex = 0;
        mirrors[mirrorIndex].SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {

        //restarts to the start scene
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("StartScene");
            //start scene will create another datacollector
            Destroy(DataCollector.Instance.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            //change height to the current headset level
            CalibrateHeight();
        }

        //increase model index
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            IncreaseModelIndex();
        }
        //decrease model index
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            DecreaseModelIndex();
        }

        //increase mirror index
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            IncreaseMirrorIndex();
        }
        //decrease mirror index
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            DecreaseMirrorIndex();
        }
    }

    void ActivateModel()
    {
        //set previous model to inactive
        playerModels[modelPrevIndex].SetActive(false);
        //set current model to active
        playerModels[modelIndex].SetActive(true);
        //update model to fit current height
        ScaleModel(playerModels[modelIndex]);
    }

    void ActivateMirror()
    {
        //deactivate previous mirror
        mirrors[mirrorPrevIndex].SetActive(false);
        //activate current mirror
        mirrors[mirrorIndex].SetActive(true);
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
        //scale the model when calibrated height
        ScaleModel(playerModels[modelIndex]);
    }

    void ScaleModel(GameObject model)
    {
        //the model itself
        GameObject go = GetModelObject(model);

        //all models should have a box collider for how large it is
        //multiplying extends by 2 give me the height of the model
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

    public void IncreaseModelIndex()
    {
        modelPrevIndex = modelIndex;
        ++modelIndex;
        if(modelIndex >= playerModels.Count)
        {
            modelIndex = 0;
        }
        ActivateModel();
    }

    public void DecreaseModelIndex()
    {
        modelPrevIndex = modelIndex;
        --modelIndex;
        if(modelIndex < 0)
        {
            modelIndex = playerModels.Count - 1;
        }
        ActivateModel();
    }

    public void IncreaseMirrorIndex()
    {
        mirrorPrevIndex = mirrorIndex;
        ++mirrorIndex;
        if (mirrorIndex >= mirrors.Count)
        {
            mirrorIndex = 0;
        }
        ActivateMirror();
    }

    public void DecreaseMirrorIndex()
    {
        mirrorPrevIndex = mirrorIndex;
        --mirrorIndex;
        if(mirrorIndex < 0)
        {
            mirrorIndex = mirrors.Count - 1;
        }
        ActivateMirror();
    }
}
