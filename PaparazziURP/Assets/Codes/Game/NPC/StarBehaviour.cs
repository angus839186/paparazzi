using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class StarBehaviour : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    public NavMeshAgent ai;
    public Transform target;
    public Transform[] EscapePoints;
    public int currentTarget;
    public int previousTarget;
    public bool walking;
    public Animation shockAnimation;

    public int walkingSpeed;
    public int runningSpeed;

    public float patrolRange;

    public Animator anime;

    public enum AiState { walking, running}

    public AiState state;

    public void Start()
    {
        if(pv.IsMine)
        {
            SetSpawnPoint();
        }
    }
    public void SetSpawnPoint()
    {
        int randomNumber = Random.Range(0, EscapePoints.Length);
        currentTarget = randomNumber;
        target.position = EscapePoints[currentTarget].position;
        ai.transform.position = target.position;
        anime.SetFloat("starSpeed", 0);
        previousTarget = currentTarget;
        Invoke("WatchMode", 3f);
        Debug.Log(currentTarget);
    }
    private void FixedUpdate()
    {
        Vector3 currentPoint = new Vector3(target.position.x, transform.position.y, target.position.z);
        switch (state)
        {

            case AiState.walking:
                if (Vector3.Distance(currentPoint, transform.position) <= ai.stoppingDistance)
                {
                    WalkingMode();
                }
                break;
        }
    }
    public void ChangeState(AiState _state)
    {
        pv.RPC("RPC_ChangeState", RpcTarget.All, _state);
    }
    [PunRPC]
    public void RPC_ChangeState(AiState _state)
    {
        if (!pv.IsMine)
            return;
        state = _state;
        if (state == AiState.running)
        {
            anime.SetFloat("starSpeed", 1);
            shockAnimation.Play();
            FindNewRunPoint();
            return;
        }
        else
        {
            if (state == AiState.walking)
            {
                anime.SetFloat("starSpeed", 0.5f);
                WalkingMode();
            }
        }
    }
    public void OnGUI()
    {
        if (GUI.Button(new Rect(60, 60, 200, 100), "Click"))
        {
            ChangeState(AiState.running);
        }
    }
    public void FindNewRunPoint()
    {
        ai.speed = runningSpeed;
        while (currentTarget == previousTarget)
        {
            currentTarget = Random.Range(0, EscapePoints.Length);
        }
        target.position = EscapePoints[currentTarget].position;
        previousTarget = currentTarget;
        Debug.Log(currentTarget);
        ai.speed = runningSpeed;
        ai.SetDestination(target.position);
        StartCoroutine(Running());

    }
    IEnumerator Running()
    {
        Vector3 _runningPoint = new Vector3(target.position.x, target.position.y, target.position.z);
        while (Vector3.Distance(_runningPoint, transform.position) >= 3f)
        {
            yield return null;
        }
        Debug.Log("backToWalk");
        target.position = transform.position;
        ai.transform.position = target.position;
        anime.SetFloat("starSpeed", 0);
        Invoke("WatchMode", 3f);
        yield break;
    }
    public void WalkingMode()
    {
        ai.speed = walkingSpeed;
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        NavMeshHit hit;
        target.position = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
        ai.SetDestination(target.position);
        Debug.Log("Find New Target");
    }

    void WatchMode()
    {
        ChangeState(AiState.walking);
    }
}
