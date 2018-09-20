using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;

//incharge of managing the ui elements on start scene
public class UIController : MonoBehaviour {

    public enum SELECTION_TYPE
    {
        HEAD,
        HAIR,
        BODY
    }

    [Tooltip("Current type selected from scrollview")]
    public SELECTION_TYPE type = SELECTION_TYPE.HEAD;
    
    [Tooltip("Current scrollview content in use in the scene")]
    public GameObject content;
    [Tooltip("Prefab for the icons shown in the scrollview for head")]
    public GameObject headIconPrefab;
    [Tooltip("Prefab for the icons shown in the scrollview for hair")]
    public GameObject hairIconPrefab;

    [Tooltip("Reference to the helper text for face")]
    public GameObject headText;
    [Tooltip("Reference to the helper text for hair")]
    public GameObject hairText;

    public bool headSelected = false;
    public bool hairSelected = false;

    string userImagesDir;
    string hairPhotosDir;

	// Use this for initialization
	void Start () {
#if UNITY_EDITOR
        userImagesDir = Application.dataPath + "/UserPhotos";
        hairPhotosDir = Application.dataPath + "/Textures/Hair";
#else
        userImagesDir = Application.dataPath + "/UserPhotos";
        hairPhotosDir = Application.dataPath + "/Hair";
#endif
        PopulateScrollview();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //populate scrollview with all selections of current type and remove all previous gameobjects
    public void PopulateScrollview()
    {
        ClearContent();
        switch(type)
        {
            case SELECTION_TYPE.HEAD:
                PopulateHeadIcons();
                break;
            case SELECTION_TYPE.HAIR:
                PopulateHairIcon();
                break;
        }
    }

    public void PopulateHeadIcons()
    {
        DirectoryInfo d = new DirectoryInfo(userImagesDir);
        //Getting user images files
        FileInfo[] files = d.GetFiles("*.jpg");
        foreach (FileInfo file in files)
        {
            //create the head icon game object
            GameObject headIcon = Instantiate(headIconPrefab, content.transform);
            //add onclick function
            headIcon.GetComponent<Button>().onClick.AddListener(delegate { OnHeadSelect(headIcon); });

            //read the bytes for the user image
            byte[] bytes = File.ReadAllBytes(userImagesDir + "/" + file.Name);
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
            texture.filterMode = FilterMode.Trilinear;
            texture.LoadImage(bytes);
            Sprite sprite;
            if (texture.width > texture.height)
            {
                //if the image is landscape
                int horizontalOffset = (texture.width - texture.height) / 2;
                //create a square rect from the landscape image from the center
                Vector2 min = new Vector2(horizontalOffset, 0);
                Vector2 max = new Vector2(texture.width - horizontalOffset, texture.height);
                sprite = Sprite.Create(texture, new Rect(min, max - min), Vector2.zero);
            }
            else if (texture.height > texture.width)
            {
                //if the image is portrait
                int verticalOffset = (texture.height - texture.width) / 2;
                //create a square rect from the portrait image from the center
                Vector2 min = new Vector2(0, verticalOffset);
                Vector2 max = new Vector2(texture.width, texture.height - verticalOffset);
                sprite = Sprite.Create(texture, new Rect(min, max - min), Vector2.zero);
            }
            else
            {
                //if the image is already a square
                sprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.zero);
            }
            //assign the sprite to icon
            headIcon.GetComponent<Image>().sprite = sprite;
            //assign the name of the image
            headIcon.GetComponentInChildren<Text>().text = file.Name;
        }
    }

    public void PopulateHairIcon()
    {
        DirectoryInfo d = new DirectoryInfo(hairPhotosDir);
        //Getting user images files
        FileInfo[] files = d.GetFiles("*.jpg");
        foreach (FileInfo file in files)
        {
            //create the head icon game object
            GameObject hairIcon = Instantiate(hairIconPrefab, content.transform);
            //add onclick function
            hairIcon.GetComponent<Button>().onClick.AddListener(delegate { OnHairSelect(hairIcon); });

            //read the bytes for the user image
            byte[] bytes = File.ReadAllBytes(hairPhotosDir + "/" + file.Name);
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
            texture.filterMode = FilterMode.Trilinear;
            texture.LoadImage(bytes);
            Sprite sprite;
            if (texture.width > texture.height)
            {
                //if the image is landscape
                int horizontalOffset = (texture.width - texture.height) / 2;
                //create a square rect from the landscape image from the center
                Vector2 min = new Vector2(horizontalOffset, 0);
                Vector2 max = new Vector2(texture.width - horizontalOffset, texture.height);
                sprite = Sprite.Create(texture, new Rect(min, max - min), Vector2.zero);
            }
            else if (texture.height > texture.width)
            {
                //if the image is portrait
                int verticalOffset = (texture.height - texture.width) / 2;
                //create a square rect from the portrait image from the center
                Vector2 min = new Vector2(0, verticalOffset);
                Vector2 max = new Vector2(texture.width, texture.height - verticalOffset);
                sprite = Sprite.Create(texture, new Rect(min, max - min), Vector2.zero);
            }
            else
            {
                //if the image is already a square
                sprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.zero);
            }
            //assign the sprite to icon
            hairIcon.GetComponent<Image>().sprite = sprite;
            //assign the name of the image
            hairIcon.GetComponentInChildren<Text>().text = file.Name;
        }
    }

    //remove all existing buttons in the scrollview
    public void ClearContent()
    {
        for(int i = 0; i < content.transform.childCount; ++i)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
    }

    public void OnHeadSelect(GameObject btn)
    {
        DataCollector.Instance.imagePath = userImagesDir + "/" + btn.GetComponentInChildren<Text>().text;
        if (!headSelected)
            headSelected = true;
        headText.GetComponent<Text>().text = "Image selected for face is " + btn.GetComponentInChildren<Text>().text;
        headText.GetComponent<Text>().color = Color.green;
    }

    public void OnHairSelect(GameObject btn)
    {
        string hairName = "";
        string fileName = btn.GetComponentInChildren<Text>().text;
        foreach (char c in fileName)
        {
            if (c == '.')
                break;
            else
                hairName += c;
        }
        DataCollector.Instance.hairIndex = int.Parse(hairName);
        if (!hairSelected)
            hairSelected = true;
        hairText.GetComponent<Text>().text = "Hair selected index is " + DataCollector.Instance.hairIndex;
        hairText.GetComponent<Text>().color = Color.green;
    }

    public void OnHeadButtonPressed()
    {
        type = SELECTION_TYPE.HEAD;
        PopulateScrollview();
    }

    public void OnHairButtonPressed()
    {
        type = SELECTION_TYPE.HAIR;
        PopulateScrollview();
    }

    public void OnStartButtonPressed()
    {
        //check if hair and head has been selected
        if(hairSelected && headSelected)
        {
            //tell datacollector to start the next scene
            DataCollector.Instance.Submit();
        }
    }
}
