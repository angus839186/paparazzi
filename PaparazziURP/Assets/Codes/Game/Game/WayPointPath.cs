using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointPath : MonoBehaviour
{
    public Transform GetWayPoint(int wayPointIndex)
    {
        return transform.GetChild(wayPointIndex);
    }
    public int GetNextWayPointIndex(int currentWayIndex)
    {
        int nextWayIndex = currentWayIndex + 1;
        if(nextWayIndex == transform.childCount)
        {
            nextWayIndex = 0;
        }
        return nextWayIndex;
    }
}
