using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public GameObject NextCheckPoint;
    public string _name;
    public AudioClip checkPointAudio;
    public void NextMove()
    {
        if(NextCheckPoint != null)
        {
            if (NextCheckPoint.activeSelf == false)
            {
                NextCheckPoint.SetActive(true);
            }
            else
            {
                NextCheckPoint.SetActive(false);
            }
        }
        gameObject.SetActive(false);
        Debug.Log("Get");
        TutorialGameSceneManager.Instance.GetComponent<AudioSource>().PlayOneShot(checkPointAudio);
    }
}
