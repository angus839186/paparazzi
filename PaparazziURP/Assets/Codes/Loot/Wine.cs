using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Threading;

public class Wine : MonoBehaviourPunCallbacks
{
    public GameObject foggyEffect;

    public GameObject boomEffect;

    public PhotonView pv;

    public Rigidbody rb;

    public GameObject wineObj;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Paparazzi")
        {
            return;
        }
        else
        {
            if (collision != null)
            {
                pv.RPC("BreakWine", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    IEnumerator BreakWine()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<BoxCollider>().isTrigger = true;
        wineObj.SetActive(false);
        GameObject foggy = Instantiate(foggyEffect, this.transform);
        GameObject boom = Instantiate(boomEffect, this.transform);
        yield return new WaitForSeconds(6f);
        if(pv.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
        yield break;
    }
}
