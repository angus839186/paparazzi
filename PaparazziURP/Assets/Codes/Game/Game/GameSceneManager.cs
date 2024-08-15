using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using Photon.Realtime;

public class GameSceneManager : MonoBehaviourPunCallbacks
{
    public static GameSceneManager instance;
    public UploadedPhotoUI uploadedPhoto;
    public SNGcar sngcar;

    public TimeManager timeManager;

    public PhotonView pv;

    public bool GameEnd;

    public GameObject GameWinBackground;
    public GameObject PaparazziWinUI;
    public GameObject AgentWinUI;


    public GameObject[] signalTransform;
    public int opensignal;
    public GameObject sceneUI;
    private void Awake()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            pv = GetComponent<PhotonView>();
        }
        instance = this;
    }
    public void Start()
    {
        OpenSignal();
    }
    void OpenSignal()
    {
        for (int i = 0; i < signalTransform.Length; i++)
        {
            if(opensignal <3)
            {
                int _random = Random.Range(0, 8);
                if (signalTransform[_random].activeSelf == false)
                {
                    if (pv.IsMine)
                    {
                        signalTransform[_random].SetActive(true);
                        pv.RPC("SyncOpenSignal", RpcTarget.OthersBuffered, _random);
                    }
                    opensignal++;
                }
            }
        }

    }

    [PunRPC]
    void SyncOpenSignal(int number)
    {
        if (signalTransform[number].activeSelf == false)
        {
            signalTransform[number].SetActive(true);
        }
    }



    public override void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (PhotonNetwork.CurrentRoom == null) return;
        if(scene.buildIndex == 2)
        {
            PhotonNetwork.Instantiate("PlayerManager", Vector3.zero, Quaternion.identity);
        }
    }

    public void GameStatusCheck()
    {
        pv.RPC("RPC_GameStatusCheck", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void RPC_GameStatusCheck()
    {
            if (sngcar.CameraAmount <= 0 || timeManager.gameTime <= 0)
            {
                StartCoroutine(GameWinUIFadeIn(1f));
                AgentWinUI.SetActive(true);
            }
            if (uploadedPhoto.currentPhoto >= uploadedPhoto.MaxUploadedPhoto)
            {
                StartCoroutine(GameWinUIFadeIn(1f));
                PaparazziWinUI.SetActive(true);
            }
            GameEnd = true;
    }

    IEnumerator GameWinUIFadeIn(float duration)
    {
        GameWinBackground.SetActive(true);
        float counter = 0;
        Color alphaColor = GameWinBackground.GetComponent<Image>().color;

        float minAlpha = 0.0f;
        float maxAlpha = 0.75f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            alphaColor.a = Mathf.Lerp(minAlpha,maxAlpha, counter / duration);
            GameWinBackground.GetComponent<Image>().color = alphaColor;

            yield return null;
        }
        yield return new WaitForSecondsRealtime(2f);
        PhotonNetwork.LeaveRoom();

        PhotonNetwork.AutomaticallySyncScene = false;
        Debug.Log("leaveRoom");
        SceneManager.LoadScene(0);
    }


    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
        PhotonNetwork.LeaveRoom();
        Debug.Log("back");
        SceneManager.LoadScene(0);
    }
    public void GameStart()
    {
        FindObjectOfType<AudioManager>().Play("BackgroundMusic");
    }

}
