using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class paparazziUI : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    public GameObject UI;
    public GameObject LeaveRoomButton;

    public PaparazziPlayerController paparazzi;

    public Text minuteText;
    public Text secondText;
    public bool openMenu;
    public bool openMap;

    private void Update()
    {
        if (pv.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!openMenu)
                {
                    openMenu = true;
                    LeaveRoom(openMenu);
                }
                else
                {
                    openMenu = false;
                    LeaveRoom(openMenu);
                }
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
                        paparazzi.canMove = false;
                    }
                }
            }
        }
    }
    public void UpdateTime(int timeToDisplay)
    {
        int minutes = timeToDisplay / 60;
        int seconds = timeToDisplay % 60;

        minuteText.text = string.Format("0{00}",minutes);
        secondText.text = string.Format("{00}",seconds);
    }
    public void ToggleUI()
    {
        if (UI.activeSelf == false)
        {
            UI.SetActive(true);
        }
        else
        {
            UI.SetActive(false);
        }
    }
    public void LeaveRoom(bool _active)
    {
        LeaveRoomButton.SetActive(_active);
        if(_active)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    public void PlayerLeaveRoom()
    {
        //PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
}
