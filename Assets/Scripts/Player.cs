using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Movement Speed
    [SerializeField]
    private float _speed = 5.5f;
    [SerializeField]
    private float _thrusterSpeed = 25.5f;
    
    //Fire Rate / Cooldown between attacks
    [SerializeField]
    private float _fireRate = 0.1f;
    private float _nextFire = -1f;

    //Player numbers
    [SerializeField]
    private int _currentHealth = 4;
    [SerializeField]
    private int _maxHealth = 4;
    private int _shieldHP = 0;
    [SerializeField]
    private int _laserAmmoCurrent = 15;
    [SerializeField]
    private int _laserAmmoMax = 15;
    [SerializeField]
    private float _energy = 5f;
    [SerializeField]
    private float _maxEnergy = 5f;

    //Prefabs for Effects
    [SerializeField]
    private GameObject _multiLaserPrefab = null;
    [SerializeField]
    private GameObject _laserPrefab = null;
    [SerializeField]
    private GameObject _tripleshotPrefab = null;
    [SerializeField]
    private GameObject _shields = null;
    [SerializeField]
    private GameObject _fire75 = null;
    [SerializeField]
    private GameObject _fire50 = null;
    [SerializeField]
    private GameObject _fire25 = null;
    [SerializeField]
    private GameObject _explosion = null;
    [SerializeField]
    private CameraShake camerashake;

    //Sound Effects
    [SerializeField]
    private AudioClip _laserSound;
    private AudioSource _audiosource;


    //Bools for Powerup Effects
    [SerializeField]
    private bool _hasTripleShot = false;
    [SerializeField]
    private bool _hasSpeedBuff = false;
    [SerializeField]
    private bool _hasShieldBuff = false;
    [SerializeField]
    private bool _hasMultishot = false;
    [SerializeField]
    private bool _isThrusting = false;
    private bool _thrusterCooldownPunish = false;

    //Manager initialization
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private GameManager _gameManager;

    //UI Initializaiton
    [SerializeField]
    private int _score;
    private bool _isGameOver;
    [SerializeField]
    private HealthBar _healthbar;
    [SerializeField]
    private HealthBar _shieldbar;
    [SerializeField]
    private EnergyBar _energybar;



    void Start()
    {
        _audiosource = GetComponent<AudioSource>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _currentHealth = _maxHealth;
        _healthbar.SetHealth(_currentHealth);
        _shieldbar.SetHealth(_shieldHP);
        _energybar.SetEnergy(_energy);
        _energybar.SetMaxEnergy(_maxEnergy);

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL.");
        }

        if (_audiosource == null)
        {
            Debug.LogError("The Audio Source is NULL.");
        }
        else 
        {
            _audiosource.clip = _laserSound;
        }

        if (_gameManager == null)
        {
            Debug.LogError("The Game Manager is NULL.");
        }
    }
    void Update()
    {
        switch (_currentHealth)
        {
            case 4:
                _fire75.SetActive(false);
                _fire50.SetActive(false);
                _fire25.SetActive(false);
                break;
            case 3:
                _fire75.SetActive(true);
                _fire50.SetActive(false);
                _fire25.SetActive(false);
                break;

            case 2:
                _fire75.SetActive(true);
                _fire50.SetActive(true);
                _fire25.SetActive(false);
                break;

            case 1:
                _fire75.SetActive(true);
                _fire50.SetActive(true);
                _fire25.SetActive(true);
                break;

            default:
                _fire25.SetActive(false);
                _fire50.SetActive(false);
                _fire75.SetActive(false);
                break;

        }
        CalculateMovement();
        Shoot();     
    }   
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        //Thruster and Speed Buff Movement System
        if (Input.GetKey(KeyCode.LeftShift) && _hasSpeedBuff == false)
        {
            _isThrusting = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && _hasSpeedBuff == false)
        {
            _isThrusting = false;
            _speed = 5.5f;
        }
        if (_energy > 0 && _thrusterCooldownPunish == false && _isThrusting == false)
        {
            _energy += Time.deltaTime;
            _energybar.SetEnergy(_energy);
            if (_energy >= 5)
            {
                _energy = 5f;
                _energybar.SetEnergy(_energy);
            }
        }
        else if (_isThrusting == true && _thrusterCooldownPunish == false)
        {
            _speed = _thrusterSpeed;
            if (_energy > 0 && _thrusterCooldownPunish == false)
            {
                _energy -= Time.deltaTime;
                _energybar.SetEnergy(_energy);
                if (_energy <= 0)
                {
                    _isThrusting = false;
                    _speed = 5.5f;
                    StartCoroutine(ThrusterCooldownPunish());
                }
            }
        }
        if (_hasSpeedBuff == true)
        {
            _speed = 10f;
        }
        else if (_hasSpeedBuff == false)
        {
            _speed = 5.5f;
        }
        if (_thrusterCooldownPunish == true)
        {
            _energy += Time.deltaTime;
            _energybar.SetEnergy(_energy);
            if (_energy >= 5)
            {
                _thrusterCooldownPunish = false;
                _energy = 5f;
                _energybar.SetEnergy(_energy);
            }
        }

        //Movement System
        transform.Translate(direction * _speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -2.3f, 0), 0);

        if (transform.position.x > 10.5f)
        {
            transform.position = new Vector3(-10.5f, transform.position.y, 0);
        }
        else if (transform.position.x < -10.5f)
        {
            transform.position = new Vector3(10.5f, transform.position.y, 0);
        }
    }
    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire && _laserAmmoCurrent > 0)
        {
            _nextFire = Time.time + _fireRate;
            if (_hasTripleShot == true)
            {
                if (_laserAmmoCurrent > 3)
                {
                    _audiosource.Play(0);
                    Instantiate(_tripleshotPrefab, transform.position + new Vector3(0f, 0f, 0), Quaternion.identity);
                    _laserAmmoCurrent -= 3;
                    _uiManager.UpdateAmmo(_laserAmmoCurrent, _laserAmmoMax);
                }
            }
            else if (_hasMultishot == true)
            {
                if (_laserAmmoCurrent > 6)
                {
                    _audiosource.Play(0);
                    Instantiate(_multiLaserPrefab, transform.position + new Vector3(0, 1.0f, 0), Quaternion.identity);
                    _laserAmmoCurrent -= 6;
                    _uiManager.UpdateAmmo(_laserAmmoCurrent, _laserAmmoMax);
                }  
            }
            else
            {
                _audiosource.Play(0);
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.0f, 0), Quaternion.identity);
                _laserAmmoCurrent -= 1;
                _uiManager.UpdateAmmo(_laserAmmoCurrent, _laserAmmoMax);
            }           
        }
    }
    public void Damage()
    {
        if (_shieldHP > 0)
        {
            _shieldHP = _shieldHP - 1;
            _shieldbar.SetHealth(_shieldHP);
            if (_shieldHP <= 0 && _hasShieldBuff == true)
            {
                _hasShieldBuff = false;
                _shields.SetActive(false);
            }
        }
        else
        {
            _currentHealth -= 1;
            _healthbar.SetHealth(_currentHealth);
            StartCoroutine(camerashake.Shake(.15f, .2f));
            if (_currentHealth <= 0)
            {         
                _spawnManager.OnPlayerDeath();
                Instantiate(_explosion, transform.position + new Vector3(0f, 0f, 0), Quaternion.identity);
                _gameManager.GameOver();
                _uiManager.GameOverSequence();
                Destroy(this.gameObject);
            }
        }     
    }
    public void TripleShotBuffDuration()
    {
        _laserAmmoCurrent = 15;
        _uiManager.UpdateAmmo(_laserAmmoCurrent, _laserAmmoMax);
        _hasTripleShot = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    } 
    public void SpeedBuffDuration()
    {
        if (_hasSpeedBuff == false)
        {
            _hasSpeedBuff = true;
            _fireRate = 0.2f;
            StartCoroutine(SpeedPowerDownRoutine());
        }        
    }
    public void ShieldBuff()
    {
        if (_hasShieldBuff == false)
        {
            _hasShieldBuff = true;
            _shieldHP = 3;
            _shieldbar.SetHealth(_shieldHP);
            _shields.SetActive(true);
        }
    }
    public void AmmoBuff()
    {
        if (_laserAmmoCurrent != _laserAmmoMax)
        {
            _laserAmmoCurrent = _laserAmmoMax;
            _uiManager.UpdateAmmo(_laserAmmoCurrent, _laserAmmoMax);
        }
    }
    public void HealthBuff()
    {
        if (_currentHealth > 0 && _currentHealth != _maxHealth)
        {
            _currentHealth += 1;
            _healthbar.SetHealth(_currentHealth);
        }
    }
    public void ScoreUpdate(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
    public void MultishotBuff()
    {
        _laserAmmoCurrent = 15;
        _uiManager.UpdateAmmo(_laserAmmoCurrent, _laserAmmoMax);
        _hasMultishot = true;
        StartCoroutine(MultiShotPowerDownRoutine());
    }
    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _hasTripleShot = false;
    }
    IEnumerator SpeedPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _hasSpeedBuff = false;
        _speed = 5.5f;
        _fireRate = 0.5f;
    }
    IEnumerator MultiShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _hasMultishot = false;
    }
    IEnumerator ThrusterCooldownPunish()
    {
        _thrusterCooldownPunish = true;
        yield return new WaitForSeconds(5f);
    }
}
