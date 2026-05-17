using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource audioSource;


    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayOneShot(AudioClip audioClip, float audioScale = 1)
    {
        audioSource.PlayOneShot(audioClip, audioScale);
    }

    public void Play(AudioClip audioClip, bool loop)
    {
        audioSource.clip = audioClip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void Stop()
    {
        audioSource.Stop();
    }
}