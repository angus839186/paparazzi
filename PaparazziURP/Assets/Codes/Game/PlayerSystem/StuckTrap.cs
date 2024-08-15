using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StuckTrap : MonoBehaviour
{
    public float durationTime;
    private void Update()
    {
        if (durationTime > 0)
        {
            durationTime -= Time.deltaTime;
        }
        else
        {
            DestroyByMasterClient();
        }
    }
    public void DestroyByMasterClient()
    {
        FindObjectOfType<AgentTrap>().RemoveTrapOnScene(gameObject);
        Destroy(gameObject);
    }

}
