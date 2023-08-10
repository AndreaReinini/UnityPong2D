using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudio : MonoBehaviour
{
    public AudioSource asSound;
    public AudioClip winSound;
    public AudioClip scoreSound;

    public void PlayWinSound()
    {
        asSound.PlayOneShot(winSound);
    }

    public void PlayScoreSound()
    {
        asSound.PlayOneShot(scoreSound);
    }
}
