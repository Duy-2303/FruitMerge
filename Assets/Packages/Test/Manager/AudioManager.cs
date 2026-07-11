using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;


public enum CommonSound
{
    Click,
    FormOpen,
    FormClose,
    LevelEnd,
    LevelStart,
    Success,
    Meow,
    Fail,
    Win,
    SaveMe,
}

public class AudioManager : Singleton<AudioManager>
{
    private const string KEY_BGM_VOLUME = "MusicVolume";
    private const string KEY_SFX_VOLUME = "SFXVolume";
    private const string KEY_BGM_ACTIVE = "MusicActive";
    private const string KEY_SFX_ACTIVE = "SFXActive";
    public const string MIXER_GROUP_BGM = "Music";
    public const string MIXER_GROUP_SFX = "SFX";
    private const float MUTE_VOLUME_THRESHOLD = 0.1f;

    public AudioMixer audioMixer; // Assign in the inspector
    public AudioSource MusicSource { get; private set; } // For background music
    public AudioSource SfxSource { get; private set; }  // For sound effects

    [System.Serializable]
    public struct CommonSoundPair
    {
        public CommonSound id;
        public AudioClip[] clips;
    }
    [SerializeField] private CommonSoundPair[] commonSounds;
    [SerializeField] private float musicVolumeMul = 1f;
    [SerializeField] private float sfxVolumeMul = 1f;

    public delegate void OnMusicVolumeChanged(bool isOn);
    public event OnMusicVolumeChanged onBgmVolumeChanged;
    public event OnMusicVolumeChanged onSfxVolumeChanged;
    public bool IsMusicOn => PlayerPrefs.GetFloat(KEY_BGM_VOLUME, 1f) > MUTE_VOLUME_THRESHOLD && PlayerPrefs.GetInt(KEY_BGM_ACTIVE, 1) == 1;
    public bool IsSfxOn => PlayerPrefs.GetFloat(KEY_SFX_VOLUME, 1f) > MUTE_VOLUME_THRESHOLD && PlayerPrefs.GetInt(KEY_SFX_ACTIVE, 1) == 1;

    public event System.Action<AudioClip> onBgmPlayed;
    public event System.Action onBgmStopped;

    protected void Awake()
    {
        //base.Awake();

        // Don't destroy this object when loading a new scene
        DontDestroyOnLoad(gameObject);

        // Add audio source components dynamically
        MusicSource = gameObject.AddComponent<AudioSource>();
        SfxSource = gameObject.AddComponent<AudioSource>();

        // Configure audio sources
        MusicSource.loop = true; // Enable looping for the music source

        if (audioMixer != null)
        {
            MusicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(MIXER_GROUP_BGM)[0];
            SfxSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(MIXER_GROUP_SFX)[0];
        }

        //IngameDebugConsole.DebugLogManager.Instance.onCmdAudioBypass.AddListener(() =>
        //{
        //    bool bypass = PlayerPrefs.GetInt("AudioBypass", 0) == 1;
        //    if (bypass)
        //    {
        //        // bypass all audio mixer effects
        //    }
        //});
    }

    public void OnUpdateAction()
    {
        onBgmVolumeChanged?.Invoke(IsMusicOn);
        onSfxVolumeChanged?.Invoke(IsSfxOn);
    }

    private void Start()
    {
        // Load saved volumes
        SetMusicVolume(PlayerPrefs.GetFloat(KEY_BGM_VOLUME, 1f));
        SetSFXVolume(PlayerPrefs.GetFloat(KEY_SFX_VOLUME, 1f));
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat(KEY_BGM_VOLUME, volume);
        PlayerPrefs.Save();

        volume *= musicVolumeMul;
        audioMixer?.SetFloat(KEY_BGM_VOLUME, -80 * (1 - volume));

        onBgmVolumeChanged?.Invoke(IsMusicOn);
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat(KEY_SFX_VOLUME, volume);
        PlayerPrefs.Save();

        volume *= sfxVolumeMul;
        audioMixer?.SetFloat(KEY_SFX_VOLUME, -80 * (1 - volume));

        onSfxVolumeChanged?.Invoke(IsSfxOn);
    }

    public void StopMusicWithoutNotify()
    {
        MusicSource.Stop();
    }

    #region Static
    public static void PlayMusic(AudioClip clip, float volume = 1f)
    {
        Instance.MusicSource.volume = volume;
        Instance.MusicSource.clip = clip;
        Instance.MusicSource.Play();
        Instance.onBgmPlayed?.Invoke(clip);
    }

    public static void StopMusic()
    {
        Instance.MusicSource.Stop();
        Instance.onBgmStopped?.Invoke();
    }

    public static void PlaySFX(params AudioClip[] clips)
    {
        if (clips != null && clips.Length > 0)
        {
            AudioClip c = clips[UnityEngine.Random.Range(0, clips.Length)];
            if (c != null)
            {
                Instance.SfxSource.PlayOneShot(c);
            }
        }
    }

    public static void PlaySFX(CommonSound sound)
    {
        foreach (var item in Instance.commonSounds)
        {
            if (item.id == sound)
            {
                PlaySFX(item.clips);
                break;
            }
        }
    }

    public static void PlaySFX(AudioClip clips, float volume = 1f)
    {
        Instance.SfxSource.PlayOneShot(clips, volume);
    }

    public static void StopSFX()
    {
        Instance.SfxSource.Stop();
    }

    public static AudioSource PlaySfx(AudioClip clip, float volume = 1f, bool isLoop = false)
    {
        Instance.SfxSource.loop = isLoop;
        Instance.SfxSource.volume = volume;
        Instance.SfxSource.clip = clip;
        Instance.SfxSource.Play();
        return Instance.SfxSource;
    }

    public static AudioSource StopSfx()
    {
        Instance.SfxSource.loop = false;
        Instance.SfxSource.Stop();
        return Instance.SfxSource;
    }

    public static void ChangeSfxVolume(float volume)
    {
        Instance.SfxSource.volume = volume;
    }

    public static void PrintVolume()
    {
        Debug.LogError("Music Volume: " + PlayerPrefs.GetFloat(KEY_BGM_VOLUME, 1f));
        Debug.LogError("SFX Volume: " + PlayerPrefs.GetFloat(KEY_SFX_VOLUME, 1f));
    }
    #endregion
}