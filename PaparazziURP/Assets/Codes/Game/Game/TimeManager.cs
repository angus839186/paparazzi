using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;
using Photon.Realtime;

public class TimeManager : MonoBehaviourPunCallbacks, IPunObservable
{

    public static TimeManager instance;

    public int prepareTime;
    public int gameTime;
    public bool prepared;
    public bool gameEnd;

    public PhotonView pv;

    public GameSceneManager GM;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        StartCountDown();
    }
    public void StartCountDown()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine("LoseTime");

        }
    }
    IEnumerator LoseTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if(PhotonNetwork.IsMasterClient)
            {
                ReduceTime();
            }
        }
    }
    private void ReduceTime()
    {
        if(!prepared)
        {
            prepareTime--;
            if (prepareTime <= 0)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    pv.RPC("RPC_GameStart", RpcTarget.AllViaServer);
                }
            }
            return;
        }
        else
        {
            gameTime--;
            if (gameTime <= 0)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    pv.RPC("RPC_GameEnd", RpcTarget.AllViaServer);
                }
            }
        }
    }

    [PunRPC]
    public void RPC_GameStart()
    {
        prepared = true;
    }

    [PunRPC]
    public void RPC_GameEnd()
    {
        StopCoroutine("LoseTime");
        gameEnd = true;
        GM.GameStatusCheck();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(prepareTime);
            stream.SendNext(gameTime);
        }
        else if(stream.IsReading)
        {
            prepareTime = (int)stream.ReceiveNext();
            gameTime = (int)stream.ReceiveNext();
        }
    }
}
