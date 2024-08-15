using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class TutorialCameraObject : MonoBehaviourPunCallbacks
{
    #region references
    public PhotonView pv;

    [Header("���W�Ӥ�UI")]
    public Image UploadedPhotoProgress;
    public Text PhotoText;
    public int PhotoOnCamera;
    public int maxPhotoOnCamera;

    [Header("�۾����AUI")]
    public bool HaveCamera;
    public GameObject hurtUI;

    [Header("�W��UI")]
    public float UpLoadProgress;
    public float maxUpLoadProgress;
    public Image UpLoadProgressImage;
    public bool Uploading;

    [Header("�T��")]
    public Collider[] Signals;
    public float signalRadius;
    public LayerMask signalMask;
    public int level;
    public GameObject targetSignal;
    public GameObject signal1;
    public GameObject signal2;
    public GameObject signal3;


    [Header("���")]
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

    [Header("�۾��ƶqUI")]
    public Text cameraAmountText;

    [Header("���P")]
    public float starRadius;
    public Collider playercollider;
    private Plane[] planes;
    public Collider[] rangeChecks;

    [Header("�״_�۾�")]
    public float RepairingProgress;
    public float maxRepairingProgress;
    public Image RepairingImage;
    public float RepairingSpeed;
    public float RepairingSelfSpeed;
    public paparazziBot RepairingTarget;
    public bool RepairingSelf;

    [Header("��s�~")]
    public string wineName;
    public int wineCount;
    public GameObject winePrefab;
    public GameObject wine;
    public Transform ThrowTransform;
    public float throwForce;

    [Header("�D����")]
    public int currentItem;

    [Header("�ʵe")]
    public Animator anime;
    public Animator HandAnime;
    public Animation warningAnime;

    [Header("�S��")]
    public GameObject PhotoEffect;
    public GameObject stunEffect;

    [Header("�����ѦҪ�")]
    public PaparazziPlayerController paparazzi;
    public SNGcar cameraManager;
    public GameObject star;


    #endregion
    private void Start()
    {
        if (pv.IsMine)
        {
            star = GameObject.Find("Star");
            currentItem = 1;

            playercollider = star.GetComponent<Collider>();
            cameraFOV = cam.GetComponent<CameraFOV>();

            ChangeCameraState(true);
            UpdatePhotoUI();
            UpdateUploadedPhotoUI();
            UpdateCameraAmountUI();
            SwitchItem(1);
        }
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            FieldOfViewCheck();
            CheckCloseSignal();
            UpdateUploadedPhotoUI();
            UpdateCameraAmountUI();
            if (!canTakePhoto)
            {
                CameraCD();
            }
            if (currentItem == 1)
            {
                if (HaveCamera)
                {
                    if (Input.GetMouseButton(1))
                    {
                        ToggleCameraMode(true);
                    }
                    if (Input.GetMouseButtonUp(1))
                    {
                        ToggleCameraMode(false);
                    }
                    if (focusing)
                    {
                        if (Input.GetMouseButtonDown(0) && canSeeStar && canTakePhoto)
                        {
                            TakePhoto();
                        }
                    }
                }
            }
            if (currentItem == 2)
            {
                if (Input.GetMouseButtonDown(0) && wineCount != 0)
                {
                    anime.SetTrigger("Throw");
                    HandAnime.SetTrigger("Throw");
                    wineCount--;
                }
            }
            if (Input.GetKey(KeyCode.E))
            {
                HandleUploadPhoto();
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                StopUploadPhoto();
            }
            if (!HaveCamera)
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
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (currentItem == 1)
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
                    if (hit.collider.gameObject.TryGetComponent(out paparazziBot OtherPlayer))
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
                                RepairingTarget.fixing = true;
                                paparazzi.canMove = false;
                            }

                        }
                    }
                }
            }

            if (RepairingTarget != null)
            {
                if (RepairingTarget.HaveCamera == false)
                {
                    RepairingImage.gameObject.SetActive(true);
                    RepairingImage.fillAmount = RepairingTarget.fixProgress / RepairingTarget.maxfixProgress;
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

    public void UpdateUploadedPhotoUI()
    {
        float UploadedPhotoPercent = (float)TutorialGameSceneManager.Instance.currentUploadPhoto / 
            TutorialGameSceneManager.Instance.maxUploadPhoto;
        UploadedPhotoProgress.fillAmount = UploadedPhotoPercent;
    }

    public void SwitchItem(int _ItemIndex)
    {
        currentItem = _ItemIndex;
        if (currentItem == 1)
        {
            wine.SetActive(false);
            if (pv.IsMine)
            {
                anime.SetBool("HoldWine", false);
                HandAnime.SetBool("HoldWine", false);
            }
        }
        else
        {
            wine.SetActive(true);
            if (pv.IsMine)
            {
                ToggleCameraMode(false);
                anime.SetBool("HoldWine", true);
                HandAnime.SetBool("HoldWine", true);
            }
        }
    }

    public void ThrowWine()
    {
        GameObject wineObj = Instantiate(winePrefab, ThrowTransform.position, ThrowTransform.rotation);
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
        if (focusing)
        {
            cameraFOV.SetCameraFOV(CameraMode_FOV);
            normalUIcanvas.alpha = 0f;
            takePhotoUI.SetActive(true);
            paparazzi.speed = 2;
        }
        else
        {
            cameraFOV.SetCameraFOV(NORMAL_FOV);
            normalUIcanvas.alpha = 1f;
            takePhotoUI.SetActive(false);
            paparazzi.speed = 4f;
        }
    }
    void CameraCD()
    {
        if (takePhotoCDtimer < takePhotoCD)
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
        if (PhotoOnCamera != 0 && HaveCamera)
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
        TutorialGameSceneManager.Instance.currentUploadPhoto++;
        TutorialGameSceneManager.Instance.alreadyUpload = true;
        TutorialGameSceneManager.Instance.UploadPhotoLevelCheck();
        UpdateUploadedPhotoUI();
    }
    public void UpdateCameraAmountUI()
    {
        cameraAmountText.text = "x" + TutorialGameSceneManager.Instance.CameraAmount;
    }
    #endregion

    #region PhotonSyncFunction

    #endregion

    #region CameraState

    public void ChangeCameraState(bool haveCamera)
    {
        HaveCamera = haveCamera;
        anime.SetBool("HaveCamera", HaveCamera);
    }

    public void TakeDamage()
    {
        if (pv.IsMine)
        {
            PhotoOnCamera = 0;
            PhotoProgress = 0;
            ToggleCameraMode(false);
            UpdatePhotoProgressUI();
            UpdatePhotoUI();
            if (paparazzi.Tracking == true)
            {
                paparazzi.SetTrack(false);
            }
            ChangeCameraState(false);
            hurtUI.SetActive(true);
            warningAnime.Play();
        }
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
        if (pv.IsMine)
        {
            RepairingProgress = 0;
            RepairingImage.fillAmount = RepairingProgress / maxRepairingProgress;
            RepairingImage.gameObject.SetActive(false);
            paparazzi.ResetMove();
            ChangeCameraState(true);
            hurtUI.SetActive(false);
            warningAnime.Stop();
            TutorialGameSceneManager.Instance.CameraAmount--;
            TutorialGameSceneManager.Instance.alreadyFixPaparazziPlayer = true;
            TutorialGameSceneManager.Instance.FixLevelCheck();
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
        if (pv.IsMine)
        {
            paparazzi.StopToTakePhoto();
            PhotoProgress++;
            canTakePhoto = false;
            TakePhotoHint.SetActive(false);
            TakePhotoImage.fillAmount = 0;
            if (PhotoProgress == maxPhotoProgress)
            {
                if (PhotoOnCamera < maxPhotoOnCamera)
                {
                    StartCoroutine(DropPhotoCoroutine());
                    TutorialGameSceneManager.Instance.alreadyTakePhoto = true;
                    TutorialGameSceneManager.Instance.UploadPhotoLevelCheck();
                    star.GetComponentInParent<StarBot>().ChangeState(StarBot.AiState.running);
                }
                PhotoProgress = 0;
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
        if (level == 1)
        {
            signal1.SetActive(true);
            signal2.SetActive(false);
            signal3.SetActive(false);
        }
        if (level == 2)
        {
            signal1.SetActive(true);
            signal2.SetActive(true);
            signal3.SetActive(false);
        }
        if (level == 3)
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
        if (pv.IsMine)
        {
            if (RepairingSelfBool)
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
        StartCoroutine(Repairing());
    }
    IEnumerator Repairing()
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
}
