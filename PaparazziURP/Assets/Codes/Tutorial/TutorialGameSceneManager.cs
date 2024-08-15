using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class TutorialGameSceneManager : MonoBehaviour
{
    public TMP_Text Tutorialtext;
    public static TutorialGameSceneManager Instance;

    public Camera cam;
    public GameObject agentCameraTransform;
    public GameObject paparazziCameraTransform;
    public GameObject paparazziUI;
    public GameObject agentUI;
    public GameObject agentHand;
    public GameObject paparazziHand;

    public GameObject _paparazziBot;
    public GameObject paparazzi;
    public GameObject agent;
    public float lerpSpeed;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        MoveLevelCheck();
        ChangePlayer(paparazziCameraTransform);
    }
    
    

    public void ChangePlayer(GameObject player)
    {
        StartCoroutine(changePlayerView(player));
    }
    IEnumerator changePlayerView(GameObject player)
    {
        cam.transform.SetParent(null);
        while(Vector3.Distance(cam.transform.position, player.transform.position) >= 1f)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, player.transform.position, 
                lerpSpeed * Time.deltaTime);
            yield return null;
        }

        cam.transform.SetParent(player.transform);
        cam.transform.position = player.transform.position;


        yield break;
    }
    #region paparazzi
    [Header("Move Level")]
    public float currentMoveNumber;
    public bool MoveLevelFinished;

    [Header("UploadPhotoLevel")]
    public bool alreadyTakePhoto;
    public bool alreadyUpload;
    public int currentUploadPhoto;
    public int maxUploadPhoto;
    public GameObject UploadPhotoLevelCheckPoint;

    [Header("ChasingLevel")]
    public bool GetInChasingLevel;
    public Transform watchingChasingTransform;
    public Transform agentStandTransform;

    [Header("FixingLevel")]
    public bool alreadyFixPaparazziBot;
    public bool alreadyFixPaparazziPlayer;

    [Header("ItemLevel")]
    public bool wineHitAgent;

    [Header("cameraAmount")]
    public int CameraAmount;
    public int maxCameraAmount;
    public void MoveLevelCheck()
    {
        if (!MoveLevelFinished)
        {
            Tutorialtext.text = "<sprite index=0> <sprite index=1> <sprite index=2> <sprite index=3> 移動/滑鼠拖曳移動視角";
            if (currentMoveNumber >= 4)
            {
                MoveLevelFinished = true;
                UploadPhotoLevelCheck();
            }
            return;
        }
    }

    public void UploadPhotoLevelCheck()
    {
        if (!alreadyTakePhoto)
        {
            Tutorialtext.text = "<sprite index=8> 開啟相機/<sprite index=7> 對明星拍照";
            return;
        }
        else
        {
            if (!alreadyUpload)
            {
                Tutorialtext.text = "<sprite index=9> 長按能在地圖各地上傳照片得分";
                return;
            }
            else
            {
                chasingLevelCheck();
            }
        }
    }
    public void chasingLevelCheck()
    {
        if (!GetInChasingLevel)
        {
            Tutorialtext.text = "我們來看看遊戲的追逐過程吧";
            UploadPhotoLevelCheckPoint.SetActive(true);
        }
        else
        {
            Invoke("StartChasingPaparazziBot", 2f);
        }
    }
    public void FixLevelCheck()
    {
        if (!alreadyFixPaparazziBot)
        {
            paparazzi.GetComponent<PaparazziPlayerController>().unFreeze();
            Tutorialtext.text = "<sprite index=5> 可以修復隊友的相機";
            return;
        }
        if (!alreadyFixPaparazziPlayer)
        {
            agent.GetComponentInParent<agentBot>().target = paparazzi.transform;
            agent.GetComponentInParent<agentBot>().chasing = true;
            Tutorialtext.text = "雖然按V可以修復自己的相機，但是給隊友修會更有效率";
            return;
        }
        else
        {
            StartCoroutine(facingAgent());
            ItemLevelCheck();
        }
    }

    public void ItemLevelCheck()
    {
        if (!wineHitAgent)
        {
            Tutorialtext.text = "<sprite index=4> 切換成酒瓶/ <sprite index=7> 可以丟向經紀人";
        }
        else
        {
            Tutorialtext.text = "碰到煙霧的經紀人會短暫進入暈眩狀態，我們來體驗看看";
            Invoke("StartToPlayAgent", 4f);
        }
    }
    IEnumerator facingAgent()
    {
        agent.transform.position = agentStandTransform.position;
        paparazzi.GetComponentInParent<CharacterController>().enabled = false;
        paparazzi.transform.position = watchingChasingTransform.transform.position;
        paparazzi.GetComponentInParent<CharacterController>().enabled = true;
        while (!wineHitAgent)
        {
            paparazzi.GetComponentInParent<PaparazziPlayerController>().canMove = false;
            yield return null;
        }
        paparazzi.GetComponentInParent<PaparazziPlayerController>().canMove = true;
        yield break;

    }

    public void StartChasingPaparazziBot()
    {
        paparazzi.GetComponentInParent<CharacterController>().enabled = false;
        paparazzi.transform.position = watchingChasingTransform.transform.position;
        paparazzi.GetComponentInParent<CharacterController>().enabled = true;
        paparazzi.GetComponent<PaparazziPlayerController>().Freeze();
        _paparazziBot.GetComponentInParent<paparazziBot>().canMove = true;
        agent.GetComponent<agentBot>().chasing = true;
    }

    #endregion

    #region agent
    public int agentTutorialStep;
    public bool takeTrap;
    public bool placeTrap;
    public void StartToPlayAgent()
    {
        paparazziCameraTransform.gameObject.GetComponentInParent<PaparazziPlayerController>().enabled = false;
        paparazziCameraTransform.gameObject.GetComponentInParent<PlayerCamera>().enabled = false;
        paparazziCameraTransform.gameObject.GetComponentInParent<paparazziUI>().enabled = false;
        paparazziCameraTransform.gameObject.GetComponentInParent<TutorialCameraObject>().enabled = false;
        paparazziUI.gameObject.SetActive(false);
        paparazziHand.SetActive(false);
        paparazzi.SetActive(false);
        ChangePlayer(agentCameraTransform);
        agentUI.gameObject.SetActive(true);
        agentHand.SetActive(true);
        agentCameraTransform.gameObject.GetComponentInParent<agentBot>().enabled = false;
        agentCameraTransform.gameObject.GetComponentInParent<NavMeshAgent>().enabled = false;
        agentCameraTransform.gameObject.GetComponentInParent<CharacterController>().enabled = true;
        agentCameraTransform.gameObject.GetComponentInParent<TutorialAgent>().enabled = true;
        agentCameraTransform.gameObject.GetComponentInParent<TutorialTracker>().enabled = true;
        agentCameraTransform.gameObject.GetComponentInParent<TutorialTrap>().enabled = true;
        agentCameraTransform.gameObject.GetComponentInParent<PlayerCamera>().enabled = true;
        agentCameraTransform.gameObject.GetComponentInParent<agentUIsystem>().enabled = true;
        AgentTutorialStepCheck();
    }
    public void AgentTutorialStepCheck()
    {
        if(agentTutorialStep == 0)
        {
            Tutorialtext.text = "接下來是經紀人的教學";
            Invoke("NextAgentStep", 5f);
        }
        if(agentTutorialStep == 1)
        {
            Tutorialtext.text = "身為經紀人，當你使用攻擊和飛槌時都會消耗下方的能量條，重生點可以補充能量";
            Invoke("NextAgentStep", 5f);
        }
        if(agentTutorialStep == 2)
        {
            Tutorialtext.text = "試著用 <sprite index=9> 丟出飛槌飛到二樓";
        }
        if(agentTutorialStep == 3)
        {
            Tutorialtext.text = "在遊戲中，你必須在時間內保護明星，別讓狗仔偷拍";
            Invoke("NextAgentStep", 5f);
        }
        if(agentTutorialStep == 4)
        {
            Tutorialtext.text = "到旁邊的桶子拿陷阱 <sprite index=10>";
            if (takeTrap == true)
            {
                Tutorialtext.text = "按Q可以拿出陷阱，點擊左鍵可以將陷阱放置在地上";
                if(placeTrap == true)
                {
                    Tutorialtext.text = "狗仔如果踩到陷阱將會暴露位置和短暫定身";
                    Invoke("NextAgentStep", 5f);
                }
            }
        }
        if(agentTutorialStep == 5)
        {
            Tutorialtext.text = "<sprite index=6>  最後，你可以打開小地圖來得知明星目前的位置";
            Invoke("NextAgentStep", 10f);
        }
        if(agentTutorialStep ==6)
        {
            Tutorialtext.text = "<sprite index=11>  即可回到遊戲大廳";
        }
        if(agentTutorialStep == 7)
        {

        }
    }
    public void NextAgentStep()
    {
        agentTutorialStep++;
        AgentTutorialStepCheck();
    }
    #endregion

    public void LeaveRoom()
    {
        SceneManager.LoadScene(0);
    }

}
