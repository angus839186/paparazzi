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
            Tutorialtext.text = "<sprite index=0> <sprite index=1> <sprite index=2> <sprite index=3> ����/�ƹ��즲���ʵ���";
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
            Tutorialtext.text = "<sprite index=8> �}�Ҭ۾�/<sprite index=7> ����P���";
            return;
        }
        else
        {
            if (!alreadyUpload)
            {
                Tutorialtext.text = "<sprite index=9> ������b�a�ϦU�a�W�ǷӤ��o��";
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
            Tutorialtext.text = "�ڭ̨ӬݬݹC�����l�v�L�{�a";
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
            Tutorialtext.text = "<sprite index=5> �i�H�״_���ͪ��۾�";
            return;
        }
        if (!alreadyFixPaparazziPlayer)
        {
            agent.GetComponentInParent<agentBot>().target = paparazzi.transform;
            agent.GetComponentInParent<agentBot>().chasing = true;
            Tutorialtext.text = "���M��V�i�H�״_�ۤv���۾��A���O�����ͭ׷|�󦳮Ĳv";
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
            Tutorialtext.text = "<sprite index=4> �������s�~/ <sprite index=7> �i�H��V�g���H";
        }
        else
        {
            Tutorialtext.text = "�I��������g���H�|�u�ȶi�J�w�t���A�A�ڭ̨�����ݬ�";
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
            Tutorialtext.text = "���U�ӬO�g���H���о�";
            Invoke("NextAgentStep", 5f);
        }
        if(agentTutorialStep == 1)
        {
            Tutorialtext.text = "�����g���H�A��A�ϥΧ����M���l�ɳ��|���ӤU�誺��q���A�����I�i�H�ɥR��q";
            Invoke("NextAgentStep", 5f);
        }
        if(agentTutorialStep == 2)
        {
            Tutorialtext.text = "�յۥ� <sprite index=9> ��X���l����G��";
        }
        if(agentTutorialStep == 3)
        {
            Tutorialtext.text = "�b�C�����A�A�����b�ɶ����O�@���P�A�O�����J����";
            Invoke("NextAgentStep", 5f);
        }
        if(agentTutorialStep == 4)
        {
            Tutorialtext.text = "����䪺��l������ <sprite index=10>";
            if (takeTrap == true)
            {
                Tutorialtext.text = "��Q�i�H���X�����A�I������i�H�N������m�b�a�W";
                if(placeTrap == true)
                {
                    Tutorialtext.text = "���J�p�G��쳴���N�|���S��m�M�u�ȩw��";
                    Invoke("NextAgentStep", 5f);
                }
            }
        }
        if(agentTutorialStep == 5)
        {
            Tutorialtext.text = "<sprite index=6>  �̫�A�A�i�H���}�p�a�Ϩӱo�����P�ثe����m";
            Invoke("NextAgentStep", 10f);
        }
        if(agentTutorialStep ==6)
        {
            Tutorialtext.text = "<sprite index=11>  �Y�i�^��C���j�U";
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
