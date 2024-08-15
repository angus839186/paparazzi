using Photon.Pun.Demo.SlotRacer.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class paparazziBot : MonoBehaviour
{
    public bool HaveCamera;
    public bool fixing;
    public bool canMove;

    [SerializeField]
    private ChasePoint chasePoints;

    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float distanceThreshold = 1f;

    public Transform currentChasePoint;

    [Range(0f, 15f)]
    public float RotationSpeed;
    private Quaternion rotationGoal;
    private Vector3 directionToWayPoint;

    public float fixProgress;
    public float maxfixProgress;
    public GameObject stunEffect;
    public Animator anime;

    private void Start()
    {
        currentChasePoint = chasePoints.GetNextChasePoint(currentChasePoint);
        transform.position = currentChasePoint.position;
        transform.LookAt(currentChasePoint);

        currentChasePoint = chasePoints.GetNextChasePoint(currentChasePoint);
    }
    public void Update()
    {
        if(HaveCamera)
        {
            if(canMove)
            {
                transform.position = Vector3.MoveTowards(transform.position, currentChasePoint.position,
                    moveSpeed * Time.deltaTime);
                RotateTowardWayPoint();


                anime.SetBool("Move", true);
                if (Vector3.Distance(currentChasePoint.position, transform.position) <= distanceThreshold)
                {
                    currentChasePoint = chasePoints.GetNextChasePoint(currentChasePoint);
                }

            }
        }
        else
        {
            transform.position = transform.position;
            anime.SetBool("Move", false);
            if (fixing)
            {
                fixProgress +=  3 * Time.deltaTime;
                if (fixProgress >= maxfixProgress)
                {
                    fixProgress = 0;
                    ResetCamera();
                }
            }
        }
    }
    public void RotateTowardWayPoint()
    {
        directionToWayPoint = (currentChasePoint.position - transform.position).normalized;
        rotationGoal = Quaternion.LookRotation(directionToWayPoint);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotationGoal, RotationSpeed * Time.deltaTime);
    }

    public void TakeDamage()
    {
        HaveCamera = false;
        canMove = false;
        stunEffect.SetActive(true);
        var stunParticles = stunEffect.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in stunParticles)
        {
            ps.Play();
        }
    }
    public void ResetCamera()
    {
        HaveCamera = true;
        fixing = false;
        var stunParticles = stunEffect.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in stunParticles)
        {
            ps.Play();
        }
        stunEffect.SetActive(false);
        TutorialGameSceneManager.Instance.alreadyFixPaparazziBot = true;
        TutorialGameSceneManager.Instance.FixLevelCheck();
    }

}
