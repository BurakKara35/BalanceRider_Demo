using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float cameraRotationX;
    private float cameraRotationZ;

    [SerializeField] private Transform LookAtTarget;
    [SerializeField] private Transform followingTarget;
 
    private void Awake()
    {
        cameraRotationX = transform.eulerAngles.x;
        cameraRotationZ = transform.eulerAngles.z;

        transform.position = followingTarget.position;
    }

    private void FixedUpdate()
    {
        Follow();
        Look();
    }

    private void Follow()
    {
        transform.position = Vector3.Lerp(transform.position, followingTarget.position, 0.3f);
    }

    private void Look()
    {
        transform.eulerAngles = new Vector3(cameraRotationX, LookAtTarget.eulerAngles.y, cameraRotationZ);
    }
}
