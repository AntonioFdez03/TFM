using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] private AudioClip fireSound;
    private AudioSource audioSource;
    private bool burnPlayer;
    private float fireDamage;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        fireDamage = 20;

        audioSource.clip = fireSound;
        audioSource.loop = true;
        audioSource.spatialBlend = 1f;

        audioSource.Play();
    }

    void Update()
    {
        if (burnPlayer)
        {
            PlayerController.instance.GetPlayerAttributes().TakeDamage(fireDamage);
        }
    }

    protected void OnTriggerEnter(Collider other)
    {   
        if (other.CompareTag("Player"))
            burnPlayer = true;
        
    }

    protected void OnTriggerExit(Collider other)
    {   
        if (other.CompareTag("Player"))
            burnPlayer = false;
    }
}