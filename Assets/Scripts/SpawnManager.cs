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
    private int _rareSpawnCounts = 0;
    private int _currentWave = 0;
    private int _numberofEnemiesSpawned = 0;
    [SerializeField]
    private int _currentEnemies = 0;
    private UIManager _uiManager;
    private Player _player;

    private void Start()
    {
        _uiManager = GameObject.Find("MainUI").GetComponent<UIManager>();
        _player = GameObject.Find("Player").GetComponent<Player>();
    }
    void Update()
    {
        if (_timeBeforeBuffs < Time.time && _buffsEnabled == false && _gameStarted == true)
        {
            _buffsEnabled = true;
            StartCoroutine(SpawnPowerUp());
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

    public void OnEnemyDeath()
    {
        _currentEnemies -= 1;
        if (_currentEnemies == 0)
        {
            StartCoroutine(SpawnEnemy());
            _player.AmmoBuff();
        }
    }
    IEnumerator SpawnEnemy()
    {
        _currentWave += 1;
        _currentEnemies = _currentWave * 5;
        _numberofEnemiesSpawned = 0;
        _uiManager.UpdateWave(_currentWave);
        if (_currentWave != 10)
        {
            while (_numberofEnemiesSpawned < (_currentWave * 5) && _stopSpawning == false)
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-9.62f, 9.58f), 7.0f, 0);
                GameObject newEnemy = Instantiate(_enemy, posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
                _numberofEnemiesSpawned += 1;
                yield return new WaitForSeconds(3.0f);
            }
        }
            
    }
    IEnumerator SpawnPowerUp()
    {
        while (_stopSpawning == false)
        {
            int powToSpawn = Random.Range(1, 6);
            if (powToSpawn == 5)
            {
                _rareSpawnCounts += 1;
                yield return new WaitForSeconds(Random.Range(10f, 15f));
                if (_rareSpawnCounts >= 5)
                {
                    Vector3 posToSpawn = new Vector3(Random.Range(-9.62f, 9.58f), 7.0f, 0);
                    _rareSpawnCounts = 0;
                    Instantiate(powerups[powToSpawn], posToSpawn, Quaternion.identity);
                    yield return new WaitForSeconds(Random.Range(10f, 15f));
                }
            }
            else
            {
                //Debug.Log(powToSpawn);
                Vector3 posToSpawn = new Vector3(Random.Range(-9.62f, 9.58f), 7.0f, 0);
                Instantiate(powerups[powToSpawn], posToSpawn, Quaternion.identity);
                yield return new WaitForSeconds(Random.Range(10f, 15f));
            }

        }
    }
}
