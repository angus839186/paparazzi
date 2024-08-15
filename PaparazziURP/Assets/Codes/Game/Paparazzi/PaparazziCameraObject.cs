using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEditor.Rendering;

public class PaparazziCameraObject : MonoBehaviourPunCallbacks, IPunObservable
{
    #region references
    public PhotonView pv;

    [Header("身上照片UI")]
    public Image UploadedPhotoProgress;
    public Text PhotoText;
    public int PhotoOnCamera;
    public int maxPhotoOnCamera;

    [Header("相機狀態UI")]
    public bool HaveCamera;
    public GameObject hurtUI;
    public AudioClip BreakCameraAudio;

    [Header("上傳UI")]
    public float UpLoadProgress;
    public float maxUpLoadProgress;
    public Image UpLoadProgressImage;
    public bool Uploading;

    [Header("訊號")]
    public Collider[] Signals;
    public float signalRadius;
    public LayerMask signalMask;
    public int level;
    public GameObject targetSignal;
    public GameObject signal1;
    public GameObject signal2;
    public GameObject signal3;


    [Header("拍照")]
    public Camera cam;
    public LayerMask obstacleMask;
    public LayerMask starMask;
    public int PhotoProgress;
    public int maxPhotoProgress;
    public Image PhotoProgressImage;
    public float takePhotoCD;
    public float takePhotoCDtimer;
    public bool canSeeStar;
    public bool canTakePhoto;
    public Image TakePhotoImage;
    public GameObject TakePhotoHint;
    public AudioClip Sound_takePhoto;
    public AudioSource _CameraAudioSource;
    public float PhotoEffectSpeed;
    public bool focusing;
    private CameraFOV cameraFOV;
    private const float NORMAL_FOV = 60f;
    private const float CameraMode_FOV = 40f;
    public GameObject takePhotoUI;
    public Animation CameraAnimation;
    public CanvasGroup normalUIcanvas;

    [Header("相機數量UI")]
    public Text cameraAmountText;

    [Header("明星")]
    public float starRadius;
    public Collider playercollider;
    private Plane[] planes;
    public Collider[] rangeChecks;

    [Header("修復相機")]
    public float RepairingProgress;
    public float maxRepairingProgress;
    public Image RepairingImage;
    public float RepairingSpeed;
    public float RepairingSelfSpeed;
    public PaparazziCameraObject RepairingTarget;
    public bool RepairingSelf;

    [Header("丟酒瓶")]
    public string wineName;
    public int wineCount;
    public GameObject[] wineItemUI;
    public GameObject wine;
    public Transform ThrowTransform;
    public float throwForce;

    [Header("道具選擇")]
    public int currentItem;

    [Header("動畫")]
    public Animator anime;
    public Animator HandAnime;
    public Animation warningAnime;

    [Header("特效")]
    public GameObject PhotoEffect;
    public GameObject stunEffect;

    [Header("場景參考物")]
    public PaparazziPlayerController paparazzi;
    public SNGcar cameraManager;
    public UploadedPhotoUI _UploadedPhoto;
    public GameObject star;

    [Header("提示")]
    public Hinter hint;


    #endregion

    public void UpdateUploadedPhotoUI()
    {
        float UploadedPhotoPercent = (float)_UploadedPhoto.currentPhoto / (float)_UploadedPhoto.MaxUploadedPhoto;
        UploadedPhotoProgress.fillAmount = UploadedPhotoPercent;
    }


    private void Start()
    {
        if(pv.IsMine)
        {
            _UploadedPhoto = FindObjectOfType<UploadedPhotoUI>();
            cameraManager = FindObjectOfType<SNGcar>();
            star = GameObject.Find("Star");
            currentItem = 1;

            playercollider = star.GetComponent<Collider>();
            cameraFOV = cam.GetComponent < CameraFOV>();

            ChangeCameraState(true);
            UpdatePhotoUI();
            UpdateUploadedPhotoUI();
            UpdateCameraAmountUI();
            SwitchItem(1);
        }
    }

    private void Update()
    {
        if(pv.IsMine)
        {
            FieldOfViewCheck();
            CheckCloseSignal();
            UpdateUploadedPhotoUI();
            UpdateCameraAmountUI();
            if (!canTakePhoto)
            {
                CameraCD();
            }
            if(currentItem == 1)
            {
                if(HaveCamera)
                {
                    if (Input.GetMouseButton(1))
                    {
                        ToggleCameraMode(true);
                    }
                    if(Input.GetMouseButtonUp(1))
                    {
                        ToggleCameraMode(false);
                    }
                    if (focusing)
                    {
                        if(Input.GetMouseButtonDown(0) && canSeeStar && canTakePhoto)
                        {
                            TakePhoto();
                        }
                    }
                }
            }
            if(currentItem == 2)
            {
                if(Input.GetMouseButtonDown(0) && wineCount != 0)
                {
                    anime.SetTrigger("Throw");
                    HandAnime.SetTrigger("Throw");
                }
            }
            if(Input.GetKey(KeyCode.E))
            {
                HandleUploadPhoto();
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                StopUploadPhoto();
            }
            if(!HaveCamera)
            {
                if (Input.GetKey(KeyCode.V))
                {
                    RepairingOwnCamera(true);
                }
                if (Input.GetKeyUp(KeyCode.V))
                {
                    RepairingOwnCamera(false);
                }
            }
            if(Input.GetKeyDown(KeyCode.Q))
            {
                if(currentItem == 1)
                {
                    SwitchItem(2);
                }
                else
                {
                    SwitchItem(1);
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                var ray = new Ray(cam.transform.position, cam.transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 5f))
                {
                    if (hit.collider.gameObject.TryGetComponent(out PaparazziCameraObject OtherPlayer))
                    {
                        if (OtherPlayer != null)
                        {

                            if (OtherPlayer.HaveCamera == true)
                            {
                                RepairingTarget = null;
                                return;
                            }
                            else
                            {
                                RepairingTarget = OtherPlayer;
                                RepairingTarget.RepairingCamera();
                                paparazzi.canMove = false;
                            }

                        }
                    }
                }
            }

            if (RepairingTarget != null)
            {
                if(RepairingTarget.HaveCamera == false)
                {
                    RepairingImage.gameObject.SetActive(true);
                    RepairingImage.fillAmount = RepairingTarget.RepairingProgress / maxRepairingProgress;
                }
                else
                {
                    RepairingTarget = null;
                    RepairingImage.fillAmount = 0 / maxRepairingProgress;
                    RepairingImage.gameObject.SetActive(false);
                    paparazzi.ResetMove();
                }
            }
        }
    }

    public void SwitchItem(int _ItemIndex)
    {
        currentItem = _ItemIndex;
        if(currentItem == 1)
        {
            wine.SetActive(false);
            if(pv.IsMine)
            {
                anime.SetBool("HoldWine", false);
                HandAnime.SetBool("HoldWine", false);
            }
        }
        else
        {
            wine.SetActive(true);
            if(pv.IsMine)
            {
                ToggleCameraMode(false);
                anime.SetBool("HoldWine", true);
                HandAnime.SetBool("HoldWine", true);
            }
        }
        if (pv.IsMine)
        {
            int changItemNumber = currentItem;
            Hashtable hash = new Hashtable();
            hash.Add("currentItemNumber", changItemNumber);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public void ThrowWine()
    {
        wineCount--;
        for (int i = 0; i < wineItemUI.Length ; i++)
        {
            if(i < wineCount)
            {
                wineItemUI[i].SetActive(true);
            }
            else
            {
                wineItemUI[i].SetActive(false);
            }
        }
        GameObject wineObj = PhotonNetwork.Instantiate(wineName, ThrowTransform.position, ThrowTransform.rotation);
        Rigidbody wineRB = wineObj.GetComponent<Rigidbody>();
        Vector3 forceDirection = cam.transform.forward;
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 500f))
        {
            forceDirection = (hit.point - ThrowTransform.position).normalized;
        }
        Vector3 forceToAdd = forceDirection * throwForce;
        wineRB.useGravity = true;
        wineRB.isKinematic = false;
        wineRB.AddForce(forceToAdd, ForceMode.Impulse);
        wineRB.transform.SetParent(null);
        anime.SetBool("HoldWine", false);
        HandAnime.SetBool("HoldWine", false);
        anime.ResetTrigger("Throw");
        HandAnime.ResetTrigger("Throw");
        SwitchItem(1);
    }

    public void ToggleCameraMode(bool _cameraMode)
    {
        focusing = _cameraMode;
        if(focusing)
        {
            cameraFOV.SetCameraFOV(CameraMode_FOV);
            normalUIcanvas.alpha = 0f;
            takePhotoUI.SetActive(true);
            paparazzi.speed = 6;
        }
        else
        {
            cameraFOV.SetCameraFOV(NORMAL_FOV);
            normalUIcanvas.alpha = 1f;
            takePhotoUI.SetActive(false);
            paparazzi.speed = 12.5f;
        }
    }
    void CameraCD()
    {
        if(takePhotoCDtimer < takePhotoCD)
        {
            takePhotoCDtimer += Time.deltaTime;
            TakePhotoImage.fillAmount = takePhotoCDtimer / takePhotoCD;
        }
        else
        {
            takePhotoCDtimer = 0;
            TakePhotoImage.fillAmount = 1;
            TakePhotoHint.SetActive(true);
            canTakePhoto = true;
        }
        
    }
    #region UploadFunction

    void HandleUploadPhoto()
    {
        if(PhotoOnCamera != 0 && HaveCamera)
        {
            Uploading = true;
            if (Uploading)
            {
                UpLoadProgress += Time.deltaTime * level;
                UpLoadProgressImage.gameObject.SetActive(true);
                UpLoadProgressImage.fillAmount = UpLoadProgress / maxUpLoadProgress;
                if (UpLoadProgress >= maxUpLoadProgress)
                {
                    UploadPhoto(PhotoOnCamera);
                    PhotoOnCamera = 0;
                    StopUploadPhoto();
                    pv.RPC(nameof(RPC_SetPhoto), RpcTarget.All);
                    UpdatePhotoUI();
                }
            }
        }
        else
        {
            return;
        }
    }

    public void StopUploadPhoto()
    {
        Uploading = false;
        UpLoadProgressImage.gameObject.SetActive(false);
        UpLoadProgress = 0;
        UpLoadProgressImage.fillAmount = UpLoadProgress / maxUpLoadProgress;
    }
    public void UploadPhoto(int photo)
    {
        _UploadedPhoto.UploadPhoto(photo);
        hint.ChangeText("", false);
        UpdateUploadedPhotoUI();
    }
    public void UpdateCameraAmountUI()
    {
        cameraAmountText.text = "x" + cameraManager.CameraAmount;
    }
    #endregion

    #region PhotonSyncFunction

    public void SetCameraBool()
    {
        bool HaveCameraBool = HaveCamera;
        Hashtable hash = new Hashtable();
        hash.Add("HaveCameraBool", HaveCameraBool);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void SetPhotoCount()
    {
        if (pv.IsMine)
        {
            int currentPhotoCount = PhotoOnCamera;
            Hashtable hash = new Hashtable();
            hash.Add("currentPhotoCount", currentPhotoCount);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps["HaveCameraBool"] != null)
        {
            if (!pv.IsMine && targetPlayer == pv.Owner)
            {
                HaveCamera = (bool)changedProps["HaveCameraBool"];
            }
        }
        if (changedProps["currentPhotoCount"] != null)
        {
            if (!pv.IsMine && targetPlayer == pv.Owner)
            {
                PhotoOnCamera = (int)changedProps["currentPhotoCount"];
            }
        }
        if (changedProps["currentItemNumber"] != null)
        {
            if(!pv.IsMine && targetPlayer == pv.Owner)
            {
                SwitchItem((int)changedProps["currentItemNumber"]);
            }
        }
    }

    #endregion

    #region CameraState

    public void ChangeCameraState(bool haveCamera)
    {
        HaveCamera = haveCamera;
        anime.SetBool("HaveCamera", HaveCamera);
        if (pv.IsMine)
        {
            SetCameraBool();
        }
    }

    public void TakeDamage()
    {
        pv.RPC("RPC_TakeDamage", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RPC_TakeDamage()
    {
        if (pv.IsMine)
        {
            PhotoOnCamera = 0;
            PhotoProgress = 0;
            hint.ChangeText("趕快找地方躲起來修相機，或是尋求隊友的幫助", true);
            ToggleCameraMode(false);
            UpdatePhotoProgressUI();
            UpdatePhotoUI();
            paparazzi.SetTrack(false);
            ChangeCameraState(false);
            hurtUI.SetActive(true);
            warningAnime.Play();
        }
        _CameraAudioSource.PlayOneShot(BreakCameraAudio);
        var stunParticles = stunEffect.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in stunParticles)
        {
            ps.Play();
        }

    }

    #endregion

    #region CameraFunction
    void TakeCamera()
    {
        pv.RPC("RPC_TakeCamera", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RPC_TakeCamera()
    {
        if (pv.IsMine)
        {
            RepairingProgress = 0;
            RepairingImage.fillAmount = RepairingProgress / maxRepairingProgress;
            RepairingImage.gameObject.SetActive(false);
            paparazzi.ResetMove();
            hint.ChangeText("", false);
            ChangeCameraState(true);
            hurtUI.SetActive(false);
            warningAnime.Stop();
            cameraManager.CameraAmountChange();
        }
        var stunParticles = stunEffect.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in stunParticles)
        {
            ps.Stop();
        }

    }

    public void TakePhoto()
    {
        _CameraAudioSource.PlayOneShot(Sound_takePhoto);
        anime.SetTrigger("TakePhoto");
        if (pv.IsMine)
        {
            paparazzi.StopToTakePhoto();
            PhotoProgress++;
            canTakePhoto = false;
            TakePhotoHint.SetActive(false);
            TakePhotoImage.fillAmount = 0;
            if(PhotoProgress == maxPhotoProgress)
            {
                if(PhotoOnCamera < maxPhotoOnCamera)
                {
                    StartCoroutine(DropPhotoCoroutine());
                    star.GetComponentInParent<StarBehaviour>().ChangeState(StarBehaviour.AiState.running);
                }
                PhotoProgress = 0;
            }
            if(PhotoOnCamera == maxPhotoOnCamera)
            {
                hint.ChangeText("底片滿了，要先上傳相片", true);
            }
            else
            {
                if(PhotoOnCamera != 0)
                {
                    hint.ChangeText("按住E可以上傳相片", true);
                }
            }
            UpdatePhotoProgressUI();
        }
    }
    IEnumerator DropPhotoCoroutine()
    {
        GameObject Photo = Instantiate(PhotoEffect, star.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        yield return new WaitForSeconds(2f);
        float Distance = Vector3.Distance(Photo.transform.position, cam.transform.position);
        Photo.GetComponent<Rigidbody>().useGravity = false;
        Photo.GetComponentInChildren<BoxCollider>().isTrigger = true;
        while (Distance > 0.1)
        {
            if (Photo != null)
            {
                Photo.transform.position = Vector3.Lerp(Photo.transform.position,
                cam.transform.position, PhotoEffectSpeed * Time.deltaTime);
            }
            yield return null;
        }
        yield break;
    }
    public void UpdatePhotoProgressUI()
    {
        float _PhotoProgress = (float)PhotoProgress / (float)maxPhotoProgress;
        PhotoProgressImage.fillAmount = _PhotoProgress;
    }
    public void GetPhoto()
    {
        if (pv.IsMine)
        {
            PhotoOnCamera++;
        }
        pv.RPC(nameof(RPC_SetPhoto), RpcTarget.All);
    }

    [PunRPC]
    void RPC_SetPhoto()
    {
        if (pv.IsMine)
        {
            SetPhotoCount();
        }

    }

    #endregion


    #region PhotoState

    public void UpdatePhotoUI()
    {
        if (pv.IsMine)
        {
            PhotoText.text = PhotoOnCamera.ToString();
        }
    }

    #endregion

    #region CheckSignal
    void CheckCloseSignal()
    {
        Signals = Physics.OverlapSphere(transform.position, signalRadius, signalMask);
        if (Signals.Length != 0)
        {
            float min = Vector3.Distance(transform.position, Signals[0].transform.position);
            int location = 0;
            for (int i = 0; i < Signals.Length; i++)
            {
                if (Vector3.Distance(transform.position, Signals[i].transform.position) < min)
                {
                    targetSignal = Signals[i].gameObject;
                    location = i;
                }

            }
            Vector3 directionToPaparazzi = new Vector3(Signals[location].transform.position.x, transform.position.y, Signals[location].transform.position.z);
            float SignalToPaparazzi = Vector3.Distance(directionToPaparazzi, transform.position);
            if (SignalToPaparazzi >= signalRadius)
            {
                level = 1;
                
            }
            if (SignalToPaparazzi <= signalRadius)
            {
                if (SignalToPaparazzi <= 30)
                {
                    level = 2;
                }
                if (SignalToPaparazzi <= 20)
                {
                    level = 3;
                }
            }
        }
        else
        {
            level = 1;
        }
        SignalDetect();
    }
    public void SignalDetect()
    {
        if(level == 1)
        {
            signal1.SetActive(true);
            signal2.SetActive(false);
            signal3.SetActive(false);
        }
        if(level == 2)
        {
            signal1.SetActive(true);
            signal2.SetActive(true);
            signal3.SetActive(false);
        }
        if(level == 3)
        {
            signal1.SetActive(true);
            signal2.SetActive(true);
            signal3.SetActive(true);
        }
    }

    #endregion

    #region CheckStarFunction

    private void FieldOfViewCheck()
    {
        if (cam != null)
        {
            planes = GeometryUtility.CalculateFrustumPlanes(cam);
        }
        if (!GeometryUtility.TestPlanesAABB(planes, playercollider.bounds))
        {
            canSeeStar = false;
        }
        rangeChecks = Physics.OverlapSphere(transform.position, starRadius, starMask);
        if (rangeChecks.Length != 0)
        {
            Transform star = rangeChecks[0].transform;
            Vector3 directionToStar = (star.position - cam.transform.position).normalized;
            float distanceToStar = Vector3.Distance(cam.transform.position, star.position);
            if (!Physics.Raycast(cam.transform.position, directionToStar, distanceToStar, obstacleMask))
            {
                canSeeStar = true;
            }
            else
            {
                canSeeStar = false;
            }
        }
    }

    #endregion
    private void OnTriggerEnter(Collider other)
    {

        if (pv.IsMine)
        {
            if (other.gameObject.tag == "Photo")
            {
                if (PhotoOnCamera < maxPhotoOnCamera)
                {
                    GetPhoto();
                    UpdatePhotoUI();
                }
                Destroy(other.gameObject);
            }
        }
    }
    public void RepairingOwnCamera(bool RepairingSelfBool)
    {
        if(pv.IsMine)
        {
            if(RepairingSelfBool)
            {
                    if (RepairingProgress <= maxRepairingProgress)
                    {
                        RepairingImage.gameObject.SetActive(true);
                        RepairingProgress += RepairingSelfSpeed * Time.deltaTime;
                        RepairingImage.fillAmount = RepairingProgress / maxRepairingProgress;
                        paparazzi.canMove = false;
                    }
                    else
                    {
                        TakeCamera();
                    }
            }
            else
            {
                RepairingImage.gameObject.SetActive(false);
                RepairingProgress = 0;
                RepairingImage.fillAmount = RepairingProgress / maxRepairingProgress;
                paparazzi.canMove = true;
            }
        }
    }
    public void RepairingCamera()
    {
        pv.RPC("RPC_Repairing", RpcTarget.AllBuffered);
    }

    [PunRPC]
    IEnumerator RPC_Repairing()
    {
        if (pv.IsMine)
        {
            while (!HaveCamera)
            {
                if (RepairingProgress <= maxRepairingProgress)
                {
                    RepairingImage.gameObject.SetActive(true);
                    RepairingProgress += RepairingSpeed * Time.deltaTime;
                    RepairingImage.fillAmount = RepairingProgress / maxRepairingProgress;
                    paparazzi.canMove = false;
                }
                else
                {
                    TakeCamera();
                }
                yield return null;
            }
            yield break;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(RepairingProgress);
        }
        else
        {
            if(stream.IsReading)
            {
                RepairingProgress = (float)stream.ReceiveNext();
            }
        }
    }
}
