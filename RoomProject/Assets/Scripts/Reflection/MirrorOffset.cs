using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorOffset : MonoBehaviour {

    public enum Directions { X, Y, Z};
    public Directions orientation;

    public GameObject mirror;
    public GameObject player;

    private float offset;

    private Vector3 probePos;
	
	// Update is called once per frame
	void Update () {
        switch (orientation)
        {
            case Directions.X:
                offset = mirror.transform.position.x - player.transform.position.x;
                probePos.x = mirror.transform.position.x + offset;
                probePos.y = mirror.transform.position.y;
                probePos.z = mirror.transform.position.z;
                break;
            case Directions.Y:
                offset = mirror.transform.position.y - player.transform.position.y;
                probePos.x = mirror.transform.position.x;
                probePos.y = mirror.transform.position.y + offset;
                probePos.z = mirror.transform.position.z;
                break;
            case Directions.Z:
                offset = mirror.transform.position.z - player.transform.position.z;
                probePos.x = mirror.transform.position.x;
                probePos.y = mirror.transform.position.y;
                probePos.z = mirror.transform.position.z + offset;
                break;
        }
        transform.position = probePos;
	}
}
