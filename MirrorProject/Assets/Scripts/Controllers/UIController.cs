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
    [Tooltip("Prefab for creating in content for icons of selections")]
    public GameObject headIconPrefab;

    public bool headSelected = false;
    public bool hairSelected = false;
    public bool bodySelected = false;

    string userImagesDir;

	// Use this for initialization
	void Start () {
#if UNITY_EDITOR
        userImagesDir = Application.dataPath + "/UserPhotos";
#else
        userImagesDir = Application.dataPath + "/UserPhotos";
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

                break;
            case SELECTION_TYPE.BODY:

                break;
        }
    }

    public void PopulateHeadIcons()
    {
        DirectoryInfo d = new DirectoryInfo(userImagesDir);
        //Getting user images files
        FileInfo[] files = d.GetFiles("*.jpg"); 
        foreach(FileInfo file in files)
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
            if(texture.width > texture.height)
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

    //remove all existing buttons in the scrollview
    public void ClearContent()
    {
        for(int i = 0; i < content.transform.childCount - 1; ++i)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
    }

    public void OnHeadSelect(GameObject btn)
    {
        DataCollector.Instance.imagePath = userImagesDir + "/" + btn.GetComponentInChildren<Text>().text;
    }

    public void OnHairSelect()
    {

    }

    public void OnBodySelect()
    {

    }
}
