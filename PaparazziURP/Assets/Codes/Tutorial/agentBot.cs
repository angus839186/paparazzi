using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class agentBot : MonoBehaviour
{
    public Transform target;

    public bool chasing;
    public float originSpeed;
    public float chasingTime;

    public bool attacked;
    public bool drowsy;

    public NavMeshAgent agent;
    public Animator anime;

    public void Start()
    {
        agent.speed = originSpeed;
    }

    private void Update()
    {
        if(target!= null)
        {
            if(chasing)
            {
                agent.SetDestination(target.position);
                anime.SetBool("move", true);
                chasingTime += Time.deltaTime;
                if (chasingTime >= 5)
                {
                    agent.speed = 3.8f;
                }
                if (Vector3.Distance(transform.position, target.position) <= 1.5f)
                {
                    agent.SetDestination(transform.position);
                    Attack();
                    chasing = false;
                }
            }
        }
        else
        {
            agent.SetDestination(transform.position);
            anime.SetBool("move", false);
        }
    }
    public void Attack()
    {
        anime.SetTrigger("attack");
        if (target.GetComponentInParent<paparazziBot>() != null)
        {
            target.GetComponentInParent<paparazziBot>().TakeDamage();
            TutorialGameSceneManager.Instance.FixLevelCheck();
        }
        if(target.GetComponentInParent<TutorialCameraObject>() != null)
        {
            target.GetComponentInParent<TutorialCameraObject>().TakeDamage();
            TutorialGameSceneManager.Instance.FixLevelCheck();
        }
        agent.speed = originSpeed;
        target = null;
        chasingTime = 0;
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "drowsy")
        {
            Debug.Log("trigger");
            drowsy = true;
            TutorialGameSceneManager.Instance.wineHitAgent = true;
            TutorialGameSceneManager.Instance.ItemLevelCheck();
            Invoke("ResetDrowsy", 10f);
        }
    }
    public void ResetDrowsy()
    {
        drowsy = false;
    }
}
