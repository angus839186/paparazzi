using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Weapon : MonoBehaviourPunCallbacks
{
    public agentPlayerController agentPlayer;

    public agentTracker tracker;
    private void OnTriggerEnter(Collider other)
    {
        PaparazziCameraObject Target = other.gameObject.GetComponentInParent<PaparazziCameraObject>();
        if(Target != null)
        {
            if(Target.HaveCamera != false)
            {
                Target.TakeDamage();
                agentPlayer.AddbrokenCameraAndCheck();
            }
            else
            {
                return;
            }
        }
    }
}
