using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText = null;
    private int _scoreRead = 0;
    [SerializeField]
    private Text _gameOver = null;

    [SerializeField]
    private Image _healthImage = null;
    [SerializeField]
    private Image _shieldImage = null;
    [SerializeField]
    private Sprite[] _healthstates = null;
    [SerializeField]
    private Sprite[] _shieldStates = null;

    private bool _gameisOver = false;

    void Start()
    {
        _gameOver.gameObject.SetActive(false);
        _scoreText.text = "Score: " + 0;
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
