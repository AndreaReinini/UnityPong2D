using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAudio : MonoBehaviour
{
    public AudioSource asSound;
    public AudioClip paddleSound;
    public AudioClip wallSound;

    public void PlayPaddleSound()
    {
        asSound.PlayOneShot(paddleSound);
    }

    public void PlayWallSound()
    {
        asSound.PlayOneShot(wallSound);
    }
}
