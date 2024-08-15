using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class AgentTrap : MonoBehaviourPunCallbacks
{
    public PhotonView pv;

    [Header("®³³´¨À")]
    public Text trapAmountText;
    public int trapAmount;

    [Header("©ñ³´¨À")]
    public Transform TrapPlacement;
    public GameObject TrapBluePrint;
    public bool TrapisGrounded;
    public GameObject TrapPrefab;
    public LayerMask ground;
    public LayerMask Loot;


    [Header("¨ä¥L")]
    public agentPlayerController agent;
    public PlayerCamera _cam;
    public Camera viewCamera;
    public GameObject takeTrapHint;

    public bool usingTrap;

    public List<GameObject> _trapOnScene = new List<GameObject>();
    void Update()
    {
        if (pv.IsMine)
        {
            var ray = new Ray(viewCamera.transform.position, viewCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 5f, Loot))
            {
                if (raycastHit.collider.gameObject.TryGetComponent(out SelectTrap trapLoot))
                {
                    if (trapLoot != null)
                    {
                        takeTrapHint.SetActive(true);
                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            AddTrap();
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                takeTrapHint.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (usingTrap)
                {
                    ToggleUsingTrap(false);
                }
                else
                {
                    ToggleUsingTrap(true);
                }
            }
            if (usingTrap)
            {
                TrapisGrounded = Physics.CheckSphere(TrapPlacement.position, 0.5f, ground);
                if (Input.GetMouseButtonDown(0) && TrapisGrounded)
                {
                    PlacingTrap();
                }
                if (Input.GetMouseButtonDown(1))
                {
                    ToggleUsingTrap(false);
                }
            }
        }
    }

    public void ToggleUsingTrap(bool usingTrapBool)
    {
        usingTrap = usingTrapBool;
        if(usingTrap)
        {
            TrapBluePrint.SetActive(true);
            agent.state = agentPlayerController.State.usingTrap;
        }
        else
        {
            TrapBluePrint.SetActive(false);
            agent.state = agentPlayerController.State.Normal;
        }
    }

    void PlacingTrap()
    {
        if (trapAmount != 0)
        {
            trapAmount--;
            trapAmountText.text = trapAmount.ToString();
            pv.RPC("RPC_placingTrap", RpcTarget.All);
        }
        else
        {
            TrapBluePrint.SetActive(false);
            return;
        }
    }
    [PunRPC]
    void RPC_placingTrap()
    {
        GameObject trap = Instantiate(TrapPrefab, TrapPlacement.position, TrapPlacement.rotation);
    }

    public void RemoveTrapOnScene(GameObject trap)
    {
        _trapOnScene.Remove(trap);
    }


    public void AddTrap()
    {
        if (trapAmount < 5)
        {
            trapAmount++;
            trapAmountText.text = trapAmount.ToString();
        }
    }
}
