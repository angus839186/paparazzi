using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UploadedPhotoUI : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    public GameSceneManager GM;
    [Header("已上傳相片")]
    public int currentPhoto;
    public int MaxUploadedPhoto;


    public void UploadPhoto(int photo)
    {
        pv.RPC("RPC_SetUpLoadPhoto", RpcTarget.All, photo);
    }

    [PunRPC]
    void RPC_SetUpLoadPhoto(int photo)
    {
        currentPhoto += photo;
        GM.GameStatusCheck();
        SetUploadPhotoCount();
    }

    public void SetUploadPhotoCount()
    {
        if (pv.IsMine)
        {
            int UploadPhotoCount = currentPhoto;
            Hashtable hash = new Hashtable();
            hash.Add("UploadPhotoCount", UploadPhotoCount);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps["UploadPhotoCount"] != null)
        {
            if (!pv.IsMine && targetPlayer == pv.Owner)
            {
                currentPhoto = (int)changedProps["UploadPhotoCount"];
                if(GM != null)
                {
                    GM.GameStatusCheck();
                }
            }
        }
    }

}
