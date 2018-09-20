using System.Collections;
using System.Collections.Generic;
using System.IO;
using ItSeez3D.AvatarSdk.Core;
using ItSeez3D.AvatarSdkSamples.Core;
using UnityEngine.UI;
using UnityEngine;

//the interface to use the avatarSDK
public class AvatarSDKController : MonoBehaviour {

    //switch between cloud and offline, but keep it as cloud
    public SdkType sdkType;

    //this is a list to contain data of all images stored in a folder
    public List<TextAsset> images;

    //face type generates only the head portion of the user, head type generates the shoulders as well
    protected PipelineType pipelineType = PipelineType.FACE;

    //index for the types of body to generate. all of these are prefabs with IK setup
    public int bodyIndex = 0;
    //list of all prefabs for body
    public List<GameObject> bodies;

    public Text progressText;

    //instance of IAvatarProvider, call dispose when monobehavior destruct
    protected IAvatarProvider avatarProvider = null;

    // Use this for initialization
    void Start()
    {
        //initilize the SDK
        if (!AvatarSdkMgr.IsInitialized)
        {
            AvatarSdkMgr.Init(sdkType: sdkType);
        }

        // Anti-aliasing is required for hair shader, otherwise nice transparent texture won't work.
        // Another option is to use cutout shader, but the look with this shader isn't that great.
#if UNITY_STANDALONE_WIN || UNITY_EDITOR || UNITY_EDITOR
        QualitySettings.antiAliasing = 8;
#else
		QualitySettings.antiAliasing = 4;
#endif

        StartCoroutine(Initialize());
        if(DataCollector.Instance != null)
        {
            //generate the model only if imagepath is available
            GenerateModel();
        }
        else
        {
            //remove all the headless bodies
            foreach(GameObject model in GameController.Instance.playerModels)
            {
                if (model.GetComponentInChildren<BodyAttachment>())
                {
                    //GameController.Instance.playerModels.Remove(model);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //initilize avatar provider
    protected virtual IEnumerator Initialize()
    {
        avatarProvider = AvatarSdkMgr.IoCContainer.Create<IAvatarProvider>();
        yield return Await(avatarProvider.InitializeAsync());
    }

    public void GenerateModel()
    {
        byte[] bytes = File.ReadAllBytes(DataCollector.Instance.imagePath);
        StartCoroutine(GenerateHead(bytes, pipelineType));
    }

    protected virtual IEnumerator GenerateAvatarFunc(byte[] photoBytes)
    {
        yield return StartCoroutine(GenerateHead(photoBytes, pipelineType));
    }

    //helper function to yield multiple async requests in coroutine
    protected IEnumerator Await(params AsyncRequest[] requests)
    {
        foreach (var r in requests)
        {
            while (!r.IsDone)
            {
                //yield null to wait for next frame, to not block main thread
                yield return null;

                //if got error, do something
                if (r.IsError)
                {
                    //log error for now
                    Debug.LogError(r.ErrorMessage);
                    throw new System.Exception(r.ErrorMessage);
                    //to do
                }

                var progress = new List<string>();
                AsyncRequest request = r;
                while (request != null)
                {
                    progress.Add(string.Format("{0}: {1}%", request.State, request.ProgressPercent.ToString("0.0")));
                    request = request.CurrentSubrequest;
                }
                progressText.text = string.Join("\n", progress.ToArray());
            }
        }
    }

    //function to generate head, create the object, and place it in the lists of models for users to use
    //this function chains multiple asynchronous requests
    protected virtual IEnumerator GenerateHead(byte[] photoBytes, PipelineType pipeline)
    {
        //choose default set of resources to generate
        var resourcesRequest = avatarProvider.ResourceManager.GetResourcesAsync(AvatarResourcesSubset.DEFAULT, pipeline);
        Debug.Log("Requesting resources...");
        yield return Await(resourcesRequest);

        //generate avatar from photo and get its code
        var initializeRequest = avatarProvider.InitializeAvatarAsync(photoBytes, "name", "description", pipeline, resourcesRequest.Result);
        Debug.Log("Initlialize request...");
        yield return Await(initializeRequest);
        string avatarCode = initializeRequest.Result;

        var calculateRequest = avatarProvider.StartAndAwaitAvatarCalculationAsync(avatarCode);
        Debug.Log("Calculate Request");
        yield return Await(calculateRequest);

        //get the texture mesh of the head
        var avatarHeadRequest = avatarProvider.GetHeadMeshAsync(avatarCode, false);
        Debug.Log("Head mesh request...");
        yield return Await(avatarHeadRequest);
        TexturedMesh headTexturedMesh = avatarHeadRequest.Result;

        //give the model a random hair, need to change, to do
        TexturedMesh haircutTexturedMesh = null;
        //get identities of all haircuts available for the generated avatar
        var haircutsIdRequest = avatarProvider.GetHaircutsIdAsync(avatarCode);
        Debug.Log("Hair mesh selection request...");
        yield return Await(haircutsIdRequest);

        //randomly select a haircut
        var haircuts = haircutsIdRequest.Result;
        if (haircuts != null && haircuts.Length > 0)
        {
            var haircut = haircuts[DataCollector.Instance.hairIndex];

            //load TexturedMesh for the chosen haircut 
            var haircutRequest = avatarProvider.GetHaircutMeshAsync(avatarCode, haircut);
            Debug.Log("Hair mesh request...");
            yield return Await(haircutRequest);
            haircutTexturedMesh = haircutRequest.Result;
        }

        CreateModel(headTexturedMesh, haircutTexturedMesh);
    }

    protected virtual void CreateModel(TexturedMesh headMesh, TexturedMesh haircutMesh)
    {
        //create the avatar object in the scene
        var avatarObject = new GameObject("ItSeez3D Avatar");// create head object in the scene

        Debug.LogFormat("Generating Unity mesh object for head...");
        var headObject = new GameObject("HeadObject");
        var headMeshRenderer = headObject.AddComponent<SkinnedMeshRenderer>();
        headMeshRenderer.sharedMesh = headMesh.mesh;
        var headMaterial = new Material(Shader.Find("AvatarUnlitShader"));
        headMaterial.mainTexture = headMesh.texture;
        headMeshRenderer.material = headMaterial;
        headObject.transform.SetParent(avatarObject.transform);
        headObject.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = true;

        var meshObject = new GameObject("HaircutObject");
        var meshRenderer = meshObject.AddComponent<SkinnedMeshRenderer>();
        meshRenderer.sharedMesh = haircutMesh.mesh;
        var material = new Material(Shader.Find("AvatarUnlitHairShader"));
        material.mainTexture = haircutMesh.texture;
        meshRenderer.material = material;
        meshObject.transform.SetParent(avatarObject.transform);
        meshObject.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = true;

        //find all the headless bodies that are in the scene
        foreach (GameObject body in GameController.Instance.playerModels)
        {
            if (body.GetComponentInChildren<BodyAttachment>())
            {
                //add head to the body
                body.GetComponentInChildren<BodyAttachment>().AttachHeadToBody(Instantiate(avatarObject));
                body.GetComponentInChildren<BodyAttachment>().RebuildBindpose();
                Destroy(avatarObject);
            }
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Calling avatar provider dispose method");
        avatarProvider.Dispose();
    }
}
