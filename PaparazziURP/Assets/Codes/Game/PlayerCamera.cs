using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class PlayerCamera : MonoBehaviourPunCallbacks
{
    public PhotonView pv;

    [Header("²Ä¤@¤HºÙ")]
    public Camera cam;
    public float mouseSensitivity;
    public float verticalLookRotation;

    public bool stopCamera;
    void Start()
    {
        if (!pv.IsMine)
        {
            cam.enabled = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pv.IsMine)
        {
            if(stopCamera)
            {
                return;
            }
            else
            {
                Look();
            }
        }
    }
    public void Look()
    {
        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -40f, 70f);

        cam.transform.localEulerAngles = Vector3.left * verticalLookRotation;
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);
    }
}
