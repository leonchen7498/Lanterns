using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource audioSource;
    public float fadeTime = 1f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void FadeOutFadeInTrack(AudioClip trackToPlay)
    {
        StartCoroutine(TrackFader(trackToPlay));
    }

    public void PlayTrackFromSeconds(AudioClip trackToPlay, float startTimestamp)
    {
        audioSource.clip = trackToPlay;
        if(startTimestamp < trackToPlay.length && startTimestamp >= 0f)
        {
            audioSource.time = startTimestamp;
        }
        else
        {
            Debug.LogWarning("Tried to play " + trackToPlay.name + " from " + startTimestamp + "s start time. This start time is invalid. Playing from 0s instead.");
            audioSource.time = 0f;
        }
        audioSource.Play();
    }

    private IEnumerator TrackFader(AudioClip trackToFadeTo)
    {
        float startVolume = audioSource.volume;

        while (!Mathf.Approximately(GameManager.instance.audioSource.volume, 0))
        {
            audioSource.volume = Mathf.MoveTowards(GameManager.instance.audioSource.volume, 0, fadeTime * Time.deltaTime);
            yield return null;
        }
        audioSource.clip = trackToFadeTo;
        audioSource.Play();
        while (!Mathf.Approximately(GameManager.instance.audioSource.volume, startVolume))
        {
            audioSource.volume = Mathf.MoveTowards(GameManager.instance.audioSource.volume, startVolume, fadeTime * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }
}
