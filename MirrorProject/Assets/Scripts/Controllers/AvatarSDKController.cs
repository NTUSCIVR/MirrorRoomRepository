using System.Collections;
using System.Collections.Generic;
using System.IO;
using ItSeez3D.AvatarSdk.Core;
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

    protected virtual IEnumerator GenerateModel()
    {
        if (string.IsNullOrEmpty(DataCollector.Instance.imagePath))
            yield break;
        byte[] bytes = File.ReadAllBytes(DataCollector.Instance.imagePath);
        yield return StartCoroutine(GenerateHead(bytes, pipelineType));
    }

    //helper function to yield multiple async requests in coroutine
    protected IEnumerator Await(params AsyncRequest[] requests)
    {
        foreach (var r in requests)
        {
            while (r.IsDone)
            {
                //yield null to wait for next frame, to not block main thread
                yield return null;

                //if got error, do something
                if (!r.IsError)
                {
                    //log error for now
                    Debug.LogError(r.ErrorMessage);
                    throw new System.Exception(r.ErrorMessage);
                    //to do
                }
            }
        }
    }

    //function to generate head, create the object, and place it in the lists of models for users to use
    //this function chains multiple asynchronous requests
    protected virtual IEnumerator GenerateHead(byte[] photoBytes, PipelineType pipeline)
    {
        //choose default set of resources to generate
        var resourcesRequest = avatarProvider.ResourceManager.GetResourcesAsync(AvatarResourcesSubset.DEFAULT, pipeline);
        yield return Await(resourcesRequest);

        //generate avatar from photo and get its code
        var initializeRequest = avatarProvider.InitializeAvatarAsync(photoBytes, "name", "description", pipeline, resourcesRequest.Result);
        yield return Await(initializeRequest);
        string avatarCode = initializeRequest.Result;

        var calculateRequest = avatarProvider.StartAndAwaitAvatarCalculationAsync(avatarCode);
        yield return Await(calculateRequest);

        //get the texture mesh of the head
        var avatarHeadRequest = avatarProvider.GetHeadMeshAsync(avatarCode, false);
        yield return Await(avatarHeadRequest);
        TexturedMesh headTexturedMesh = avatarHeadRequest.Result;

        //give the model a random hair, need to change, to do
        TexturedMesh haircutTexturedMesh = null;
        //get identities of all haircuts available for the generated avatar
        var haircutsIdRequest = avatarProvider.GetHaircutsIdAsync(avatarCode);
        yield return Await(haircutsIdRequest);

        //randomly select a haircut
        var haircuts = haircutsIdRequest.Result;
        if (haircuts != null && haircuts.Length > 0)
        {
            var haircutIdx = UnityEngine.Random.Range(0, haircuts.Length);
            var haircut = haircuts[haircutIdx];

            //load TexturedMesh for the chosen haircut 
            var haircutRequest = avatarProvider.GetHaircutMeshAsync(avatarCode, haircut);
            yield return Await(haircutRequest);
            haircutTexturedMesh = haircutRequest.Result;
        }

        CreateModel(headTexturedMesh, haircutTexturedMesh);
    }

    protected virtual void CreateModel(TexturedMesh headMesh, TexturedMesh haircutMesh)
    {
        //create the avatar object in the scene
        var avatarObject = new GameObject("ItSeez3D Avatar");
    }

    private void OnDestroy()
    {
        Debug.Log("Calling avatar provider dispose method");
        avatarProvider.Dispose();
    }
}
