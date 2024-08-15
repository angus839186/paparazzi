using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWine : MonoBehaviour
{
    public GameObject foggyEffect;

    public GameObject boomEffect;

    public Rigidbody rb;

    public GameObject wineObj;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Paparazzi")
        {
            return;
        }
        else
        {
            if (collision != null)
            {
                StartCoroutine(BreakWine());
            }
        }
    }


    IEnumerator BreakWine()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<BoxCollider>().isTrigger = true;
        wineObj.SetActive(false);
        GameObject foggy = Instantiate(foggyEffect, this.transform);
        GameObject boom = Instantiate(boomEffect, this.transform);
        yield return new WaitForSeconds(6f);
        Destroy(this.gameObject);
        yield break;
    }
}
