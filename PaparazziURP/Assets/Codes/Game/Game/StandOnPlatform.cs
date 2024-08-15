using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandOnPlatform : MonoBehaviour
{
    public PhotonView pv;
    public void ChangeParent(int PVID)
    {
        pv.RPC("RPC_ChangeParent", RpcTarget.AllBuffered, PVID);
    }

    [PunRPC]
    private void RPC_ChangeParent(int newParentViewID)
    {
        Transform newParent = PhotonView.Find(newParentViewID).transform;
        Debug.Log(newParent);
        transform.SetParent(newParent);
    }
    public void LeaveParent()
    {
        pv.RPC("RPC_LeaveParent", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_LeaveParent()
    {
        transform.SetParent(null);
    }
}
