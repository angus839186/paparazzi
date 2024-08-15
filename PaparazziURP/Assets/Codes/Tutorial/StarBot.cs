using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StarBot : MonoBehaviour
{
    public NavMeshAgent ai;
    public Transform target;
    public Transform[] EscapePoints;

    public int walkingSpeed;
    public int runningSpeed;

    public float patrolRange;

    public Animator anime;

    public enum AiState { none, walking, running}

    public AiState state;

    public void Start()
    {
        ChangeState(AiState.none);
    }
    private void Update()
    {
        Vector3 currentPoint = new Vector3(target.position.x, transform.position.y, target.position.z);
        switch (state)
        {
            case AiState.none:
                anime.SetFloat("starSpeed", 0, 0.1f, Time.deltaTime);
                break;
            case AiState.walking:
                anime.SetFloat("starSpeed", 0.5f, 0.1f, Time.deltaTime);
                if (Vector3.Distance(currentPoint, transform.position) <= ai.stoppingDistance)
                {
                    WalkingMode();
                }
                break;
            case AiState.running:
                ai.SetDestination(target.position);
                if (Vector3.Distance(currentPoint, transform.position) <= ai.stoppingDistance)
                {

                    anime.SetFloat("starSpeed", 0, 0.1f, Time.deltaTime);
                    Invoke("WatchMode", 3f);
                }
                else
                {
                    anime.SetFloat("starSpeed", 1, 0.1f, Time.deltaTime);
                }

                break;
        }
    }
    public void ChangeState(AiState _state)
    {
        state = _state;
        if (state == AiState.running)
        {
            ai.speed = runningSpeed;
            float max = Vector3.Distance(transform.position, EscapePoints[0].position);
            int location = 0;
            for (int i = 0; i < EscapePoints.Length; i++)
            {
                if (Vector3.Distance(transform.position, EscapePoints[i].position) > max)
                {
                    target.position = EscapePoints[i].position;
                    location = i;
                }

            }
            target.position = EscapePoints[location].position;
        }
        if (state == AiState.walking)
        {
            WalkingMode();
        }
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
    }

    void WatchMode()
    {
        ChangeState(AiState.none);
    }
}
