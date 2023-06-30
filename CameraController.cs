using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    
    public Transform target;

    private float startFOV, targetFOV;

    public float zoomSpeed;

    public Camera theCam;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        startFOV = theCam.fieldOfView;
        targetFOV = startFOV;
    }

    public void StartDelay(Transform cameraPivot)
    {
        target = cameraPivot;
        transform.position = target.position;
        transform.rotation = target.rotation;
        transform.SetParent(target);
    }
    public void LateUpdate()
    {
        theCam.fieldOfView = Mathf.Lerp(theCam.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
    }
    public void ZoomIn(float newZoom)
    {
        targetFOV = newZoom;
    }
    public void ZoomOut()
    {
        targetFOV = startFOV;
    }
}
