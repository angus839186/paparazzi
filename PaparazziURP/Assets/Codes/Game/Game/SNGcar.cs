using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SNGcar : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    public GameSceneManager GM;
    [Header("相機數量")]
    public int CameraAmount;
    public int MaxCameraAmount;


    public void CameraAmountChange()
    {
        pv.RPC("RPC_SetCameraAmount", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_SetCameraAmount()
    {
        CameraAmount--;
        GM.GameStatusCheck();
        SetCameraAmount();
    }

    public void SetCameraAmount()
    {
        if (pv.IsMine)
        {
            int cameraAmount = CameraAmount;
            Hashtable hash = new Hashtable();
            hash.Add("cameraAmount", cameraAmount);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps["cameraAmount"] != null)
        {
            if (!pv.IsMine && targetPlayer == pv.Owner)
            {
                CameraAmount = (int)changedProps["cameraAmount"];
                if(GM != null)
                {
                    GM.GameStatusCheck();
                }
            }
        }
    }
}
