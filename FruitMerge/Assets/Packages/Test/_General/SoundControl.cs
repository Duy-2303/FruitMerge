using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link
{
    public class SoundControl : MonoBehaviour
    {
        private static SoundControl ins;
        public static SoundControl Ins
        {
            get
            {
                if (ins == null)
                {
                    ins = FindObjectOfType<SoundControl>();
                }
                return ins;
            }
        }

        Dictionary<AudioClip, AudioSource> sources = new Dictionary<AudioClip, AudioSource>();
        [SerializeField] AudioClip[] audioClips;

        bool isTurnOn = true;
        float time;

        public bool IsEmpty => audioClips.Length == 0;

        private void Awake()
        {
            sources.Clear();

            AudioManager.Instance.onSfxVolumeChanged += TurnOnFx;
            AudioManager.Instance.OnUpdateAction();

            time = Time.time;
        }

        private void OnDestroy()
        {
            AudioManager.Instance.onSfxVolumeChanged -= TurnOnFx;
        }

        public void PlayFX<T>(T t, bool loop = false) where T : System.Enum
        {
            if(IsEmpty) return;

            int index = (int)(object)t;
            PlayFX(audioClips[index], loop);
        }

        public void PlayFX<T>(T t, float delay) where T : System.Enum
        {
            if(IsEmpty) return;
            StartCoroutine(IEPlayFX(t, delay));
        }

        public bool IsPlaying<T>(T t)
        {
            if(IsEmpty) return false;
            int index = (int)(object)t;
            return sources.ContainsKey(audioClips[index]) && sources[audioClips[index]] != null && sources[audioClips[index]].isPlaying;
        }

        public void StopFX<T>(T t) where T : System.Enum
        {
            if(IsEmpty) return;
            if (isTurnOn)
            {
                int index = (int)(object)t;
                if (sources.ContainsKey(audioClips[index]) && sources[audioClips[index]] != null)
                {
                    sources[audioClips[index]].Stop();
                }
            }
        }

        private IEnumerator IEPlayFX<T>(T t, float delay) where T : System.Enum
        {
            yield return new WaitForSeconds(delay);
            PlayFX(t);
        }

        public void PlayFX(AudioClip clip, bool loop = false)
        {
            if(IsEmpty) return;
            if (isTurnOn && time + 0.2f < Time.time)
            {
                if (!sources.ContainsKey(clip) || sources[clip] == null)
                {
                    sources[clip] = gameObject.AddComponent<AudioSource>();
                    sources[clip].loop = loop;
                    sources[clip].clip = clip;
                }
                if (sources.ContainsKey(clip) && sources[clip] != null)
                {
                    sources[clip].Play();
                }
            }
        }

        public void PlayFX(AudioClip clip, float delay)
        {
            if(IsEmpty) return;
            StartCoroutine(IEPlayFX(clip, delay));
        }

        private IEnumerator IEPlayFX(AudioClip t, float delay)
        {
            yield return new WaitForSeconds(delay);
            PlayFX(t);
        }

        public void StopFX(AudioClip clip)
        {
            if(IsEmpty) return;
            if (isTurnOn && sources.ContainsKey(clip) && sources[clip] != null)
            {
                sources[clip].Stop();
            }
        }


        private void TurnOnFx(bool turnOn)
        {
            isTurnOn = turnOn;

            if(!isTurnOn)
            {
                foreach (var item in sources)
                {
                    if (item.Value != null) item.Value.Stop();
                }
            }

#if UNITY_EDITOR
            isTurnOn = true;
#endif
        }

    }
}
