using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class agentTutorial : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        CheckPoint checkPoint = other.gameObject.GetComponent<CheckPoint>();
        if (checkPoint != null)
        {
            if(checkPoint._name == "agentLevel")
            {
                TutorialGameSceneManager.Instance.NextAgentStep();
                checkPoint.NextMove();
            }
        }
    }
}
