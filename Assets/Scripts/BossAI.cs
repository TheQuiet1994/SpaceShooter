using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    //Movement and Status Values
    private float _speed = 1.2f;
    private bool _isDead = false;
    private int _health = 10;
    private bool isInPosition = false;
    
    //Inspector Driven Fields
    [SerializeField]
    private GameObject _laserPrefab = null;
    [SerializeField]
    private GameObject _enemyDeath = null;
    [SerializeField]
    private GameObject _explosion = null;
    [SerializeField]
    private GameObject _laserSource = null;
    [SerializeField]
    private AudioClip _laserSound = null;
    [SerializeField]
    private GameObject _beam = null;

    //Initialize these on Start or They'll be null
    private Player _player = null; 
    private SpawnManager _spawnManager = null;
    private UIManager _uiManager = null;
    private AudioSource _audiosource = null;

    //Variables for Attacks and Starting Movement
    private int callCount = 0;
    private bool _attackMove = false;
    private Vector3 _startingPosition;
    private int _positionTick = 0;
    
    //Abilities Variables
    private bool _isOrbitAttack = false;
    private int _orbitAttacks = 0;
    private int _beamAttackCount = 0;
    private bool _isBeamAttack = false;

    void Start()
    {
        _audiosource = GetComponent<AudioSource>();
        if (_audiosource == null)
        {
            Debug.LogError("The Audio Source is NULL.");
        }
        else
        {
            _audiosource.clip = _laserSound;
        }
        _uiManager = GameObject.Find("MainUI").GetComponent<UIManager>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
    }
    void Update()
    {
        Movement();
    }
    private void Movement()
    {
        if (transform.position.x < -7.11)
        {
            transform.position = new Vector3(-7.11f, transform.position.y, 0);
        }
        else if (transform.position.x > 7.1f)
        {
            transform.position = new Vector3(7.1f, transform.position.y, 0);
        }
        if (!_isOrbitAttack)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
            if (transform.position.y < 2.78f)
            {
                transform.position = new Vector3(transform.position.x, 2.78f, 0);
                isInPosition = true;
            }
        }
        if (isInPosition && callCount != 1)
        {
            callCount += 1;
            StartCoroutine(StartDelay());
            _startingPosition = transform.position;
        }
        if (_attackMove)
        {
            if (_startingPosition.x <= 0)
            {
                _positionTick = 0;
            }
            else if (_startingPosition.x >= 0.1f)
            {
                _positionTick = 1;
            }
            switch (_positionTick)
            {
                case 0:
                    transform.Translate(Vector3.right * _speed * Time.deltaTime);
                    break;
                case 1:
                    transform.Translate(Vector3.left * _speed * Time.deltaTime);
                    break;
                default:
                    break;
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
                Debug.Log("Push! - NOT CODED");
            }
        }
        else if (other.tag == ("Laser"))
        {
            Destroy(other.gameObject);
            GameObject laserExplosion = Instantiate(_explosion, new Vector3(other.transform.position.x, (other.transform.position.y + 0.5f), other.transform.position.z), Quaternion.identity);
            float _size = RandomExplosionSize();
            laserExplosion.transform.localScale = new Vector3(_size, _size, _size);
            _health -= 1;
            if (_health < 1)
            {
                _uiManager.GameWinSequence();
                _isDead = true;
                _speed = 0f;         
                GameObject bossDeath = Instantiate(_enemyDeath, transform.position + new Vector3(0f, 0f, 0), Quaternion.identity);
                bossDeath.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f); 
                Destroy(this.gameObject);
            }
        }
    } //Death and Being Attacked
    private int RandomNum()
    {
        int RandomNum = Random.Range(1, 10);
        Debug.Log(RandomNum);
        return RandomNum;
    } //Random Number Generator (1 - 10)
    private float RandomExplosionSize()
    {
        float _size = Random.Range(0.1f, 0.4f);
        return _size;
    } //Randomize Explosion Size Effect
    private void OrbitAttackStart()
    {
        _isOrbitAttack = true;
        StartCoroutine(OrbitAttack());
    } //Orbit Attack Method for Coroutine
    IEnumerator StartDelay() //Delay 3 seconds after spawning; random attack
    {
        yield return new WaitForSeconds(3f);
        if (RandomNum() <= 5)
        {
            _isOrbitAttack = true;
            StartCoroutine(OrbitAttack());
        }
        else
        {
            _isBeamAttack = true;
            StartCoroutine(BeamAttack());
        }
    }
    IEnumerator OrbitAttack() //Send 40 lasers out rotating around boss
    {
        while (_isOrbitAttack == true && _orbitAttacks < 40)
        {
            _attackMove = true;
            GameObject enemyLaser = Instantiate(_laserPrefab, _laserSource.transform.position, _laserSource.transform.rotation);
            _audiosource.Play(0);
            LaserBehavior[] lasers = enemyLaser.GetComponentsInChildren<LaserBehavior>();
            _orbitAttacks += 1;           
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignBoss();
            }
            if (_orbitAttacks >= 40)
            {
                _attackMove = false;
                _startingPosition = transform.position;
                StopAllCoroutines();
                _isOrbitAttack = false;
                _orbitAttacks = 0;
                StartCoroutine(StartDelay());
            }
            yield return new WaitForSeconds(0.3f);
        }   
    }
    IEnumerator BeamAttack() //Activate Beam Attack GameObject and Move
    {
        while (_isBeamAttack == true && _beamAttackCount < 5)
        {
            _attackMove = true;
            _beam.SetActive(true);
            _beamAttackCount += 1;
            if (_beamAttackCount >= 4)
            {
                _attackMove = false;
                _beam.SetActive(false);
                _startingPosition = transform.position;
                StopAllCoroutines();
                _isBeamAttack = false;
                _beamAttackCount = 0;
                StartCoroutine(StartDelay());
            }
            yield return new WaitForSeconds(2f);
        }
    }
}
