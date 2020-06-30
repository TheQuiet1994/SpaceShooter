using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidBehavior : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;
    [SerializeField]
    private bool _isDead;
    private SpawnManager _spawnManager;
    [SerializeField]
    private GameObject _explosion;


    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == ("Laser"))
        {
            _spawnManager.StartSpawn();
            Destroy(other.gameObject);
            Instantiate(_explosion, transform.position + new Vector3(0f, 0f, 0), Quaternion.identity);
            Destroy(this.gameObject);
        }

    }
}
