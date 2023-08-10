using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Ball ball;
    public GameAudio gameAudio;
    public int scorePlayer1 = 0;
    public int scorePlayer2 = 0;
    public System.Action onReset;
    public GameUI gameUI;
    public Shake screenshake;
    public int maxScore = 4;
    public PlayMode playMode;
    public MyMessageListener myMessageListener;
    public int[] serialMessageReceveid;
    public bool gameAlreadyStarted;
    public bool gameAlreadyResetted;
    public Paddle leftPaddle;
    public Paddle rightPaddle;


    public TestLight testLightPinkWall;
    public TestLight testLightBlueWall;
    //public UnityEngine.Rendering.Universal.Light2D lightBall;
    public UnityEngine.Rendering.Universal.Light2D lightBall;
    

    public enum PlayMode
    {
        PlayerVsPlayer,
        PlayerVsAi,
        AiVsAi,
        AiVsPlayer
    }

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            gameUI.onStartGame += OnStartGame;
        }
    }

    private void Start()
    {
        playMode = PlayMode.AiVsAi;
        gameAlreadyStarted = false;
        serialMessageReceveid = new int[7];
        gameAlreadyResetted = false;
    }

    private void Update()
    {
        serialMessageReceveid = myMessageListener.GetMessageById(0);

        if (serialMessageReceveid[6] == 0 && gameAlreadyStarted == false)
        {
            gameAlreadyStarted = true;
            gameAlreadyResetted = true;
            gameUI.OnStartButtonClicked();
            
        }
        if (serialMessageReceveid[4] == 0 && serialMessageReceveid[5] == 0 && gameAlreadyResetted == false)
        {
            gameAlreadyResetted = true;
            gameUI.OnGameReset();
            ball.transform.position = Vector3.zero;
            ball.rb2d.velocity = Vector2.zero;

        }
    }


    private void OnDestroy()
    {
        gameUI.onStartGame -= OnStartGame;
    }

    public void OnScoreZoneReached(int id)
    {

        if (id == 1)
            scorePlayer1+=1;
        else
            scorePlayer2++;
        gameUI.UpdateScores(scorePlayer1, scorePlayer2);
        gameUI.HighlightScore(id);
        CheckWin();
        lightBall.color = Color.white;
        leftPaddle.boostType = Paddle.BoostType.NoBoost;
        rightPaddle.boostType = Paddle.BoostType.NoBoost;
        ball.collisionParticle.startColor = Color.white;
        

    }

    private void CheckWin()
    {
        int winnerId = scorePlayer1 == maxScore ? 1 : scorePlayer2 == maxScore ? 2 : 0;
        
        if (winnerId != 0)
        {
            GameManager.instance.gameUI.OnGameEnd(winnerId);
            gameAudio.PlayWinSound();
            ball.transform.position = Vector3.zero;
            ball.rb2d.velocity = Vector2.zero;

        }
        else
        {
            gameAudio.PlayScoreSound();
            onReset?.Invoke();
        }
        
    }

    private void OnStartGame()
    {
        scorePlayer1 = 0;
        scorePlayer2 = 0;
        gameUI.UpdateScores(scorePlayer1, scorePlayer2);
    }

    public void SwitchPlayMode(int idPaddle)
    {
        if (idPaddle == 1)
        {
            switch (playMode)
            {
                case PlayMode.AiVsPlayer:
                    playMode = PlayMode.PlayerVsPlayer;
                    break;
                case PlayMode.AiVsAi:
                    playMode = PlayMode.PlayerVsAi;
                    break;
                case PlayMode.PlayerVsAi:
                    playMode = PlayMode.AiVsAi;
                    break;
                case PlayMode.PlayerVsPlayer:
                    playMode = PlayMode.AiVsPlayer;
                    break;
            }
        }

        if (idPaddle == 2)
        {
            switch (playMode)
            {
                case PlayMode.PlayerVsAi:
                    playMode = PlayMode.PlayerVsPlayer;
                    break;
                case PlayMode.AiVsAi:
                    playMode = PlayMode.AiVsPlayer;
                    break;
                case PlayMode.PlayerVsPlayer:
                    playMode = PlayMode.PlayerVsAi;
                    break;
                case PlayMode.AiVsPlayer:
                    playMode = PlayMode.AiVsAi;
                    break;
            }
        }
    }

    public bool IsPlayer2Ai()
    {
        return playMode == PlayMode.PlayerVsAi || playMode == PlayMode.AiVsAi;
    }

    public bool IsPlayer1Ai()
    {
        return (playMode == PlayMode.AiVsAi || playMode == PlayMode.AiVsPlayer);
    }

    public void ResetPlayMode()
    {
        playMode = PlayMode.AiVsAi;
    }
}
