using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TutorialTracker : MonoBehaviour
{
    public GameObject tracker;
    public GameObject canvas;

    public Camera cam;

    public Vector3 offset;

    public List<PaparazziPlayerController> targetList = new List<PaparazziPlayerController>();

    public List<Image> trackerList = new List<Image>();

    public PhotonView pv;
    public void CreateTracker()
    {
        if (pv.IsMine)
        {
            GameObject _tracker = Instantiate(tracker);
            _tracker.transform.parent = canvas.transform;
            _tracker.GetComponent<RectTransform>().sizeDelta = tracker.GetComponent<RectTransform>().sizeDelta;
            _tracker.GetComponent<RectTransform>().localScale = tracker.GetComponent<RectTransform>().localScale;

            Image _trackerToadd = _tracker.GetComponent<Image>();
            if (_trackerToadd != null)
            {
                Debug.Log("add a tracker");
                trackerList.Add(_trackerToadd);
            }
            else
            {
                Debug.Log("fail to add a tracker");
            }
            _tracker.gameObject.SetActive(true);
        }

    }
    public void AddTarget(PaparazziPlayerController paparazziPlayer)
    {
        targetList.Add(paparazziPlayer);

    }
    public void RemoveTarget()
    {
        for (int i = 0; i < trackerList.Count; i++)
        {
            Destroy(trackerList[i]);
            trackerList.Remove(trackerList[i]);
            targetList.Remove(targetList[i]);
        }
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            if (targetList.Count > 0)
            {
                TrackTarget();
            }
        }
    }
    public void TrackTarget()
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            if (targetList[i] != null)
            {
                float minX = trackerList[i].GetPixelAdjustedRect().width / 2;
                float maxX = Screen.width - minX;

                float minY = trackerList[i].GetPixelAdjustedRect().height / 2;
                float maxY = Screen.height - minY;

                Vector2 pos = cam.WorldToScreenPoint(targetList[i].transform.position + offset);

                if (Vector3.Dot((targetList[i].transform.position - transform.position), transform.forward) < 0)
                {
                    if (pos.x < Screen.width / 2)
                    {
                        pos.x = maxX;
                    }
                    else
                    {
                        pos.x = minX;
                    }
                }

                pos.x = Mathf.Clamp(pos.x, minX, maxX);
                pos.y = Mathf.Clamp(pos.y, minY, maxY);

                trackerList[i].transform.position = pos;
            }
        }
    }
}
