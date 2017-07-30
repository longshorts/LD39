using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationFixer : MonoBehaviour {

    [SerializeField] Transform robot;
    Quaternion originalRotation;
    Vector3 playerAdjustment;

	// Use this for initialization
	void Start () {
        originalRotation = transform.rotation;
        playerAdjustment = transform.position - robot.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = originalRotation;
        transform.position = robot.position + playerAdjustment;
    }
}
