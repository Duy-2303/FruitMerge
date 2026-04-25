using System.Collections;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public bool playOnEnable;
    public bool playOnlyIfEnabled = true;
    public float delay = 0;
    public AudioClip[] clips;

    public void PlayRandom()
    {
        StartCoroutine(_PlayRandom());
    }
    private IEnumerator _PlayRandom()
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        if (clips.Length > 0)
        {
            if (enabled || !playOnlyIfEnabled) AudioManager.PlaySFX(clips[Random.Range(0, clips.Length)]);
        }
    }
    public void Play(int index)
    {
        StartCoroutine(_Play(index));
    }
    private IEnumerator _Play(int index)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        if (enabled || !playOnlyIfEnabled) AudioManager.PlaySFX(clips[index]);
    }

    private void OnEnable()
    {
        if (playOnEnable)
        {
            PlayRandom();
        }
    }
}