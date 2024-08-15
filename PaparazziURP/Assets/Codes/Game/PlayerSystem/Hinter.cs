using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hinter : MonoBehaviour
{
    public PhotonView pv;
    public Text HintText;
    public GameObject _hint;
    public void ChangeText(string text, bool _toggle)
    {
        if (pv.IsMine)
        {
            _hint.SetActive(_toggle);
            HintText.text = text;
        }
    }
}
