using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.XR;
using Unity.VisualScripting;

public class PaparazziPlayerController : MonoBehaviourPunCallbacks
{

    public PhotonView pv;
    public SpriteRenderer minimapIcon;
    public Sprite selfIcon;
    public Sprite otherIcon;

    [Header("移動")]
    public float speed;
    public bool canMove;
    public CharacterController playerCC;
    public Vector3 moveVelocity;
    float moveVelocityY;
    public int jumpCount;
    public AudioSource jumpAudio;
    public AudioClip jumpClip;

    public Animator Anime;
    public Animator HandAnime;

    public AudioListener audioListener;

    public GameObject body;
    public GameObject hand;

    [Header("UI")]
    public GameObject PaparazziUI;
    public GameObject miniMapCamera;

    public bool Tracking;


    [Header("特效")]

    public GameObject stuckEffect;
    public GameObject flyingBee;
    public PlayerCamera _cam;

    [Header("音效")]
    public AudioSource FootStep_Sound;
    public AudioClip[] footStepSounds;
    public int CurrentFootStepNum;
    public bool playingTutorial;


    public State paparazzistate;

    public enum State
    {
        preparing,
        normal,
    }

    // Start is called before the first frame update
    void Start()
    {

        if (!pv.IsMine)
        {
            miniMapCamera.GetComponent<Camera>().enabled = false;
            Destroy(PaparazziUI);
            audioListener.enabled = false;
            SkinnedMeshRenderer[] handSkin = hand.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < handSkin.Length; i++)
            {
                handSkin[i].enabled = false;
            }
            minimapIcon.sprite = otherIcon;
        }
        else
        {
            miniMapCamera.GetComponent<Camera>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            canMove = true;
            SkinnedMeshRenderer[] skin = body.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < skin.Length; i++)
            {
                skin[i].enabled = false;
            }
            if(playingTutorial)
            {
                GameStart();
            }
            else
            {
                Invoke(nameof(GameStart), TimeManager.instance.prepareTime);
            }
            minimapIcon.sprite = selfIcon;
        }
    }
    private void Update()
    {
        if (pv.IsMine)
        {
            switch (paparazzistate)
            {
                case State.normal:
                    Move();
                    break;
            }
            if (transform.position.y <= -90)
            {
                RespawnPlayer();
            }
        }
    }
    public void GameStart()
    {
        ChangeState(State.normal);
        _cam.stopCamera = false;
    }
    private void Move()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        if(!canMove)
        {
            moveVelocity = Vector3.zero;
            Anime.SetBool("Move", false);
            HandAnime.SetBool("Move", false);
        }
        else
        {
            moveVelocity = transform.right * moveX * speed + transform.forward * moveZ * speed;
            if (moveVelocity.x != 0 || moveVelocity.z != 0)
            {
                Anime.SetBool("Move", true);
                HandAnime.SetBool("Move", true);
                if(CurrentFootStepNum == 0)
                {
                    if(!FootStep_Sound.isPlaying)
                    {
                        FootStep_Sound.PlayOneShot(footStepSounds[0]);
                        CurrentFootStepNum = 1;
                    }
                }
                else
                {
                    if(!FootStep_Sound.isPlaying)
                    {
                        FootStep_Sound.PlayOneShot(footStepSounds[1]);
                        CurrentFootStepNum = 0;
                    }
                }
            }
            else
            {
                Anime.SetBool("Move", false);
                HandAnime.SetBool("Move", false);
                FootStep_Sound.Stop();
                CurrentFootStepNum = 0;
            }
        }

        if (playerCC.isGrounded)
        {
            if (jumpCount != 1)
            {
                jumpCount = 1;
            }
            moveVelocityY = 0f;
        }
        else
        {
            float gravityDownForce = -60f;
            moveVelocityY += gravityDownForce * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (jumpCount == 1)
            {
                jumpCount = 0;
                jumpAudio.PlayOneShot(jumpClip);
                float jumpSpeed = 20f;
                moveVelocityY = jumpSpeed;
                Debug.Log("jump");
            }
        }

        moveVelocity.y = moveVelocityY;

        playerCC.Move(moveVelocity * Time.deltaTime);
    }

    public void ChangeState(State _state)
    {
        paparazzistate = _state;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponentInParent<StuckTrap>())
        {
            StuckByTrap();
            SetTrack(true);
        }
    }
    public void StuckByTrap()
    {
       pv.RPC("RPC_Stuck", RpcTarget.All);
    }

    [PunRPC]
    IEnumerator RPC_Stuck()
    {
        stuckEffect.GetComponent<ParticleSystem>().Play();
        if (pv.IsMine)
        {
            canMove = false;
            SetMoveBool();
            yield return new WaitForSeconds(4f);
            canMove = true;
            SetMoveBool();
        }
        yield break;
    }

    public void SetTrack(bool track)
    {
        pv.RPC("RPC_SetTrack", RpcTarget.All, track);
    }

    [PunRPC]
    public void RPC_SetTrack(bool tracking)
    {
        Tracking = tracking;
        Anime.SetTrigger("Bug");
        if (Tracking == true)
        {
            agentTracker _trap = FindObjectOfType<agentTracker>();
            _trap.AddTarget(this);
            _trap.CreateTracker();
        }
        if (Tracking == false)
        {
            agentTracker _trap = FindObjectOfType<agentTracker>();
            _trap.RemoveTarget();
        }
        if (pv.IsMine)
        {
            if (Tracking == true)
            {
                flyingBee.SetActive(true);
            }
            if (Tracking == false)
            {
                flyingBee.SetActive(false);
            }
            SetTrackBool();
        }
    }

    public void SetMoveBool()
    {
        bool canMoveBool = canMove;
        Hashtable hash = new Hashtable();
        hash.Add("canMoveBool", canMoveBool);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void SetTrackBool()
    {
        bool trackBool = Tracking;
        Hashtable hash = new Hashtable();
        hash.Add("TrackBool", trackBool);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps["canMoveBool"] != null)
        {
            if (!pv.IsMine && targetPlayer == pv.Owner)
            {
                canMove = (bool)changedProps["canMoveBool"];
            }
        }
        if (changedProps["TrackBool"] != null)
        {
            if (!pv.IsMine && targetPlayer == pv.Owner)
            {
                Tracking = (bool)changedProps["TrackBool"];
            }
        }
    }
    private void RespawnPlayer()
    {
        if (pv.IsMine)
        {
            int team = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];

            if (team == 0)
            {
                Transform spawn = SpawnManager.instance.GetTeamSpawn(0);
                transform.position = spawn.position;

            }

            if (team == 1)
            {
                Transform spawn = SpawnManager.instance.GetTeamSpawn(1);
                transform.position = spawn.position;
            }
        }
    }

    public void Freeze()
    {
        canMove = false;
    }

    public void unFreeze()
    {
        canMove = true;
    }

    public void StopToTakePhoto()
    {
        canMove = false;
        Invoke("ResetMove", 0.5f);
    }
    public void ResetMove()
    {
        if(pv.IsMine)
        {
            canMove = true;
        }
    }

}
