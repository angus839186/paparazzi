using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Bar : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView pv;

    public float WineCoolDown;
    public float WineCoolDownTimer;
    public GameObject WineObject;


    [Header("UI")]
    public TMP_Text RefreshText;
    public Image RefreshingImage;

    public GameObject Hint;

    public bool wine;

    public string WineName;

    private void Start()
    {
        WineCoolDownTimer = WineCoolDown;
        wine = false;
    }
    private void Update()
    {
        if (pv.IsMine)
        {
            if (!wine)
            {
                CoolDown();
            }
        }
    }
    private void CoolDown()
    {
        if(!pv.IsMine)
        {
            RefreshingImage.fillAmount = WineCoolDownTimer / WineCoolDown;
            RefreshText.text = WineCoolDownTimer.ToString("0");
        }
        pv.RPC("RPC_syncCoolDown", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_syncCoolDown()
    {
        WineCoolDownTimer -= Time.deltaTime;
        RefreshingImage.fillAmount = WineCoolDownTimer / WineCoolDown;
        RefreshText.text = WineCoolDownTimer.ToString("0");
        if (WineCoolDownTimer <= 0)
        {
            WineStatus(true);
            WineCoolDownTimer = WineCoolDown;
            RefreshingImage.fillAmount = 0f;
            RefreshText.text = "";
        }
    }

    public void SetWineToFalse()
    {
        pv.RPC("RPC_SetWineToFalse", RpcTarget.All);
    }

    [PunRPC]
    void RPC_SetWineToFalse()
    {
        if(pv.IsMine)
        {
            WineStatus(false);
        }

    }

    public void WineStatus(bool _wine)
    {
        wine = _wine;
        WineObject.SetActive(_wine);
        if (pv.IsMine)
        {
            SetWineBool();
        }
    }

    public void SetWineBool()
    {
        bool HaveWineBool = wine;
        Hashtable hash = new Hashtable();
        hash.Add("HaveWineBool", HaveWineBool);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps["HaveWineBool"] != null)
        {
            if (!pv.IsMine && targetPlayer == pv.Owner)
            {
                wine = (bool)changedProps["HaveWineBool"];
            }
        }
    }




    private void OnTriggerStay(Collider other)
    {
        var paparazzi = other.gameObject.GetComponentInParent<PaparazziPlayerController>();
        if (paparazzi != null)
        {

            if(wine == true)
            {
                Hint.SetActive(true);
            }
            else
            {
                Hint.SetActive(false);
            }
        }
        else
        {
            Hint.SetActive(false);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(WineCoolDownTimer);
        }
        else if(stream.IsReading)
        {
            WineCoolDownTimer = (float)stream.ReceiveNext();
        }
    }
}
