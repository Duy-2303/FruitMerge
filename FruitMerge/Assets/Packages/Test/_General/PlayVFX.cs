using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link
{

    public class PlayVFX : MonoBehaviour
    {
        [SerializeField] ParticleSystem vfx;
        [SerializeField] AudioClip audioClip;

        public void Play()
        {
            vfx.Stop();
            vfx.Play();
            SoundControl.Ins.PlayFX(audioClip);
        }

        public void PlayLoop()
        {
            vfx.Stop();
            vfx.Play();
            SoundControl.Ins.PlayFX(audioClip, true);
        }

        public void Stop()
        {
            vfx.Stop();
            SoundControl.Ins.StopFX(audioClip);
        }
    }
}
