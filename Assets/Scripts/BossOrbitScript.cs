using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossOrbitScript : MonoBehaviour
{
    //Array of waypoints to walk
    public Transform[] waypoints;

    [SerializeField]
    private float _speed = 2f;
    [SerializeField]
    private int _waypointIndex = 0;

    void Start()
    {
        transform.position = waypoints[_waypointIndex].transform.position;
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (_waypointIndex < waypoints.Length)
        {
            transform.position = Vector2.MoveTowards(transform.position, waypoints[_waypointIndex].transform.position, _speed * Time.deltaTime);
            if (transform.position == waypoints[_waypointIndex].transform.position)
            {
                _waypointIndex += 1;
            }
        }
        if (_waypointIndex >= waypoints.Length)
        {
            _waypointIndex = 0;
        }
    }
}
