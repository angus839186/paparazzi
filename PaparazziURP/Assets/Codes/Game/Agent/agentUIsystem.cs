using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class agentUIsystem : MonoBehaviourPunCallbacks
{
    public PhotonView pv;

    public GameObject UI;
    public GameObject miniMapUI;
    public GameObject LeaveRoomButton;
    public Text minuteText;
    public Text secondText;

    public agentPlayerController agent;

    public Animator anime;

    private void Start()
    {
        if (!pv.IsMine)
        {
            UI.SetActive(false);
            Destroy(UI);
        }
    }

    private void Update()
    {
        if(pv.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                LeaveRoom();
            }
            if (Input.GetKey(KeyCode.Tab))
            {
                MiniMapToggle(true);
            }
            else
            {
                MiniMapToggle(false);
            }
            if(Input.GetKeyDown(KeyCode.N))
            {
                ToggleUI();
            }
            if(TimeManager.instance != null)
            {
                if (TimeManager.instance.prepared == false)
                {
                    UpdateTime(TimeManager.instance.prepareTime);
                }
                else
                {
                    UpdateTime(TimeManager.instance.gameTime);
                    if (TimeManager.instance.gameEnd == true)
                    {
                        agent.canMove = false;
                    }
                }
            }
        }
    }
    public void UpdateTime(int timeToDisplay)
    {
        int minutes = timeToDisplay / 60;
        int seconds = timeToDisplay % 60;

        minuteText.text = string.Format("0{00}", minutes);
        secondText.text = string.Format("{00}", seconds);
    }

    public void MiniMapToggle(bool minimaptoggle)
    {
        miniMapUI.SetActive(minimaptoggle);
        anime.SetBool("watchMap", minimaptoggle);
    }
    public void ToggleUI()
    {
        if(UI.activeSelf == false)
        {
            UI.SetActive(true);
            GameSceneManager.instance.sceneUI.SetActive(true);
        }
        else
        {
            UI.SetActive(false);
            GameSceneManager.instance.sceneUI.SetActive(false);
        }
    }
    public void LeaveRoom()
    {
        if (LeaveRoomButton.activeSelf == false)
        {
            LeaveRoomButton.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            LeaveRoomButton.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    public void PlayerLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
    }
}
