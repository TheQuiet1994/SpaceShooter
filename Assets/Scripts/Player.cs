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
    private int _health = 4;
    private int _shieldHP = 0;
    [SerializeField]
    private int _laserAmmoCurrent = 15;
    [SerializeField]
    private int _laserAmmoMax = 15;

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
    private GameObject _sprintThruster = null;

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

    //Manager initialization
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private GameManager _gameManager;

    //UI Initializaiton
    [SerializeField]
    private int _score;
    private bool _isGameOver;



    void Start()
    {
        _audiosource = GetComponent<AudioSource>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

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
        switch (_health)
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

        if (Input.GetKey(KeyCode.LeftShift) && _hasSpeedBuff == false)
        {
            _speed = _thrusterSpeed;
            _sprintThruster.SetActive(true);
        }
        else if (_hasSpeedBuff == true)
        {
            _speed = 10f;
            _sprintThruster.SetActive(false);
        }
        else if (_hasSpeedBuff == false)
        {
            _speed = 5.5f;
            _sprintThruster.SetActive(false);
        }
        transform.Translate(direction * _speed * Time.deltaTime);



        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

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
            _uiManager.UpdateShields(_shieldHP);
            if (_shieldHP <= 0 && _hasShieldBuff == true)
            {
                _hasShieldBuff = false;
                _shields.SetActive(false);
            }
        }
        else
        {
            _health = _health - 1;
            _uiManager.UpdateHealth(_health);
            if (_health <= 0)
            {
                _spawnManager.OnPlayerDeath();
                Instantiate(_explosion, transform.position + new Vector3(0f, 0f, 0), Quaternion.identity);
                _gameManager.GameOver();
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
            _uiManager.UpdateShields(_shieldHP);
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
        if (_health > 0 && _health < 4)
        {
            _health += 1;
            _uiManager.UpdateHealth(_health);
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
}
