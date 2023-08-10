using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ball : MonoBehaviour
{
    public Rigidbody2D rb2d;
    public float maxInitialAngle;
    public float startMoveSpeed;
    public float maxStartY;
    public BallAudio ballAudio;
    public ParticleSystem collisionParticle;
    public BloomModifier bloomModifier;
    public float greatBoostSpeedMultiplier;
    public float badBoostSpeedDecreaser;
    public float noBoostSpeedMultiplier;
    public float excelentBoostSpeedMultiplier;
    public float maxSpeedMagnitude;
    public float maxCollisionAngle = 45f;

    private float velY;
    private Vector2 speedBallControlled;
    private float startX = 0f;
    private float absoluteDistanceFromCenter;
    private float relativeDistanceFromCenter;
    private int angleSign;
    private float angle;
    

    private void Awake()
    {
        speedBallControlled = Vector2.zero;
    }

    private void Start()
    {
        GameManager.instance.onReset += ResetBall;
        GameManager.instance.gameUI.onStartGame += ResetBall;
    }

    private void InitialPush()
    {
        rb2d = GetComponent<Rigidbody2D>(); //already done in the inspector
        Vector2 dir = UnityEngine.Random.value < 0.5f ? Vector2.left : Vector2.right;
        dir.y = UnityEngine.Random.Range(-maxInitialAngle, maxInitialAngle);
        rb2d.velocity = dir * startMoveSpeed;
        collisionParticle.startColor = Color.white;
        this.EmitParticle(32);
    }

    private void ResetBall()
    {
        ResetBallPosition();
        InitialPush();
    }

    private void ResetBallPosition()
    {
        float posY = UnityEngine.Random.Range(-maxStartY, maxStartY);
        Vector2 position = new Vector2(startX, posY);
        transform.position = position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        ScoreZone scoreZone = collision.GetComponent<ScoreZone>();
        if (scoreZone)
        {
            GameManager.instance.OnScoreZoneReached(scoreZone.id);
            GameManager.instance.screenshake.StartShake(0.33f, 0.1f);

        }
    }

    float ballVelocityMagnitude;

    private void IncrementBallSpeed(float speedMultiplier)
    {
        ballVelocityMagnitude = rb2d.velocity.magnitude;
        if (ballVelocityMagnitude >= 23f)
            rb2d.velocity *= 1f;
        else
            rb2d.velocity *= speedMultiplier;
    }

    private int randomAiBoost;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Paddle paddle = collision.collider.GetComponent<Paddle>();
       
        if (paddle != null)
        {
            ballAudio.PlayPaddleSound(); 
            AdjustAngle(paddle, collision);
            switch (paddle.boostType)
            {
                //caso no boost
                case Paddle.BoostType.NoBoost:
                    IncrementBallSpeed(noBoostSpeedMultiplier);
                    collisionParticle.startColor = Color.white;
                    break;
                //caso great boost
                case Paddle.BoostType.GreatBoost:
                    IncrementBallSpeed(greatBoostSpeedMultiplier);
                    collisionParticle.startColor = Color.blue;
                    break;
                //caso bad boost
                case Paddle.BoostType.BadBoost:
                    IncrementBallSpeed(badBoostSpeedDecreaser);
                    collisionParticle.startColor = Color.red;
                    break;
                case Paddle.BoostType.ExcelentBoost:
                    IncrementBallSpeed(excelentBoostSpeedMultiplier);
                    collisionParticle.startColor = Color.yellow;
                    
                    break;
            }

            paddle.buttonStillToClick = true;
            this.EmitParticle(16);
            paddle.boostType = Paddle.BoostType.NoBoost;
            //GameManager.instance.lightBall.color = Color.white;
            

            GameManager.instance.screenshake.StartShake(0.1f, 0.05f);

        }

        Debug.Log(rb2d.velocity.magnitude);

        Wall wall = collision.collider.GetComponent<Wall>();
        if (wall != null)
        {
            if (wall.isTopWall)
                GameManager.instance.testLightPinkWall.StartLightEffect();
            else
                GameManager.instance.testLightBlueWall.StartLightEffect();
            ballAudio.PlayWallSound();
            collisionParticle.startColor = Color.white;
            this.EmitParticle(8);
            GameManager.instance.screenshake.StartShake(0.033f, 0.033f);
        }
    }

    private void EmitParticle(int amount)
    {
        collisionParticle.Emit(amount);
    }

    private void AdjustAngle(Paddle paddle, Collision2D collision)
    {
        Vector2 median = Vector2.zero;

        foreach(ContactPoint2D point in collision.contacts)
        {
            median += point.point;
            Debug.DrawRay(point.point, Vector2.right, Color.red, 2f);
        }
        median /= collision.contactCount;
        Debug.DrawRay(median, Vector2.right, Color.cyan, 2f);

        absoluteDistanceFromCenter = median.y - paddle.transform.position.y;
        relativeDistanceFromCenter = absoluteDistanceFromCenter * 2 / paddle.GetHeight();

        //calculate rotation using quaternion
        angleSign = paddle.IsLeftPaddle() ? 1 : -1;
        angle = relativeDistanceFromCenter * maxCollisionAngle * angleSign;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Debug.DrawRay(median, Vector3.forward, Color.yellow, 2f);

        //calculate direction/velocity
        Vector2 dir = paddle.IsLeftPaddle() ? Vector2.right : Vector2.left;
        Vector2 velocity = rotation * dir * rb2d.velocity.magnitude;
        rb2d.velocity = velocity;
        Debug.DrawRay(median, velocity, Color.green, 2f);

    }
}
