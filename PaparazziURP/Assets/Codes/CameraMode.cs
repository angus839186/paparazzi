using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMode : MonoBehaviour
{
    private RectTransform CameraScreen;
    private float targetScale;
    private float scale;

    private void Awake()
    {
        CameraScreen = GetComponent<RectTransform>();
        targetScale = CameraScreen.localScale.x;
        scale = targetScale;
    }
    private void Update()
    {
        float fovSpeed = 6f;
        scale = Mathf.Lerp(scale, targetScale, Time.deltaTime * fovSpeed);
        CameraScreen.localScale = new Vector3(scale, CameraScreen.localScale.y, CameraScreen.localScale.z);
    }
    public void SetCameraMode(float _targetScale)
    {
        this.targetScale = _targetScale;
        Debug.Log("ChangeScale" + _targetScale);
    }
}
