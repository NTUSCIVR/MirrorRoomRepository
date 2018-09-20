using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//manage the events that occurs in the application
public class GameController : MonoBehaviour {
    
    public static GameController Instance;

    [Tooltip("Place all existing objects of player models in this list to enable swapping")]
    public List<GameObject> playerModels;
    int modelIndex;
    int modelPrevIndex;
    float userHeight = 1.8f;

    [Tooltip("Place all existing mirrors in this list to enable swapping")]
    public List<GameObject> mirrors;
    int mirrorIndex;
    int mirrorPrevIndex;

    [Tooltip("Place all types of hair that is used with avatarSDK")]
    public List<GameObject> hairs;

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
        modelIndex = 0;
        playerModels[modelIndex].SetActive(true);
        ScaleModel(playerModels[modelIndex]);

        mirrorIndex = 0;
        mirrors[mirrorIndex].SetActive(true);
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
            IncreaseModelIndex();
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            DecreaseModelIndex();
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            IncreaseMirrorIndex();
        }
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
        ScaleModel(playerModels[modelIndex]);
    }

    void ActivateMirror()
    {
        mirrors[mirrorPrevIndex].SetActive(false);
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
        ScaleModel(playerModels[modelIndex]);
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
