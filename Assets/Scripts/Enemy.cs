using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private GameObject _laserPrefab = null;
    private float _fireRate = 3.0f;
    private float _canFire = -1f;
    [SerializeField]
    private float _speed = 4.0f;
    private int _mapLoops = 0;
    private bool _isDead;
    private Player _player;
    [SerializeField]
    private GameObject _enemyDeath;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime); //Basic Movement
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            LaserBehavior[] lasers = enemyLaser.GetComponentsInChildren<LaserBehavior>();
            
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemy();
            }
        }
        LeaveMap();
        
    }

    private void LeaveMap()
    {
        if (transform.position.y < -6)
        {
            transform.position = new Vector3(Random.Range(-8.0f, 8.0f), 7.3f, 0);
            _mapLoops = _mapLoops + 1;
            if (_mapLoops > 10)
            {
                _speed = 0f;
                Instantiate(_enemyDeath, transform.position + new Vector3(0f, 0f, 0), Quaternion.identity);
                Destroy(this.gameObject);
            }          
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == ("Player"))
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null && _isDead != true)
            {
                _isDead = true;
                player.Damage();
            }
            _speed = 0f;
            Instantiate(_enemyDeath, transform.position + new Vector3(0f, 0f, 0), Quaternion.identity);
            Destroy(this.gameObject);
        }

        if (other.tag == ("Laser"))
        {
            Destroy(other.gameObject);
            if (_player != null && _isDead != true)
            {
                _isDead = true;
                _player.ScoreUpdate(Random.Range(10, 16));
            }
            _speed = 0f;
            Instantiate(_enemyDeath, transform.position + new Vector3(0f, 0f, 0), Quaternion.identity);
            Destroy(this.gameObject);
        }

    }
}
