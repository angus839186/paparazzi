using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFOV : MonoBehaviour
{
    private Camera playerCamera;
    private float targetFOV;
    private float fov;

    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
        targetFOV = playerCamera.fieldOfView;
        fov = targetFOV;
    }
    private void Update()
    {
        float fovSpeed = 4f;
        fov = Mathf.Lerp(fov, targetFOV, Time.deltaTime * fovSpeed);
        playerCamera.fieldOfView = fov;
    }
    public void SetCameraFOV(float targetFOV)
    {
        this.targetFOV = targetFOV;
    }
}
