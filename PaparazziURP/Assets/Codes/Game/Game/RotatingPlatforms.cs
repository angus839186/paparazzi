using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatforms : MonoBehaviour
{
    [SerializeField]
    private WayPointPath _wayPointPath;

    [SerializeField]
    private float _speed;
    public int _targetWayPointIndex;
    private Transform _previousWayPoint;
    private Transform _targetWayPoint;
    public Transform Platform;

    public float _timeToWayPoint;
    public float _elapsedTime;

    public Quaternion rot1;

    private void Start()
    {
        TargetNextWayPoint();
    }
    private void FixedUpdate()
    {
        _elapsedTime += Time.deltaTime;
        float elapsedPercentage = _elapsedTime / _timeToWayPoint;
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
        Platform.rotation = Quaternion.Lerp(_previousWayPoint.rotation, _targetWayPoint.rotation, elapsedPercentage);
        if (elapsedPercentage >= 1)
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
        float distance = Vector3.Distance(_previousWayPoint.position, _targetWayPoint.position);
        _timeToWayPoint = distance / _speed;
    }
}
