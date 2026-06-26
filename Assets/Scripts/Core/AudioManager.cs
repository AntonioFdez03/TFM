using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundEntry
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private AudioSource audioSource;
    private AudioSource loopSource;

    [SerializeField] private SoundEntry[] sounds;

    private Dictionary<string, AudioClip> soundDictionary;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        audioSource = GetComponent<AudioSource>();

        soundDictionary = new Dictionary<string, AudioClip>();

        foreach (SoundEntry sound in sounds)
        {
            if (!string.IsNullOrEmpty(sound.name) && sound.clip != null)
            {
                soundDictionary[sound.name] = sound.clip;
            }
        }

        loopSource = gameObject.AddComponent<AudioSource>();
        loopSource.playOnAwake = false;
        loopSource.loop = true;
    }

    public void PlayOneShot(AudioClip audioClip, float audioScale = 1f)
    {
        if (audioClip != null)
            audioSource.PlayOneShot(audioClip, audioScale);
    }

    public void PlayOneShot(string audioName, float audioScale = 1f)
    {
        if (soundDictionary.TryGetValue(audioName, out AudioClip clip))
        {
            audioSource.PlayOneShot(clip, audioScale);
        }
        else
        {
            Debug.LogWarning($"Sound '{audioName}' not found.");
        }
    }

    public void Play(AudioClip audioClip, bool loop)
    {
        if (audioClip == null)
            return;

        audioSource.clip = audioClip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    public void PlayAmbient(string name)
    {
        if (soundDictionary.TryGetValue(name, out AudioClip clip))
        {   
            print("Entra al sonido");
            StartCoroutine(FadeInCR(clip, 2, 0.5f));
        }
    }

    public IEnumerator FadeInCR(AudioClip audio, float duration, float targetVolume)
    {   
        print("Entra");

        yield return StartCoroutine(FadeOutCR(duration));

        audioSource.clip = audio;
        audioSource.volume = 0f;
        audioSource.loop = true;
        audioSource.Play();

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    public IEnumerator FadeOutCR(float duration)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }

    public void PlayLoop(string audioName)
    {
        if (soundDictionary.TryGetValue(audioName, out AudioClip clip))
        {
            if (loopSource.clip == clip && loopSource.isPlaying)
                return;

            loopSource.volume = 0.6f;
            loopSource.clip = clip;
            loopSource.Play();
        }
    }

    public void StopLoop()
    {
        loopSource.Stop();
        loopSource.clip = null;
    }
}