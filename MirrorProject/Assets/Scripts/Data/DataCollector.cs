﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataCollector : MonoBehaviour {

    [Tooltip("InputField reference to allow user to key in ID")]
    public InputField inputField;
    [Tooltip("ID of the current user for use in other scene, no need to touch")]
    public string dataID = "";
    public static DataCollector Instance;

    [Tooltip("Boolean to know if DataCollector should start recording")]
    public bool startRecording = false;
    [Tooltip("Intervals between recording of data in seconds")]
    public float dataRecordInterval = 1f;
    float time = 0f;

    string currentFilePath;
    
    [Tooltip("GameObject reference to camera rig in main scene to access attributes for recording")]
    public GameObject user;

    [Tooltip("Selected user image filepath")]
    public string imagePath;
    [Tooltip("Selected hair index")]
    public int hairIndex;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        AssignInputField();
    }

    // Use this for initialization
    void Start () {
        Instance = this;
    }
	
	// Update is called once per frame
	void Update () {
        if(startRecording)
        {
            time += Time.deltaTime;
            if (time > dataRecordInterval)
            {
                time = 0;
                StreamWriter sw = File.AppendText(currentFilePath + "/" + dataID + ".csv");
                sw.WriteLine(GenerateData());
                sw.Close();
            }
        }
	}

    public void Submit()
    {
        Debug.Log("change scene");
        dataID = inputField.text;
        CreateDataElement();
        startRecording = true;
        SceneManager.LoadScene("MainScene");
    }

    void AssignInputField()
    {
        inputField = FindObjectOfType<InputField>();
    }

    //this generate in a string format for time, position and rotation
    string GenerateData()
    {
        string data = "";
        data += System.DateTime.Now.ToString("HH");
        data += ":";
        data += System.DateTime.Now.ToString("mm");
        data += ":";
        data += System.DateTime.Now.ToString("ss");
        data += ":";
        data += System.DateTime.Now.ToString("FFF");
        data += ",";
        string posstr = user.GetComponent<SteamVR_Camera>().head.transform.position.ToString("F3");
        data += ChangeLetters(posstr, ',', '.');
        data += ",";
        string rotstr = user.GetComponent<SteamVR_Camera>().head.transform.rotation.ToString("F3");
        data += ChangeLetters(rotstr, ',', '.');
        return data;
    }

    //returns a path for the current dataID, will return a duplicate path if path exists already
    private string GetPath()
    {
        string filePath = Application.dataPath + "/Data/" + dataID;
        int duplicateCounts = 0;
        //checks for duplicate, if got duplicate, put a number clone thing beside the name
        while(true)
        {
            if (Directory.Exists(filePath))
            {
                ++duplicateCounts;
                filePath = Application.dataPath + "/Data/" + dataID + "(" + duplicateCounts.ToString() + ")";
            }
            else
                break;
        }
        return filePath;
    }

    //create a folder with the csv inside it
    void CreateDataElement()
    {
        currentFilePath = GetPath();
        Directory.CreateDirectory(currentFilePath);
        //in case file is unable to be accessed
        File.SetAttributes(currentFilePath, FileAttributes.Normal);
        CreateCSV(currentFilePath);
    }

    //create a csv file with dataID name in directory provided
    void CreateCSV(string filePath)
    {
        StreamWriter output = System.IO.File.CreateText(filePath + "/" + dataID + ".csv");
        output.WriteLine("Time, Position, Rotation");
        output.Close();
    }

    //change all the letters in a string to something else
    string ChangeLetters(string str, char letter, char toBeLetter)
    {
        char[] ret = str.ToCharArray();
        for(int i = 0; i < ret.Length; ++i)
        {
            if(ret[i] == letter)
            {
                ret[i] = toBeLetter;
            }
        }
        return new string(ret);
    }
}
