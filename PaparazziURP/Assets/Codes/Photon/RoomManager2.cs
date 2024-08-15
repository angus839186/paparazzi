using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Security.Cryptography;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine.SceneManagement;

public class RoomManager2 : MonoBehaviourPunCallbacks
{
    public Transform playerListTransform;
    public GameObject playerListPrefab;
    public GameObject StartGameButton;
    public static RoomManager2 instance;

    public void Awake()
    {
        instance = this;
    }
    public void CheckReadyCount()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int readyCount = 0;
            foreach (Transform players in playerListTransform)
            {
                if (players.GetComponent<PlayerInfo>().ready == true)
                {
                    if (readyCount < PhotonNetwork.CurrentRoom.PlayerCount)
                    {
                        readyCount++;
                    }
                }
            }
            if (readyCount == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                StartGameButton.SetActive(true);
            }
            else
            {
                StartGameButton.SetActive(false);
                Debug.Log("非称计ぃì");
            }
            Debug.Log(readyCount);
        }
    }
    public void OnClick_LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnJoinedRoom()
    {
        Player[] players = PhotonNetwork.PlayerList;
        foreach (Transform child in playerListTransform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < players.Count(); i++)
        {
            GameObject playerinfo = Instantiate(playerListPrefab, playerListTransform);
            playerinfo.GetComponent<PlayerInfo>().SetPlayerInfo(players[i]);
            CheckReadyCount();
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject playerinfo = Instantiate(playerListPrefab, playerListTransform);
        playerinfo.GetComponent<PlayerInfo>().SetPlayerInfo(newPlayer);
        Debug.Log(newPlayer.CustomProperties["Team"]);
        CheckReadyCount();
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        Debug.Log("Leave From Room");
        LobbyManager2.instance.LobbyMenu.SetActive(true);
        LobbyManager2.instance.Room.SetActive(false);
        PhotonNetwork.JoinLobby();
    }
    public void OnClick_StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int readyCount = 0;
            foreach (Transform players in playerListTransform)
            {
                if (players.GetComponent<PlayerInfo>().ready == true)
                {
                    if(readyCount <= PhotonNetwork.CurrentRoom.PlayerCount)
                    {
                        readyCount++;
                    }
                }
            }
            if (readyCount == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                PhotonNetwork.LoadLevel(2);
                Debug.Log("GO");
            }
            else
            {
                Debug.Log("非称计ぃì");
            }
        }
    }

}
