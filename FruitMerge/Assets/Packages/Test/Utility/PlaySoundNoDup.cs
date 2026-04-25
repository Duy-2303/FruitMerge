using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Play sound without duplicating the sound (won't play if the same sound is already playing)
public class PlaySoundNoDup : MonoBehaviour
{
    public AudioSource asrc;

    public void Play()
    {
        if (asrc != null && !asrc.isPlaying)
        {
            asrc.Play();
        }
    }
}
