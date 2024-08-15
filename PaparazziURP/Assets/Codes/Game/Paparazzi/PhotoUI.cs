using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotoUI : MonoBehaviourPunCallbacks
{
    public PhotonView pv;

    [Header("References")]
    public Slider ProgressSlider;

    public Text currentPhotoText;
    public int currentPhotoOnCamera;
    public int maxPhotoOnCamera = 3;

    [Header("失去相片進度_冷卻時間")]
    public float LosingPictureCD; 
    public float LosingPictureCDtimer;

    [Header("當前相片進度")]
    public float currentProgress;
    public float maxProgress = 100f;

    public Sprite healthy;
    public Sprite unhealthy;
    public Sprite RecIng;
    public Image statusImage;

    public PaparazziCameraObject cameraObject;
    private void Start()
    {
           UpdatePhotoUIState();
    }
    private void Update()
    {
        if(pv.IsMine)
        {
            LosingPicture();
            CameraStateUI();
        }
    }

    private void CameraStateUI()
    {
        if (cameraObject.HaveCamera == false)
        {
            statusImage.sprite = unhealthy;
        }
        else
        {
            if (currentProgress != 0)
            {
                statusImage.sprite = RecIng;
            }
            else
            {
                statusImage.sprite = healthy;
            }
        }
    }

    public void UpdatePhotoUIState()
    {
        if(pv.IsMine)
        {
            ProgressSlider.value = currentProgress / maxProgress;
            currentPhotoText.text = currentPhotoOnCamera + "/" + maxPhotoOnCamera;
        }

    }
    public void LosingPicture()
    {
        if (LosingPictureCDtimer > 0)
        {
            LosingPictureCDtimer -= Time.deltaTime;
        }
        if (LosingPictureCDtimer < 0)
        {
            StartCoroutine(LosingPhotoProgress());
        }
    }
    IEnumerator LosingPhotoProgress()
    {
        if (currentProgress > 0)
        {
            currentProgress -= Time.deltaTime * 4;
        }
        if (currentProgress < 0)
        {
            currentProgress = 0;
        }
        ProgressSlider.value = currentProgress / maxProgress;
        yield return null;

    }
    public void SetPhotoCount()
    {
        if(pv.IsMine)
        {
            int currentPhotoCount = currentPhotoOnCamera;
            Hashtable hash = new Hashtable();
            hash.Add("currentPhotoCount", currentPhotoCount);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps["currentPhotoCount"] != null)
        {
            if (!pv.IsMine && targetPlayer == pv.Owner)
            {
                currentPhotoOnCamera = (int)changedProps["currentPhotoCount"];
            }
        }
    }


}
