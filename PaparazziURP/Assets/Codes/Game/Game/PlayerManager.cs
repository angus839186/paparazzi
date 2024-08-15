using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
    PhotonView pv;

    public GameObject controller;

    public int paparazziTeam = 0;
    public int agentTeam = 1;

    public Transform spawnPoint;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();

    }
    private void Start()
    {
        if(pv.IsMine)
        {
            CreatePlayer();
        }
    }
    void CreatePlayer()
    {
        int team = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];

        if (team == paparazziTeam)
        {
            Transform spawn = SpawnManager.instance.GetTeamSpawn(0);
            controller = PhotonNetwork.Instantiate("paparazziController",
                spawn.position, spawn.rotation, 0, new object[] { pv.ViewID });
            spawnPoint = spawn;

        }
        else
        {
            Transform spawn = SpawnManager.instance.GetTeamSpawn(1);
            controller = PhotonNetwork.Instantiate("agentController",
                spawn.position, spawn.rotation, 0, new object[] { pv.ViewID });
            spawnPoint = spawn;
        }
    }
}
