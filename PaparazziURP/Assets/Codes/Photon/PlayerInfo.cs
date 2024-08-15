using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEditor;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun.UtilityScripts;

public class PlayerInfo : MonoBehaviourPunCallbacks
{
    public RoomManager2 room;
    public Image playerImage;
    public GameObject Ready;
    public GameObject ClickHint;
    public Sprite MasterClientSprite;
    public Sprite Paparazzi1Sprite;
    public Sprite Paparazzi2Sprite;
    public Sprite Paparazzi3Sprite;
    public TMP_Text playerName;
    public Player _Player;
    public bool ready;
    public AudioClip click;

    private ExitGames.Client.Photon.Hashtable _teamProperties = new ExitGames.Client.Photon.Hashtable();

    public void Awake()
    {
        room = FindObjectOfType<RoomManager2>();
    }

    public void SetPlayerInfo(Player player)
    {
        _Player = player;
        playerName.text = player.NickName;
        if (player.IsMasterClient)
        {
            gameObject.GetComponent<Button>().interactable = false;
            int agentTeam = 1;
            _teamProperties["Team"] = agentTeam;
            _Player.CustomProperties = _teamProperties;
            Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["Team"]);
            ready = true;
            Ready.SetActive(ready);
            ClickHint.SetActive(!ready);
            SetReadyBool();

        }
        else
        {
            if (_Player != PhotonNetwork.LocalPlayer)
            {
                ClickHint.SetActive(false);
                gameObject.GetComponent<Button>().interactable = false;
                foreach (var kvp in PhotonNetwork.CurrentRoom.Players)
                {
                    if(_Player == kvp.Value)
                    {
                        if (kvp.Value.CustomProperties.ContainsKey("readyBool"))
                        {
                            ready = kvp.Value.CustomProperties.ContainsKey("readyBool");
                            Debug.Log(kvp.Value.CustomProperties.ContainsKey("readyBool"));
                        }
                    }
                }
            }
            else
            {
                gameObject.GetComponent<Button>().interactable = true;
                ClickHint.SetActive(true);
            }
            int paparazziTeam = 0;
            _teamProperties["Team"] = paparazziTeam;
            _Player.CustomProperties = _teamProperties;
            Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["Team"]);
            playerImage.sprite = Paparazzi1Sprite;
            //ready = false;
            Ready.SetActive(ready);
            SetReadyBool();
        }
    }
    public override void OnLeftRoom()
    {
        _Player.CustomProperties.Remove(_teamProperties);
        Destroy(this.gameObject);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(_Player == otherPlayer)
        {
            Destroy(this.gameObject);
        }
    }
    public void OnClick_Ready()
    {
        if(!ready)
        {
            ready = true;
            Ready.SetActive(ready);
            ClickHint.SetActive(!ready);
        }
        else
        {
            ready = false;
            Ready.SetActive(ready);
            ClickHint.SetActive(!ready);
        }
        LobbyManager2.instance.GetComponent<AudioSource>().PlayOneShot(click);
        SetReadyBool();
    }

    public void OnGUI()
    {
        if (GUI.Button(new Rect(10, 70, 50, 30), "Click"))
        {
            int paparazziTeam = 0;
            _teamProperties["Team"] = paparazziTeam;
            _Player.CustomProperties = _teamProperties;
        }
    }
    public void SetReadyBool()
    {
        bool _ready = ready;
        Hashtable hash = new Hashtable();
        hash.Add("readyBool", _ready);
        _Player.SetCustomProperties(hash);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps["readyBool"] != null)
        {
            if (targetPlayer != PhotonNetwork.LocalPlayer && targetPlayer == _Player)
            {
                ready = (bool)changedProps["readyBool"];
                Ready.SetActive(ready);
                if(PhotonNetwork.IsMasterClient)
                {
                    RoomManager2.instance.CheckReadyCount();
                }
            }
        }
    }
}
