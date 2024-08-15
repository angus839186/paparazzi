using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePoint : MonoBehaviour
{
    [Range(0f, 2f)]
    [SerializeField] private float chasePointsize = 1f;
    private void OnDrawGizmos()
    {
        foreach (Transform t in transform)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(t.position, 0.1f);
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < transform.childCount - 1 ; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }
    }
    public Transform GetNextChasePoint(Transform currentChasePoint)
    {
        if(currentChasePoint == null)
        {
            return transform.GetChild(0);

        }
        if(currentChasePoint.GetSiblingIndex() < transform.childCount -1)
        {
            return transform.GetChild(currentChasePoint.GetSiblingIndex() + 1);
        }
        else
        {
            return transform.GetChild(0);
        }
        return null;
    }
}
