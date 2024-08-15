using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    public Text RoomName;
    public RoomInfo info;
    public void SetUp(RoomInfo _info)
    {
        info = _info;
        RoomName.text = _info.Name;
    }
    public void OnClick()
    {
        LobbyManager2.instance.JoinRoom(info);
    }
}
