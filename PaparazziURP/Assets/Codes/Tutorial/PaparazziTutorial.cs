using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PaparazziTutorial : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        CheckPoint checkPoint = other.gameObject.GetComponent<CheckPoint>();
        if(checkPoint != null)
        {
            if(checkPoint._name == "MoveLevel")
            {
                TutorialGameSceneManager.Instance.currentMoveNumber++;
                TutorialGameSceneManager.Instance.MoveLevelCheck();
                checkPoint.NextMove();
            }
            if(checkPoint._name == "chasingLevel")
            {
                TutorialGameSceneManager.Instance.GetInChasingLevel = true;
                TutorialGameSceneManager.Instance.chasingLevelCheck();
                checkPoint.NextMove();
            }
        }
    }


}
