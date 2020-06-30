using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour


{
    [SerializeField]
    private GameObject _enemy;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] powerups;
    [SerializeField]
    private float _timeBeforeBuffs = 8f;
    private bool _buffsEnabled = false;
    private bool _stopSpawning = false;
    private bool _gameStarted = false;

    void Start()
    {
        
    }


    void Update()
    {
        if (_timeBeforeBuffs < Time.time && _buffsEnabled == false && _gameStarted == true)
        {
            _buffsEnabled = true;
            StartCoroutine(SpawnPowerUp());
        }
    }
    IEnumerator SpawnEnemy()
    {
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-9.62f, 9.58f), 7.0f, 0);
            GameObject newEnemy = Instantiate(_enemy, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(3.0f);
        }          
    }

    IEnumerator SpawnPowerUp()
    {
        while (_stopSpawning == false)
        {
            int powToSpawn = Random.Range(0, 4);
            Vector3 posToSpawn = new Vector3(Random.Range(-9.62f, 9.58f), 7.0f, 0);   
            Instantiate(powerups[powToSpawn], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(10f, 15f));
        }
    }

    public void StartSpawn()
    {
        _gameStarted = true;
        StartCoroutine(SpawnEnemy());
        _timeBeforeBuffs = Time.time + 8f;

    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
