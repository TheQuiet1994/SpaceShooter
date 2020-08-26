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
    [SerializeField]
    private Text _gameOver = null;
    [SerializeField]
    private Text _gameWin = null;
    [SerializeField]
    private Text _currentWave = null;
    [SerializeField]
    private CameraShake camerashake;
    private GameManager _gameManager;

    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _gameOver.gameObject.SetActive(false);
        _scoreText.text = "Score: " + 0;
        _ammoText.text = "Ammo: " + 25 + " / " + 25;
        _currentWave.text = "Current Wave: " + 0;
    }


    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }
    public void GameOverSequence()
    {
        StartCoroutine(camerashake.Shake(.8f, .7f));
        _gameOver.gameObject.SetActive(true);
        StartCoroutine(FlickerGameOver());
    }

    public void GameWinSequence()
    {
        _gameWin.gameObject.SetActive(true);
        _gameManager.GameOver();
        StartCoroutine(FlickerWin());
    }
    public void UpdateAmmo(int currentAmmo, int maxammo)
    {
        if (currentAmmo > 0)
        {
            _ammoText.text = "Ammo: " + currentAmmo + " / " + maxammo;
        }
        else if (currentAmmo == 0)
        {
            _ammoText.text = "OUT OF AMMO!";
        }
    }
    public void UpdateWave(int currentWave)
    {
            _currentWave.text = "Current Wave: " + currentWave;
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

    IEnumerator FlickerWin()
    {
        while (true)
        {
            _gameWin.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            _gameWin.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.0f);
        }
    }
}
