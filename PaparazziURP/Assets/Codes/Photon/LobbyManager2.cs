using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager2 : MonoBehaviourPunCallbacks
{
    public static LobbyManager2 instance;
    public GameObject Room;
    public GameObject LobbyMenu;
    public Button StartGameButton;
    public GameObject NormalMatch;
    public GameObject roomListPrefab;
    public Transform roomListContent;
    public Text RoomNameText;
    public GameObject developer;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if(!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }
    }
    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
    }
    public override void OnJoinedRoom()
    {
        LobbyMenu.SetActive(false);
        Room.SetActive(true);
        RoomNameText.text = PhotonNetwork.CurrentRoom.Name;
        StartGameButton.interactable = PhotonNetwork.IsMasterClient;
    }
    public void OnClick_CreateRoom()
    {
        string RoomName = PhotonNetwork.NickName + "ªº©Ð¶¡";
        RoomOptions Room = new RoomOptions();
        Room.IsOpen = true;
        Room.IsVisible = true;
        Room.MaxPlayers = 4;
        Room.BroadcastPropsChangeToAll = true;
        PhotonNetwork.CreateRoom(RoomName, Room, TypedLobby.Default);
        Debug.Log(RoomName);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }
    public void OnClick_NormalMatch()
    {
        NormalMatch.SetActive(true);
    }
    public void OnClick_CloseNormalMatch()
    {
        NormalMatch.SetActive(false);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform rooms in roomListContent)
        {
            Destroy(rooms.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }
    public override void OnJoinedLobby()
    {
        LobbyMenu.SetActive(true);
        Room.SetActive(false);
        Debug.Log("JoinedLobby");
    }
    public override void OnConnectedToMaster()
    {
        if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }
    public void OnClick_TutorialMode()
    {
        SceneManager.LoadScene(3);
    }
    public void ToggleDeveloper(bool toggle)
    {
        if(toggle)
        {
            developer.SetActive(true);
        }
        else
        {
            developer.SetActive(false);
        }
    }

}
