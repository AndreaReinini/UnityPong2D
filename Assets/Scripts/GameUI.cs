using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public ScoreText scoreTextPlayer1, scoreTextPlayer2;
    public GameObject menuObject;
    public System.Action onStartGame;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI volumeValueText;
    public TextMeshProUGUI playModeButtonText;
    public TextMeshProUGUI timerBallSpawnText;

    public void UpdateScores(int scorePlayer1, int scorePlayer2)
    {
        scoreTextPlayer1.SetScore(scorePlayer1);
        scoreTextPlayer2.SetScore(scorePlayer2);
    }
    /*
    private void Start()
    {
        AdjustPlayModeButtonText();
    }*/

    public void HighlightScore(int id)
    {
        if (id == 1)
            scoreTextPlayer1.Highlight();
        else
            scoreTextPlayer2.Highlight();
    }

    public void OnStartButtonClicked()
    {
        menuObject.SetActive(false);
        StartTimerBallSpawn();
        
    }

    private void StartTimerBallSpawn()
    {
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        timerBallSpawnText.text = "3";
        yield return new WaitForSeconds(1f);
        timerBallSpawnText.text = "2";
        yield return new WaitForSeconds(1f);
        timerBallSpawnText.text = "1";
        yield return new WaitForSeconds(1f);
        timerBallSpawnText.text = "";
        onStartGame?.Invoke();
        GameManager.instance.gameAlreadyResetted = false;
    }

    public void OnGameEnd(int winnerId)
    {
        menuObject.SetActive(true);
        UpdateScores(0, 0);
        winText.text = $"Giocatore {winnerId} ha vinto";
        GameManager.instance.ResetPlayMode();
        GameManager.instance.gameAlreadyStarted = false;

    }

    public void OnGameReset()
    {
        menuObject.SetActive(true);
        UpdateScores(0, 0);
        GameManager.instance.ResetPlayMode();
        GameManager.instance.gameAlreadyStarted = false;
    }


    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        volumeValueText.text = $"{Mathf.RoundToInt(value * 100)} %";
    }
}
