using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCubeRotate : MonoBehaviour
{
    private float _speed = 40f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * _speed * (Time.deltaTime));
    }
}
