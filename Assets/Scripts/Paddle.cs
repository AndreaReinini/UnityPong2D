using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Paddle : MonoBehaviour
{

    public int id;
    public float minY; // Coordinata X minima
    public float maxY;// Coordinata X massima
    public float maxPotValue; // Valore massimo del potenziometro
    public float greatBoostWidth;
    public float badBoostWidth;
    public float excelentBoostWidth;
    public bool buttonStillToClick; //vero: il tasto boost deve essere ancora cliccato in fase di arrivo della palla mentre è nelle zone boost
    public BoostType boostType;
    public float moveSpeedPaddle;
    public float aiDeadZone;
    public int lightBallEffectDuration;

    private int direction = 0;
    
    

    private float startOfExcelentBoostZoneX;
    private Rigidbody2D rb2d;
    private (int, int) potButtonValue;
    private float endOfExcelentBoostZoneX;
    private float startOfGreatBoostZoneX;
    private float endOfGreatBoostZoneX;
    private float startOfBadBoostZoneX;
    private float endOfBadBoostZoneX;
    private int[] serialMessage;
    private float moveSpeedMultiplier;

    public enum BoostType
    {
        NoBoost, //0
        GreatBoost, //1
        BadBoost, //2
        ExcelentBoost //3
    }

    // Start is called before the first frame update
    void Awake()
    {
        buttonStillToClick = true;
        boostType = BoostType.NoBoost;
        rb2d = GetComponent<Rigidbody2D>();
        serialMessage = new int[7];
    }

    // Update is called once per frame
    void Update()
    {
        serialMessage = GameManager.instance.myMessageListener.GetMessageById(id);

        //Paddle sinistro
        if (IsLeftPaddle())
        {
            //se gioca bot
            if (GameManager.instance.IsPlayer1Ai())
            {
                //ma ho premuto tasto per far giocare player1
                if (serialMessage[2] == 0)
                {
                    //gioca player1
                    GameManager.instance.SwitchPlayMode(id);
                    MovePaddleByArduino();
                }
                //altrimenti muovi bot
                else
                    MoveAi();
            }
            //se turno giocatore ma premo tasto per far giocare bot
            else if (serialMessage[4] == 0)
            {
                GameManager.instance.SwitchPlayMode(id);
                MoveAi();
            }
            //altrimenti muovi player1
            else
                MovePaddleByArduino();
        }

        //Paddle destro
        if (!IsLeftPaddle())
        {
            if (GameManager.instance.IsPlayer2Ai())
            {
                if (serialMessage[3] == 0)
                {
                    GameManager.instance.SwitchPlayMode(id);
                    MovePaddleByArduino();
                }
                else
                    MoveAi();
            }
            else if (serialMessage[5] == 0)
            {
                GameManager.instance.SwitchPlayMode(id);
                MoveAi();
            }
            else
                MovePaddleByArduino();
        }
    }
    private bool IsAi()
    {
        return (IsLeftPaddle() && GameManager.instance.IsPlayer1Ai()) || (!IsLeftPaddle() && GameManager.instance.IsPlayer2Ai());
    }

    private int RandomBoostAi;

    private void MoveAi()
    {
        Vector2 ballPosition = GameManager.instance.ball.transform.position;
        if (Math.Abs(ballPosition.y - transform.position.y) > aiDeadZone)
        {
            direction = ballPosition.y > transform.position.y ? 1 : -1;
        }

        if (UnityEngine.Random.value < 0.01f)
        {
            moveSpeedMultiplier = UnityEngine.Random.Range(0.5f, 1.5f);
        }

        Vector2 velocity = new Vector2(0f, direction);
        rb2d.velocity = velocity * moveSpeedPaddle * moveSpeedMultiplier;

    }

    private void CheckBoost(int id, int buttonValue)
    {
        startOfExcelentBoostZoneX = IsLeftPaddle() ? (transform.position.x + (transform.localScale.x / 2)) : transform.position.x - transform.localScale.x/2;
        endOfExcelentBoostZoneX = IsLeftPaddle() ? startOfExcelentBoostZoneX + excelentBoostWidth : startOfExcelentBoostZoneX - excelentBoostWidth;
        startOfGreatBoostZoneX = endOfExcelentBoostZoneX;
        endOfGreatBoostZoneX = IsLeftPaddle() ? startOfGreatBoostZoneX + greatBoostWidth : startOfGreatBoostZoneX - greatBoostWidth;
        startOfBadBoostZoneX = endOfGreatBoostZoneX;
        endOfBadBoostZoneX = IsLeftPaddle() ? startOfBadBoostZoneX + badBoostWidth : startOfBadBoostZoneX - badBoostWidth;

        //se il primo paddle preme il tasto boost mentre la palla sta arrivando e non lo ha già cliccato nello stesso "turno" in una sua zona boost
        if (IsLeftPaddle() && buttonValue == 0 && GameManager.instance.ball.rb2d.velocity.x < 0 && buttonStillToClick)
        {
            //se la palla sta nella fascia "bad"
            if (GameManager.instance.ball.transform.position.x > startOfBadBoostZoneX && GameManager.instance.ball.transform.position.x <= endOfBadBoostZoneX)
            {
                boostType = BoostType.BadBoost;
                buttonStillToClick = false;
                GameManager.instance.lightBall.color = Color.red;
            }
            //great zone
            else if (GameManager.instance.ball.transform.position.x > startOfGreatBoostZoneX && GameManager.instance.ball.transform.position.x <= endOfGreatBoostZoneX)
            {
                boostType = BoostType.GreatBoost;
                buttonStillToClick = false;
                GameManager.instance.lightBall.color = Color.blue;
            }
            else if (GameManager.instance.ball.transform.position.x > startOfExcelentBoostZoneX && GameManager.instance.ball.transform.position.x <= endOfExcelentBoostZoneX)
            {
                boostType = BoostType.ExcelentBoost;
                buttonStillToClick = false;
                GameManager.instance.lightBall.color = Color.yellow;
            }
            else
            {
                GameManager.instance.lightBall.color = Color.white;
                boostType = BoostType.NoBoost;
            }
            if (boostType != BoostType.NoBoost)
                StartCoroutine(BoostCoroutine());
        }

        //se il secondo paddle preme il tasto boost mentre la palla sta arrivando e non lo ha già cliccato nello stesso "turno" in una sua zona boost
        if (!IsLeftPaddle() && buttonValue == 0 && GameManager.instance.ball.rb2d.velocity.x > 0 && buttonStillToClick)
        {
            //se la palla sta nella fascia "bad" [0; rangeMax)
            if (GameManager.instance.ball.transform.position.x >= endOfBadBoostZoneX && GameManager.instance.ball.transform.position.x < startOfBadBoostZoneX)
            {
                boostType = BoostType.BadBoost;
                buttonStillToClick = false;
                GameManager.instance.lightBall.color = Color.red;
            }
            //se la palla sta nella fascia "great" [rangeMax; 7.67)
            else if (GameManager.instance.ball.transform.position.x >= endOfGreatBoostZoneX && GameManager.instance.ball.transform.position.x < startOfGreatBoostZoneX)
            {
                boostType = BoostType.GreatBoost;
                buttonStillToClick = false;
                GameManager.instance.lightBall.color = Color.blue;
            }
            //excelent zone
            else if (GameManager.instance.ball.transform.position.x >= endOfExcelentBoostZoneX && GameManager.instance.ball.transform.position.x < startOfExcelentBoostZoneX)
            {
                boostType = BoostType.ExcelentBoost;
                buttonStillToClick = false;
                GameManager.instance.lightBall.color = Color.yellow;
            }
            else
            {
                GameManager.instance.lightBall.color = Color.white;
                boostType = BoostType.NoBoost;
            }
            if (boostType != BoostType.NoBoost)
                StartCoroutine(BoostCoroutine());
        }
        //Debug.Break();
    }

    IEnumerator BoostCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        GameManager.instance.lightBall.color = Color.white;

    }

    private void MovePaddleByArduino()
    {
        if (GameManager.instance.myMessageListener != null)
        {
            if (serialMessage[0] != -1000 && serialMessage[1] != -1000 && serialMessage[2] != -1000 && serialMessage[3] != -1000)
            {
                if (id == 1)
                {
                    MovePaddle(serialMessage[0]);
                    CheckBoost(id, serialMessage[2]);
                }
                else
                {
                    MovePaddle(serialMessage[1]);
                    CheckBoost(id, serialMessage[3]);
                }
            }
        }
    }


    private void MovePaddle(int potValue)
    {
        float normValue = Mathf.InverseLerp(0f, maxPotValue, potValue);
        float targetY = Mathf.Lerp(minY, maxY, normValue);
        transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
    }

    public float GetHeight()
    {
        return transform.localScale.y;
    }

    public bool IsLeftPaddle()
    {
        return id == 1;
    }
}
