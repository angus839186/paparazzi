using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public InputField UserID_Input;

    public GameObject StartMenu;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (!PhotonNetwork.IsConnected)
        {
            StartMenu.SetActive(true);
            return;
        }
        else
        {
            PhotonNetwork.Disconnect();
        }
    }
    public void OnClick_JoinServer()
    {
        if (string.IsNullOrEmpty(UserID_Input.text))
        {
            Debug.Log("Name should not empty");
            return;
        }
        else
        {
            if(PhotonNetwork.IsConnected)
            {
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.IsMessageQueueRunning = true;
                PhotonNetwork.LocalPlayer.NickName = GetPlayerName();
                PhotonNetwork.AuthValues = new AuthenticationValues(GetPlayerName());
                Debug.Log("reconnect");
                SceneManager.LoadScene(1);
            }
            else
            {
                PhotonNetwork.GameVersion = "0.0.1";
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.IsMessageQueueRunning = true;
                PhotonNetwork.LocalPlayer.NickName = GetPlayerName();
                PhotonNetwork.AuthValues = new AuthenticationValues(GetPlayerName());
                Debug.Log("connect");
                PhotonNetwork.ConnectUsingSettings();
            }
        }
    }
    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene(1);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(cause);
    }
    public string GetPlayerName()
    {
        string playerName = UserID_Input.text;
        return playerName;
    }

}
