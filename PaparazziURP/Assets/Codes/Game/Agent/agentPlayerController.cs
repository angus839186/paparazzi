using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using UnityEngine.UI;

public class agentPlayerController : MonoBehaviourPunCallbacks
{

    public PhotonView pv;
    public SpriteRenderer minimapSprite;
    public Sprite selfSprite;

    [Header("移動")]
    public bool canMove;
    public CharacterController playerCC;
    public float speed;
    [SerializeField]
    private Vector3 moveVelocity;
    private float moveVelocityY;
    private Vector3 characterVelocityMomentum;
    public int jumpCount;
    public AudioSource jumpAudio;
    public AudioClip jumpClip;

    [Header("攝影機")]
    public Camera miniMapCamera;
    public LayerMask SecondFloorMask;
    public LayerMask OneFloorMask;
    public Camera viewCamera;
    private CameraFOV cameraFOV;
    private const float NORMAL_FOV = 60f;
    private const float HOOKSHOT_FOV = 100f;

    [Header("動畫")]
    public GameObject body;
    public GameObject hand;
    public Animator Anime;
    public Animator HandAnime;

    [Header("狀態")]
    public bool drowsy;
    public GameObject drowsyUI;
    public State state;


    [Header("攻擊")]
    public bool HammerAttack;
    public float HammerAttackCD;
    public float HammerAttackCDtimer;
    public Image HammerAttackImage;
    public AudioSource weaponAudio;
    public AudioClip weaponAttackAudio;

    [Header("丟槌子")]
    public GameObject BodyHammer;
    public GameObject HandHammer;
    public GameObject Hammer;
    public Transform HammerShotTransform;
    public GameObject SpeedUpEffect;
    public Vector3 HammerShotPosition;
    public float HammerShotDistance;
    public float HammerShotCD;
    public float HammerShotCDtimer;
    public bool HammerShotSkill;
    public bool speedUp;
    public float maxHammerDistance;
    public LayerMask WorldMask;
    public Image HammerShotImage;
    public GameObject flyingUI;

    [Header("能量")]
    public Image EnergyImage;
    public int energyCost;
    public int maxEnergy;
    public GameObject energyWarning;
    public AudioSource energyAudio;
    public AudioClip refillEnergyClip;
    public AudioClip outofEnergyClip;

    [Header("遊戲分數")]
    public int brokenCameraAmount;
    public GameSceneManager GM;

    [Header("其他")]
    public AudioListener audioListener;
    public Hinter _hint;
    public enum State
    {
        Normal,
        HammerShotFlyingPlayer,
        HammerShotThrown,
        usingTrap,
        usingMap

    }

    void Start()
    {
        GM = FindObjectOfType<GameSceneManager>();
        cameraFOV = viewCamera.GetComponent<CameraFOV>();
        if (!pv.IsMine)
        {
            miniMapCamera.GetComponent<Camera>().enabled = false;
            audioListener.enabled = false;
            SkinnedMeshRenderer[] handSkin = hand.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < handSkin.Length; i++)
            {

                handSkin[i].enabled = false;
            }
            minimapSprite.sprite = null;

        }
        else
        {
            miniMapCamera.GetComponent<Camera>().enabled = true;
            SkinnedMeshRenderer[] bodyskin = body.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < bodyskin.Length; i++)
            {

                bodyskin[i].enabled = false;
            }
            minimapSprite.sprite = selfSprite;
            BodyHammer.GetComponent<MeshRenderer>().enabled = false;
            EnergyImage.fillAmount = energyCost / maxEnergy;
        }
    }
    private void Update()
    {
        if(pv.IsMine)
        {
            switch(state)
            {
                case State.Normal:
                    Move();
                    if (HammerShotSkill == false)
                    {
                        HammerShotCoolDown();
                    }
                    if (HammerAttack == false)
                    {
                        WeaponAttackCD();
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (energyCost > 0)
                        {
                            TriggerWeaponAttack();
                        }
                        else
                        {
                            _hint.ChangeText("你需要回到重生點補充能量!", true);
                            energyAudio.PlayOneShot(outofEnergyClip);
                            energyWarning.SetActive(true);
                            energyWarning.GetComponent<Animation>().Play();
                            return;
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if(energyCost > 2)
                        {
                            HandleHammerShot();
                        }
                        else
                        {
                            _hint.ChangeText("你需要回到重生點補充能量!", true);
                            energyAudio.PlayOneShot(outofEnergyClip);
                            energyWarning.SetActive(true);
                            energyWarning.GetComponent<Animation>().Play();
                        }
                    }
                    break;
                case State.HammerShotThrown:
                    Move();
                    HandleHammerShotThrown();
                    break;
                case State.HammerShotFlyingPlayer:
                    HandleHookShotMoveMent();
                    break;
                case State.usingTrap:
                    Move();
                    if (HammerShotSkill == false)
                    {
                        HammerShotCoolDown();
                    }
                    if (HammerAttack == false)
                    {
                        WeaponAttackCD();
                    }
                    break;
                case State.usingMap:
                    if (HammerShotSkill == false)
                    {
                        HammerShotCoolDown();
                    }
                    if (HammerAttack == false)
                    {
                        WeaponAttackCD();
                    }
                    break;
            }
            if (transform.position.y <= -90)
            {
                RespawnPlayer();
            }
        }
    }
    public void UpdateEnergyCost()
    {
        if(pv.IsMine)
        {
            float energy = (float)energyCost / (float)maxEnergy;
            EnergyImage.fillAmount = energy;
        }
    }
    private void Move()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        if (!canMove)
            return;
        if (drowsy)
        {
            moveVelocity = transform.forward * moveX * speed + transform.right * moveZ * speed;
        }
        else
        {
            moveVelocity = transform.right * moveX * speed + transform.forward * moveZ * speed;
        }
        if (moveX != 0 || moveZ != 0)
        {
            Anime.SetBool("move", true);
            HandAnime.SetBool("move", true);
        }
        else
        {
            Anime.SetBool("move", false);
            HandAnime.SetBool("move", false);
        }
        if (playerCC.isGrounded)
        {
            if(jumpCount != 1)
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
            if(jumpCount == 1)
            {
                jumpCount = 0;
                jumpAudio.PlayOneShot(jumpClip);
                float jumpSpeed = 20f;
                moveVelocityY = jumpSpeed;
                Debug.Log("jump");
            }
        }

        moveVelocity.y = moveVelocityY;
        moveVelocity += characterVelocityMomentum;

        playerCC.Move(moveVelocity * Time.deltaTime);

        if (characterVelocityMomentum.magnitude >= 0f)
        {
            float momentumDrag = 3f;
            characterVelocityMomentum -= characterVelocityMomentum * momentumDrag * Time.deltaTime;
            if (characterVelocityMomentum.magnitude < .0f)
            {
                characterVelocityMomentum = Vector3.zero;
            }
        }
    }

    public void WeaponAttackCD()
    {
        HammerAttackCDtimer -= Time.deltaTime;
        HammerAttackImage.fillAmount = HammerAttackCDtimer / HammerAttackCD;
        if (HammerAttackCDtimer <= 0)
        {
            HammerAttack = true;
            HammerAttackCDtimer = 0;
            HammerAttackImage.fillAmount = 0;
        }
    }

    public void ChangeState(State _state)
    {
        state = _state;
    }

    private void ResetGravtiyEffect()
    {
        moveVelocity.y = 0f;
    }
    void FloorStateCheck()
    {
        if (transform.position.y >= -10)
        {
            miniMapCamera.cullingMask = SecondFloorMask;
        }
        else
        {
            miniMapCamera.cullingMask = OneFloorMask;
        }
    }


    void TriggerWeaponAttack()
    {
        pv.RPC("RPC_attack", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public IEnumerator RPC_attack()
    {
        if (HammerAttack)
        {
            BodyHammer.GetComponent<BoxCollider>().enabled = true;
            weaponAudio.PlayOneShot(weaponAttackAudio);
            Anime.SetTrigger("attack");
            HandAnime.SetTrigger("attack");
            energyCost--;
            HammerAttackCDtimer = HammerAttackCD;
            UpdateEnergyCost();
            HammerAttack = false;
        }
        yield return new WaitForSeconds(0.9f);
        Anime.ResetTrigger("attack");
        HandAnime.ResetTrigger("attack");
        BodyHammer.GetComponent<BoxCollider>().enabled = false;
        yield break;
    }
    public void AddbrokenCameraAndCheck()
    {
        pv.RPC("RPC_AddbrokenCameraAndCheck", RpcTarget.All);
    }

    [PunRPC]
    void RPC_AddbrokenCameraAndCheck()
    {
        if(pv.IsMine)
        {
            brokenCameraAmount++;
        }
        if (brokenCameraAmount >= 9)
        {
            GM.GameStatusCheck();
        }
        SetbrokenCameraAmount();
    }
    public void SetbrokenCameraAmount()
    {
        int brokenCameraAmountCount = brokenCameraAmount;
        Hashtable hash = new Hashtable();
        hash.Add("brokenCameraAmountCount", brokenCameraAmountCount);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void HammerShotCoolDown()
    {
        HammerShotCDtimer -= Time.deltaTime;
        HammerShotImage.fillAmount = HammerShotCDtimer / HammerShotCD;
        if (HammerShotCDtimer <= 0)
        {
            HammerShotCDtimer = 0;
            HammerShotImage.fillAmount = 0;
            HammerShotSkill = true;
        }
    }


    private void HandleHammerShot()
    {
        if(HammerShotSkill == true)
        {
            if (Physics.Raycast(viewCamera.transform.position, viewCamera.transform.forward, out RaycastHit raycastHit, WorldMask))
            {
                HammerShotPosition = raycastHit.point;
                Anime.SetBool("shot", true);
                HandAnime.SetBool("shot", true);
                HammerShotSkill = false;
                Debug.Log(Vector3.Distance(transform.position, HammerShotPosition));
            }
            else
            {
                return;
            }
        }
    }
    public void ThrowHammer()
    {
        Hammer.SetActive(true);
        HandHammer.SetActive(false);
        ChangeState(State.HammerShotThrown);
        Rigidbody projectileRb = Hammer.GetComponent<Rigidbody>();
        projectileRb.gameObject.transform.SetParent(null);
        Vector3 forceDirection = viewCamera.transform.forward;

        float ThrowSpeed = 50f;

        Vector3 forceToAdd = forceDirection * ThrowSpeed;
        float turn = 200f;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
        projectileRb.AddTorque(Hammer.transform.TransformDirection(Vector3.forward) * turn, ForceMode.Impulse);
    }

    private void HandleHammerShotThrown()
    {
        float HammerDistance = Vector3.Distance(transform.position, Hammer.transform.position);
        float HammerAndShotPoint = Vector3.Distance(Hammer.transform.position, HammerShotPosition);

        Rigidbody projectileRb = Hammer.GetComponent<Rigidbody>();

        if (HammerDistance >= maxHammerDistance)
        {
            Debug.Log("Too far");
            StopHookShot();
        }
        else
        {
            if (HammerAndShotPoint <= 4f)
            {
                projectileRb.isKinematic = true;
                projectileRb.velocity = Vector3.zero;
                projectileRb.angularVelocity = Vector3.zero;
                projectileRb.transform.position = HammerShotPosition + new Vector3(0.5f, 0, 0);
                projectileRb.transform.LookAt(transform.position);
                projectileRb.transform.localEulerAngles = HammerShotTransform.eulerAngles;
                ChangeState(State.HammerShotFlyingPlayer);
                cameraFOV.SetCameraFOV(HOOKSHOT_FOV);
            }
            else
            {
                Debug.Log("shooting Hammer");
                return;
            }
        }
    }

    private void HandleHookShotMoveMent()
    {
        if(!speedUp)
        {
            SpeedUpEffect.GetComponent<ParticleSystem>().Play();
            energyCost -= 3;
            UpdateEnergyCost();
            flyingUI.SetActive(true);
            speedUp = true;
        }
        Anime.SetBool("shot", false);
        HandAnime.SetBool("shot", false);
        Anime.SetBool("flying", true);
        HandAnime.SetBool("flying", true);
        Vector3 HookShotDir = (HammerShotPosition - transform.position).normalized;

        float hookshotSpeedMin = 10f;
        float hookshotSpeedMax = 40f;
        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, HammerShotPosition), hookshotSpeedMin,
            hookshotSpeedMax);
        float hookshotMultiplier = 2f;

        playerCC.Move(HookShotDir * hookshotSpeed * hookshotMultiplier * Time.deltaTime);
        float reachedHookShotPositionDistance = 2f;
        if (Vector3.Distance(transform.position, HammerShotPosition) <= reachedHookShotPositionDistance)
        {
            StopHookShot();
        }
        if (TestInputDownHookShot())
        {
            StopHookShot();
        }
        if (TestInputJump())
        {
            float momentumExtraSpeed = 7f;
            characterVelocityMomentum = HookShotDir * hookshotSpeed * momentumExtraSpeed;
            float jumpspeed = 15f;
            characterVelocityMomentum += Vector3.up * jumpspeed;
            StopHookShot();
        }
    }

    [PunRPC]
    public void HammerSpeedUpEffect()
    {
        SpeedUpEffect.GetComponent<ParticleSystem>().Play();
        Debug.Log("play effect");
    }

    private void StopHookShot()
    {
        ChangeState(State.Normal);
        speedUp = false;
        flyingUI.SetActive(false);
        Rigidbody projectileRb = Hammer.GetComponent<Rigidbody>();
        projectileRb.isKinematic = false;
        projectileRb.velocity = Vector3.zero;
        projectileRb.angularVelocity = Vector3.zero;
        projectileRb.gameObject.transform.SetParent(HammerShotTransform);
        projectileRb.gameObject.transform.position = HammerShotTransform.position;
        projectileRb.gameObject.transform.rotation = HammerShotTransform.rotation;
        Anime.SetBool("shot", false);
        HandAnime.SetBool("shot", false);
        Anime.SetBool("flying", false);
        HandAnime.SetBool("flying", false);
        ResetGravtiyEffect();
        Hammer.SetActive(false);
        HandHammer.SetActive(true);
        HammerShotCDtimer = HammerShotCD;
        cameraFOV.SetCameraFOV(NORMAL_FOV);
        characterVelocityMomentum = new Vector3(0, characterVelocityMomentum.y, 0);
    }
    private bool TestInputDownHookShot()
    {
        return Input.GetKeyDown(KeyCode.E);
    }
    private bool TestInputJump()
    {
        return Input.GetKeyDown(KeyCode.Space);
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

    private void OnTriggerEnter(Collider other)
    {
        if (pv.IsMine)
        {
            if (other.gameObject.tag == "drowsy")
            {
                Debug.Log("trigger");
                drowsy = true;
                drowsyUI.SetActive(true);
                Invoke("ResetDrowsy", 10f);
            }
            if (energyCost < maxEnergy)
            {
                if (other.name == "energyPoint")
                {
                    energyCost = maxEnergy;
                    energyAudio.PlayOneShot(refillEnergyClip);
                    UpdateEnergyCost();
                    _hint.ChangeText("", false);
                    energyWarning.SetActive(false);
                    energyWarning.GetComponent<Animation>().Stop();
                }
            }
        }
    }
    void ResetDrowsy()
    {
        if (pv.IsMine)
        {
            drowsy = false;
            drowsyUI.SetActive(false);
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps["brokenCameraAmountCount"] != null)
        {
            if (!pv.IsMine && targetPlayer == pv.Owner)
            {
                brokenCameraAmount = (int)changedProps["brokenCameraAmountCount"];
                GM.GameStatusCheck();
            }
        }
    }
}
