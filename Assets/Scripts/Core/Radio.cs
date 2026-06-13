using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Radio : MonoBehaviour, IInteractiveObject
{   
    [SerializeField] private AudioClip radioSound;
    private AudioSource audioSource;
    private bool active = false;

    void Start()
    {
        audioSource.clip = radioSound;
        audioSource.loop = false;
    }

    public void Interact()
    {   
        active = !active;

        if(active)
            audioSource.Play();
        else
            audioSource.Stop();
            
    }
}