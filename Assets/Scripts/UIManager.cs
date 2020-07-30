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
    private Text _currentWave = null;
    [SerializeField]
    private CameraShake camerashake;

    void Start()
    {
        _gameOver.gameObject.SetActive(false);
        _scoreText.text = "Score: " + 0;
        _ammoText.text = "Ammo: " + 15 + " / " + 15;
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
}
