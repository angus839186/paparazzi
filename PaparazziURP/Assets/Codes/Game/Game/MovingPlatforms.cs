using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    [SerializeField]
    private WayPointPath _wayPointPath;

    [SerializeField]
    private float _speed;
    public int _targetWayPointIndex;
    private Transform _previousWayPoint;
    private Transform _targetWayPoint;

    public float _timeToWayPoint;
    public float _elapsedTime;

    private void Start()
    {
        TargetNextWayPoint();
    }
    private void FixedUpdate()
    {
        _elapsedTime += Time.deltaTime;
        float elapsedPercentage = _elapsedTime / _timeToWayPoint;
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
        transform.position = Vector3.Lerp(_previousWayPoint.position, _targetWayPoint.position, elapsedPercentage);
        if(elapsedPercentage >= 1)
        {
            TargetNextWayPoint();
        }
    }

    private void TargetNextWayPoint()
    {
        _previousWayPoint = _wayPointPath.GetWayPoint(_targetWayPointIndex);
        _targetWayPointIndex = _wayPointPath.GetNextWayPointIndex(_targetWayPointIndex);
        _targetWayPoint = _wayPointPath.GetWayPoint(_targetWayPointIndex);

        _elapsedTime = 0;

        float distanceToWayPoint = Vector3.Distance(_previousWayPoint.position, _targetWayPoint.position);
        _timeToWayPoint = distanceToWayPoint / _speed;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    var player = collision.gameObject.GetComponentInParent<StandOnPlatform>();
    //    if (player != null)
    //    {
    //        player.gameObject.transform.SetParent(transform);
    //        player.GetComponentInParent<StandOnPlatform>().ChangeParent(this.GetComponent<PhotonView>().ViewID);
    //    }
    //}
    //private void OnCollisionExit(Collision collision)
    //{
    //    var player = collision.gameObject.GetComponentInParent<StandOnPlatform>();
    //    if (player != null)
    //    {
    //        player.gameObject.transform.SetParent(null);
    //        player.GetComponent<StandOnPlatform>().LeaveParent();
    //    }
    //    else
    //    {
    //        return;
    //    }
    //}
    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponentInParent<StandOnPlatform>();
        if (player != null)
        {
            player.ChangeParent(this.GetComponent<PhotonView>().ViewID);
        }
        else
        {
            return;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var player = other.gameObject.GetComponentInParent<StandOnPlatform>();
        if (player == null)
        {
            return;
        }
        else
        {
            player.LeaveParent();
        }
    }
}
