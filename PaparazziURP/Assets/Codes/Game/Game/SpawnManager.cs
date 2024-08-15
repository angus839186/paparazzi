using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;
    public GameObject[] agentTeamSpawns;
    public GameObject[] paparazziTeamSpawns;


    private void Awake()
    {
        instance = this;

    }
    public Transform GetRandomAgentSpawn()
    {
        return agentTeamSpawns[Random.Range(0, agentTeamSpawns.Length)].transform;
    }
    public Transform GetRandomPaparazziSpawn()
    {
        return paparazziTeamSpawns[Random.Range(0, paparazziTeamSpawns.Length)].transform;
    }

    public Transform GetTeamSpawn(int Team)
    {

        return Team ==  0? GetRandomPaparazziSpawn() : GetRandomAgentSpawn();
    }

}
