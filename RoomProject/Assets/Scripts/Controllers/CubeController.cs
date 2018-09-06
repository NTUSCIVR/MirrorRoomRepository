using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour {

    public float timeToDespawn = 1f;
    float time = 0;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<Rigidbody>().useGravity)
        {
            time += Time.deltaTime;
            if (time > timeToDespawn)
                Destroy(gameObject);
        }
	}

    //scale down the cube if they collide to prevent them from getting stucked
    private void OnCollisionStay(Collision collision)
    {
        transform.localScale = transform.localScale * 0.9999f;
    }
}
