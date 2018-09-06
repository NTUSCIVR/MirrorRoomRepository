using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataCollector : MonoBehaviour {

    public InputField inputField;
    public string dataID = "";
    public static DataCollector Instance;

    public bool startRecording = false;
    public float dataRecordInterval = 1f;
    float time = 0f;
    
    public GameObject user;

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
                StreamWriter sw = File.AppendText(GetPath());
                sw.WriteLine(GenerateData());
                sw.Close();
            }
        }
	}

    void OnInputSubmitCallback()
    {
        Debug.Log("change scene");
        dataID = inputField.text;
        CreateCSV();
        startRecording = true;
        SceneManager.LoadScene("MainScene");
    }

    void AssignInputField()
    {
        inputField = FindObjectOfType<InputField>();
        inputField.onEndEdit.AddListener(delegate { OnInputSubmitCallback(); });
    }

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

    private string GetPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/Data/" + dataID + ".csv";
#elif UNITY_ANDROID
        return Application.persistentDataPath+dataID + ".csv";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+dataID + ".csv";
#else
        return Application.dataPath +"/"+dataID + ".csv";
#endif
    }

    void CreateCSV()
    {
        if(File.Exists(GetPath()))
        {
            File.Delete(GetPath());
        }
        StreamWriter output = System.IO.File.CreateText(GetPath());
        output.WriteLine("Time, Position, Rotation");
        output.Close();
    }

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
