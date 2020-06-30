using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText = null;
    [SerializeField]
    private Text _ammoText = null;
    private int _scoreRead = 0;
    [SerializeField]
    private Text _gameOver = null;
    private Coroutine _lastRoutine = null;
    [SerializeField]
    private Image _healthImage = null;
    [SerializeField]
    private Image _shieldImage = null;
    [SerializeField]
    private Sprite[] _healthstates = null;
    [SerializeField]
    private Sprite[] _shieldStates = null;
    private bool _coroutineRunning = false;

    private bool _gameisOver = false;

    void Start()
    {
        _gameOver.gameObject.SetActive(false);
        _scoreText.text = "Score: " + 0;
        _ammoText.text = "Ammo: " + 15 + " / " + 15;
    }


    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }

    public void UpdateHealth(int currentHealth)
    {
        _healthImage.sprite = _healthstates[currentHealth];

        if (currentHealth == 0)
        {
            GameOverSequence();
        }
    }

    public void GameOverSequence()
    {
        _gameOver.gameObject.SetActive(true);
        StartCoroutine(FlickerGameOver());
    }

    public void UpdateShields(int currentShields)
    {
        _shieldImage.sprite = _shieldStates[currentShields];
    }

    public void UpdateAmmo(int currentAmmo, int maxammo)
    {
        if (currentAmmo > 0)
        {
            if (_coroutineRunning == true)
            {
                _ammoText.text = "Ammo: " + currentAmmo + " / " + maxammo;
                StopCoroutine(_lastRoutine);
            }
            else if (_coroutineRunning == false)
            {
                _ammoText.text = "Ammo: " + currentAmmo + " / " + maxammo;
            }
            
        }
        else if (currentAmmo == 0)
        {
            _ammoText.text = "OUT OF AMMO!";
            _lastRoutine = StartCoroutine(FlickerOutofAmmo());
        }
        
    }
    IEnumerator FlickerOutofAmmo()
    {
        while (true)
        {
            _ammoText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            _ammoText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator FlickerGameOver()
    {
        while (true)
        {
            _gameOver.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            _gameOver.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.0f);
        }
    }
}
