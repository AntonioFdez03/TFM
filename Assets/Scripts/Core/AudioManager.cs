using System;
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
}